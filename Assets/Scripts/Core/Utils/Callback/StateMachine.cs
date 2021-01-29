using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

namespace Core.Utils {

	/// <summary>
	/// 状态机 V 2.0
	/// </summary>
	public class StateMachine : EnumTypeProcessor {

		#region 回调项

		/// <summary>
		/// 更新回调项
		/// </summary>
		public class UpdateCallbackItem : BaseCallbackItem<string> {

			/// <summary>
			/// 函数名
			/// </summary>
			protected override string methodName => "_update" + name;

		}

		/// <summary>
		/// 进入回调项
		/// </summary>
		public class EnterCallbackItem : BaseCallbackItem<string> {

			/// <summary>
			/// 函数名
			/// </summary>
			protected override string methodName => "_enter" + name;

		}

		/// <summary>
		/// 退出回调项
		/// </summary>
		public class ExitCallbackItem : BaseCallbackItem<string> {

			/// <summary>
			/// 函数名
			/// </summary>
			protected override string methodName => "_exit" + name;

		}

		/// <summary>
		/// 切换回调项（需要手动注册）
		/// </summary>
		public class ChangeCallbackItem : BaseCallbackItem<Tuple<string ,string>> {

			// Empty

		}

		#endregion

		/// <summary>
		/// 状态字典 (state, action)
		/// </summary>
		//protected DictList<string, UnityAction> stateDict =
		//	new DictList<string, UnityAction>();
		//protected DictList<string, UnityAction> stateEnters =
		//	new DictList<string, UnityAction>();
		//protected DictList<string, UnityAction> stateExits =
		//	new DictList<string, UnityAction>();
		protected DictCallback<string, UpdateCallbackItem> stateDict =
			new DictCallback<string, UpdateCallbackItem>();
		protected DictCallback<string, EnterCallbackItem> stateEnters =
			new DictCallback<string, EnterCallbackItem>();
		protected DictCallback<string, ExitCallbackItem> stateExits =
			new DictCallback<string, ExitCallbackItem>();

		/// <summary>
		/// 状态改变字典 (<state, state>, action)
		/// </summary>
		//protected DictList<Tuple<string, string>, UnityAction> stateChanges =
		//	new DictList<Tuple<string, string>, UnityAction>();
		protected DictCallback<Tuple<string, string>, ChangeCallbackItem> stateChanges =
			new DictCallback<Tuple<string, string>, ChangeCallbackItem>();
		
		/// <summary>
		/// 当前状态
		/// </summary>
		public string state { get; protected set; } = "";
		public string lastState { get; protected set; } = "";

		/// <summary>
		/// 状态改变回调
		/// </summary>
		public UnityAction onStateChanged { get; set; } = null;

		#region 更新控制

		/// <summary>
		/// 更新（每帧）
		/// </summary>
		public virtual void update() {
			updateState();
			updateStateChange();
		}

		/// <summary>
		/// 更新状态机
		/// </summary>
		void updateState() {
			stateDict.on(state);
			//var data = stateDict[state];
			//if (data == null) return;

			//foreach (var action in data) action?.Invoke();
		}

		/// <summary>
		/// 更新状态变化
		/// </summary>
		void updateStateChange() {
			if (isStateChanged()) {
				onStateChanged?.Invoke();

				processStateExit(lastState);
				processStateChange(lastState, state);
				processStateEnter(state);
			}

			lastState = state;
		}

		/// <summary>
		/// 处理状态变化
		/// </summary>
		void processStateChange(string from, string to) {
			if (from == to) return;
			stateChanges.on(getKey(from, to));
			//var actions = getStateChange(from, to);
			//if (actions != null)
			//	foreach (var action in actions) action?.Invoke();
		}

		/// <summary>
		/// 处理状态进入
		/// </summary>
		/// <param name="to"></param>
		void processStateEnter(string to) {
			stateEnters.on(to);
			//var actions = getStateEnter(to);
			//if (actions != null)
			//	foreach (var action in actions) action?.Invoke();
		}

		/// <summary>
		/// 处理状态退出
		/// </summary>
		/// <param name="from"></param>
		void processStateExit(string from) {
			stateExits.on(from);
			//var actions = getStateExit(from);
			//if (actions != null)
			//	foreach (var action in actions) action?.Invoke();
		}

		#endregion

		#region 状态控制

		/// <summary>
		/// 是否处于状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns>是否存在</returns>
		public bool isState(params string[] states) {
			foreach (var state in states)
				if (this.state == state) return true;
			return false;
		}
		public bool isState(params Enum[] states) {
			foreach (var state in states)
				if (this.state == state.ToString()) return true;
			return false;
		}

		/// <summary>
		/// 状态是否改变
		/// </summary>
		/// <returns>状态改变</returns>
		public bool isStateChanged() {
			return lastState != state;
		}

		/// <summary>
		/// 改变状态
		/// </summary>
		/// <param name="state">新状态</param>
		public void changeState(string state, bool force = false, object logObj = null) {
			Debug.Log((logObj ?? this) + ".changeState: " + this.state + " -> " + state);
			if ((force || hasState(state)) && this.state != state) this.state = state;
		}
		public void changeState(Enum state, bool force = false, object logObj = null) {
			changeState(state.ToString(), force, logObj);
		}

		#endregion

		#region 状态字典

		/// <summary>
		/// 添加状态字典
		/// </summary>
		/// <param name="state">状态</param>
		/// <param name="action">动作</param>
		public void addStateDict(string state, UnityAction action = null) {
			stateDict.add(state, action);
		}
		public void addStateDict(Enum state, UnityAction action = null) {
			addStateDict(state.ToString(), action);
		}
		public void addStateDict<E>() where E : Enum {
			addStateDict(typeof(E));
		}
		public void addStateDict(Type type) {
			setupType(type);
		}

