using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

namespace BattleModule.Data {

	/// <summary>
	/// 战斗者
	/// </summary>
	/// <typeparam name="S"></typeparam>
	public abstract partial class BaseRuntimeBattler<S, T> 
		: BaseRuntimeBattler<T> where S : BaseState<T> where T : TraitData {

		/// <summary>
		/// 属性
		/// </summary>
		//[AutoConvert]
		public Dictionary<int, RuntimeState<S, T>> states { get; protected set; }
			= new Dictionary<int, RuntimeState<S, T>>();

		/// <summary>
		/// 重置
		/// </summary>
		public override void reset() {
			base.reset(); clearStates();
		}

		#region 数据读取

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			this.states.Clear();

			var states = DataLoader.load(json, "states");
			if (states != null) {
				Debug.Log("Load states: " + states.ToJson());
				states.SetJsonType(JsonType.Object);
				foreach (KeyValuePair<string, JsonData> pair in states) {
					var key = int.Parse(pair.Key);
					var data = DataLoader.load<RuntimeState<S, T>>(pair.Value);
					Debug.Log("Load states: " + key + ", " + data);
					this.states.Add(key, data);
				}
			}

		}

		/// <summary>
		/// 转化自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);
			var states = new JsonData();

			states.SetJsonType(JsonType.Object);
			foreach (var pair in this.states)
				states[pair.Key.ToString()] = DataLoader.convert(pair.Value);