		/// <summary>
		/// 删除状态字典
		/// </summary>
		/// <param name="state"></param>
		/// <param name="action"></param>
		public void removeStateDict(string state, UnityAction action = null) {
			stateDict.remove(state, action);
		}
		public void removeStateDict(Enum state, UnityAction action = null) {
			removeStateDict(state.ToString(), action);
		}

		/// <summary>
		/// 是否存在状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns>是否存在</returns>
		public bool hasState(string state) {
			return stateDict.contains(state);
		}
		public bool hasState(Enum state) {
			return hasState(state.ToString());
		}

		#endregion

		#region 状态改变字典

		/// <summary>
		/// 获取改变字典键
		/// </summary>
		public Tuple<string, string> getKey(string from, string to) {
			return new Tuple<string, string>(from, to);
		}

		/// <summary>
		/// 注册状态切换函数
		/// </summary>
		/// <param name="from">始状态</param>
		/// <param name="to">末状态</param>
		/// <param name="action">动作</param>
		public void addStateChange(string from, string to, UnityAction action) {
			if (from == to) return;
			stateChanges.add(getKey(from, to), action);
		}
		public void addStateChange(Enum from, Enum to, UnityAction action) {
			addStateChange(from.ToString(), to.ToString(), action);
		}

		/// <summary>
		/// 删除状态切换函数
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="action"></param>
		public void removeStateChange(string from, string to, UnityAction action) {
			stateChanges.remove(getKey(from, to), action);
		}
		public void removeStateChange(Enum from, Enum to, UnityAction action) {
			removeStateChange(from.ToString(), to.ToString(), action);
		}

		/// <summary>
		/// 注册状态进入函数
		/// </summary>
		/// <param name="enumType">类型</param>
		/// <param name="to">初状态</param>
		/// <param name="action">动作</param>
		public void addStateEnter(string to, UnityAction action) {
			stateEnters.add(to, action);
		}
		public void addStateEnter(Enum to, UnityAction action) {
			addStateEnter(to.ToString(), action);
		}

		/// <summary>
		/// 删除状态进入函数
		/// </summary>
		/// <param name="to"></param>
		/// <param name="action"></param>
		public void removeStateEnter(string to, UnityAction action) {
			stateEnters.remove(to, action);
		}
		public void removeStateEnter(Enum to, UnityAction action) {
			removeStateEnter(to.ToString(), action);
		}

		/// <summary>
		/// 注册状态退出函数
		/// </summary>
		/// <param name="enumType">类型</param>
		/// <param name="from">初状态</param>
		/// <param name="action">动作</param>
		public void addStateExit(string from, UnityAction action) {
			stateExits.add(from, action);
		}
		public void addStateExit(Enum from, UnityAction action) {
			addStateExit(from.ToString(), action);
		}

		/// <summary>
		/// 删除状态退出函数
		/// </summary>
		/// <param name="from"></param>
		/// <param name="action"></param>
		public void removeStateExit(string from, UnityAction action) {
			stateExits.remove(from, action);
		}
		public void removeStateExit(Enum from, UnityAction action) {
			removeStateExit(from.ToString(), action);
		}

		/// <summary>
		/// 是否存在状态变更
		/// </summary>
		public bool hasStateChange(string from, string to) {
			return stateChanges.contains(getKey(from, to));
		}
		public bool hasStateChange(Enum from, Enum to) {
			return hasStateChange(from.ToString(), to.ToString());
		}

		/// <summary>
		/// 是否存在状态变更
		/// </summary>
		public ChangeCallbackItem getStateChange(string from, string to) {
			return stateChanges.get(getKey(from, to));
		}
		public ChangeCallbackItem getStateChange(Enum from, Enum to) {
			return getStateChange(from.ToString(), to.ToString());
		}

		/// <summary>
		/// 是否存在状态进入
		/// </summary>
		public EnterCallbackItem getStateEnter(string to) {
			return stateEnters.get(to);
		}
		public EnterCallbackItem getStateEnter(Enum to) {
			return getStateEnter(to.ToString());
		}

		/// <summary>
		/// 是否存在状态退出
		/// </summary>
		public ExitCallbackItem getStateExit(string from) {
			return stateExits.get(from);
		}
		public ExitCallbackItem getStateExit(Enum from) {
			return getStateExit(from.ToString());
		}

		#endregion

		#region 注册对象管理

		/// <summary>
		/// 注册对象
		/// </summary>
		public void registerObject(object obj) {
			stateDict.registerObject(obj);
			stateEnters.registerObject(obj);
			stateExits.registerObject(obj);
			stateChanges.registerObject(obj);
		}

		/// <summary>
		/// 移除对象
		/// </summary>
		public void removeObject(object obj) {
			stateDict.removeObject(obj);
			stateEnters.removeObject(obj);
			stateExits.removeObject(obj);
			stateChanges.removeObject(obj);
		}

		#endregion

		/// <summary>
		/// 处理值
		/// </summary>
		/// <param name="val"></param>
		protected override void processValue(string val) {
			stateDict.create(val);
			stateEnters.create(val);
			stateExits.create(val);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public StateMachine() { }
		public StateMachine(Type type, params object[] objs) : base(type) {
			foreach (var obj in objs) registerObject(obj);
		}
		//	addStateDict(type);
		//}
	}
}