			json["states"] = states;
		}

		#endregion

		#region 特性控制

		/// <summary>
		/// 特性对象
		/// </summary>
		/// <returns></returns>
		public override List<ITraitsObject<T>> traitObjects() {
			var res = base.traitObjects();
			res.AddRange(allStates());
			return res;
		}

		/// <summary>
		/// 所有状态特性
		/// </summary>
		List<T> statesTraits() {
			var res = new List<T>();
			foreach (var state in states)
				res.AddRange(state.Value.traits);
			return res;
		}

		#endregion

		#region 状态控制

		/// <summary>
		/// 状态是否改变
		/// </summary>
		bool _isStateChanged = false;
		public bool isStateChanged {
			get {
				var res = _isStateChanged;
				_isStateChanged = false; return res;
			}
		}

		/// <summary>
		/// 获取所有状态
		/// </summary>
		/// <returns></returns>
		public List<S> allStates() {
			var res = new List<S>(states.Count);
			foreach (var pair in states)
				res.Add(pair.Value.state());
			return res;
		}

		/// <summary>
		/// 获取所有运行时状态
		/// </summary>
		/// <returns></returns>
		public List<RuntimeState<S, T>> allRuntimeStates() {
			var res = new List<RuntimeState<S, T>>(states.Count);
			foreach (var pair in states) res.Add(pair.Value);
			return res;
		}

		#region 状态变更

		/// <summary>
		/// 添加状态
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">变化值</param>
		/// <param name="rate">变化率</param>
		/// <param name="turns">持续回合</param>
		/// <returns>返回添加的Buff</returns>
		public RuntimeState<S, T> addState(S state, int turns = 0) {
			return addState(state.id, turns);
		}
		public RuntimeState<S, T> addState(int stateId, int turns = 0) {
			RuntimeState<S, T> state;
			_isStateChanged = true;

			if (states.ContainsKey(stateId)) {
				state = states[stateId];
				state.add(turns);
			} else {
				state = new RuntimeState<S, T>(stateId, turns);
				states.Add(stateId, state);
				on(BaseBattlerCallback.StateAdded, state);
			}

			return state;
		}

		/// <summary>
		/// 移除状态
		/// </summary>
		/// <param name="stateId">状态ID</param>
		/// <param name="turns">移除回合数</param>
		public RuntimeState<S, T> removeState(int stateId, int turns = 0, bool force = false) {
			if (!containsState(stateId)) return null;
			var state = states[stateId];
			_isStateChanged = true;

			if (turns <= 0) {
				states.Remove(stateId);
				on(BaseBattlerCallback.StateRemoved, state, force);
			} else {
				state.remove(turns);
				if (state.isOutOfDate())
					removeState(stateId, force: force);
			}

			return state;
		}
		/// <param name="buff">Buff对象</param>
		public RuntimeState<S, T> removeState(RuntimeState<S, T> state, int turns = 0, bool force = false) {
			return removeState(state.stateId, turns, force);
		}

		/// <summary>
		/// 移除多个满足条件的状态
		/// </summary>
		/// <param name="p">条件</param>
		public void removeStates(Predicate<RuntimeState<S, T>> p, bool force = true) {
			for (int i = states.Count - 1; i >= 0; --i)
				if (p(states[i])) removeState(i, force: force);
		}

		/// <summary>
		/// 清除所有Debuff
		/// </summary>
		public void removeNegeStates() {
			removeStates(state => state.isNega());
		}

		/// <summary>
		/// 清除所有状态
		/// </summary>
		public void clearStates(bool force = true) {
			var tmp = new Dictionary<int, RuntimeState<S, T>>(states);
			foreach (var pair in tmp)
				removeState(pair.Value, force: force);
		}

		#endregion

		#region 状态判断

		/// <summary>
		/// 是否处于指定条件的状态
		/// </summary>
		public bool containsState(int stateId) {
			return states.ContainsKey(stateId);
		}
		public bool containsState(Predicate<RuntimeState<S, T>> p) {
			foreach (var pair in states)
				if (p(pair.Value)) return true;
			return false;
		}

		/// <summary>
		/// 是否存在负面状态
		/// </summary>
		public bool anyNegaState() {
			return containsState(state => state.isNega());
		}

		/// <summary>
		/// 获取指定条件的状态
		/// </summary>
		public RuntimeState<S, T> getState(int stateId) {
			return states[stateId];
		}
		public RuntimeState<S, T> getState(Predicate<RuntimeState<S, T>> p) {
			foreach (var pair in states)
				if (p(pair.Value)) return pair.Value;
			return null;
		}

		/// <summary>
		/// 获取指定条件的状态（多个）
		/// </summary>
		public List<RuntimeState<S, T>> getStates(Predicate<RuntimeState<S, T>> p) {
			var res = new List<RuntimeState<S, T>>();
			foreach (var pair in states)
				if (p(pair.Value)) res.Add(pair.Value);
			return res;
		}

		/// <summary>
		/// 是否处于可移动状态
		/// </summary>
		/// <returns></returns>
		public bool isMovableState() {
			return !isEscaped && !isDead();
		}

		#endregion

		#endregion

		#region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		protected override void _onBattleStart() {
			base._onBattleStart();
			clearStates();
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		/// <param name="round"></param>
		protected override void _onRoundStart(int round) {
			base._onRoundStart(round);
			_isStateChanged = false;
		}

		#endregion

		#region 回调

		/// <summary>
		/// 状态添加回调
		/// </summary>
		protected virtual void _onStateAdded(RuntimeState<S, T> state) { }

		/// <summary>
		/// 状态解除回调
		/// </summary>
		protected virtual void _onStateRemoved(RuntimeState<S, T> state, bool force = false) { }

		/// <summary>
		/// 回合结束回调
		/// </summary>
		/// <param name="round"></param>
		protected override void _onRoundEnd(int round) {
			base._onRoundEnd(round);
			processStatesRoundEnd();
		}

		/// <summary>
		/// 处理状态回合结束
		/// </summary>
		void processStatesRoundEnd() {
			var states = this.states.ToArray();

			foreach (var pair in states) {
				var state = pair.Value;
				state.onRoundEnd();
				_isStateChanged = true;
				if (state.isOutOfDate())
					removeState(state.stateId);
			}
		}

		#endregion
	}
}
