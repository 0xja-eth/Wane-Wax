using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Core.Data;
using Core.Utils;

namespace BattleModule.Data {

	/// <summary>
	/// 战斗管理器
	/// </summary>
	public abstract class BaseRuntimeBattle : RuntimeData {

		/// <summary>
		/// 战斗结果
		/// </summary>
		public enum Result {
			None = 0, Win = 1, Lose = 2, Draw = 3 // 平手
		}

		/// <summary>
		/// 玩家、敌人
		/// </summary>
		[AutoConvert]
		public List<BaseRuntimeBattler> actors { get; protected set; } = new List<BaseRuntimeBattler>();
		[AutoConvert]
		public List<BaseRuntimeBattler> enemies { get; protected set; } = new List<BaseRuntimeBattler>();
		[AutoConvert]
		public int round { get; protected set; }
		[AutoConvert]
		public double time { get; protected set; } // 战斗时间
		[AutoConvert]
		public Result result { get; protected set; } = Result.None; // 战斗结果

		/// <summary>
		/// 状态机
		/// </summary>
		public StateMachine stateMachine;

		/// <summary>
		/// 状态机
		/// </summary>
		public CallbackManager cbManager;

		/// <summary>
		/// 状态
		/// </summary>
		[AutoConvert]
		public string state {
			get => stateMachine.state;
			set { changeState(value, true); }
		}

		/// <summary>
		/// 战斗者
		/// </summary>
		BaseRuntimeBattler[] _battlers = null;
		public BaseRuntimeBattler[] battlers {
			get {
				var actorCnt = actors.Count;
				var enemyCnt = enemies.Count;
				if (_battlers == null || _battlers.Length != actorCnt + enemyCnt) {

					_battlers = new BaseRuntimeBattler[actorCnt + enemyCnt];

					for (int i = 0; i < actorCnt; ++i) _battlers[i] = actors[i];
					for (int i = 0; i < enemyCnt; ++i) _battlers[i + actorCnt] = enemies[i];
				}
				return _battlers;
			}
		}

		#region 状态/回调管理

		/// <summary>
		/// 状态类
		/// </summary>
		public virtual Type stateType => null;

		/// <summary>
		/// 是否处于状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns>是否存在</returns>
		public bool isState(string state) {
			return stateMachine.isState(state);
		}
		public bool isState(Enum state) {
			return stateMachine.isState(state);
		}

		/// <summary>
		/// 状态是否改变
		/// </summary>
		/// <returns>状态改变</returns>
		public bool isStateChanged() {
			return stateMachine.isStateChanged();
		}

		/// <summary>
		/// 改变状态
		/// </summary>
		/// <param name="state">新状态</param>
		public void changeState(string state, bool force = false) {
			stateMachine.changeState(state, force, this);
		}
		public void changeState(Enum state, bool force = false) {
			stateMachine.changeState(state, force, this);
		}

		/// <summary>
		/// 状态类
		/// </summary>
		public virtual Type cbType => null;

		/// <summary>
		/// 触发事件
		/// </summary>
		/// <param name="type"></param>
		public void on(string type, params object[] params_) {
			cbManager.on(type, params_);
		}

		/// <summary>
		/// 判断事件
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool judge(string name) {
			return cbManager.judge(name);
		}

		#endregion

		#region 成员控制

		/// <summary>
		/// 生成敌人
		/// </summary>
		protected virtual List<BaseRuntimeBattler> generateEnemies() {
			return new List<BaseRuntimeBattler>();
		}

		/// <summary>
		/// 添加敌人
		/// </summary>
		/// <param name="enemy"></param>
		public void addEnemy(BaseRuntimeBattler enemy) {
			enemies.Add(enemy);
		}

		/// <summary>
		/// 添加队员
		/// </summary>
		/// <param name="enemy"></param>
		public void addActor(BaseRuntimeBattler enemy) {
			actors.Add(enemy);
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 开始/结束判断
		/// </summary>
		public virtual bool isStarted => !isEnd;
		public virtual bool isEnd => isState(BaseBattleState.End);

		/// <summary>
		/// 角色失败
		/// </summary>
		/// <returns></returns>
		public bool isActorsLost => actors.All(b => b.isLost());

		/// <summary>
		/// 敌人失败
		/// </summary>
		/// <returns></returns>
		public bool isEnemiesLost => enemies.All(b => b.isLost());

		/// <summary>
		/// 战斗开始
		/// </summary>
		public virtual void start() {
			changeState(BaseBattleState.Start);
			on(BaseBattleCallback.BattleStart);
		}

		/// <summary>
		/// 下一回合
		/// </summary>
		public virtual void nextRound() {
			if (round++ > 0) // 如果当前回合大于0，执行回合结束事件
				on(BaseBattleCallback.RoundEnd);
			on(BaseBattleCallback.RoundStart);
		}

		/// <summary>
		/// 战斗结束
		/// </summary>
		public virtual void end() {
			if (round > 0) on(BaseBattleCallback.RoundEnd);
			on(BaseBattleCallback.BattleEnd);
			changeState(BaseBattleState.End);
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		public void update() {
			updateTime();
			updateBattlers();
			updateStateMachine();
			updateOthers();
		}

		/// <summary>
		/// 更新状态机
		/// </summary>
		void updateStateMachine() {
			updateAnyState();
			stateMachine.update();
		}

		/// <summary>
		/// 更新时间
		/// </summary>
		void updateTime() {
			if (isStarted) time += Time.deltaTime;
		}

		/// <summary>
		/// 更新任意状态
		/// </summary>
		protected virtual void updateAnyState() { }

		/// <summary>
		/// 更新其他
		/// </summary>
		protected virtual void updateOthers() { }

		/// <summary>
		/// 更新战斗者
		/// </summary>
		void updateBattlers() {
			foreach (var battler in battlers) battler.update();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		protected virtual void _onBattleStart() {
			time = round = 0;
			foreach (var battler in battlers)
				battler.on(BaseBattlerCallback.BattleStart);
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		protected virtual void _onRoundStart() {
			foreach (var battler in battlers)
				battler.on(BaseBattlerCallback.RoundStart, round);
		}

		/// <summary>
		/// 回合结束回调
		/// </summary>
		protected virtual void _onRoundEnd() {
			foreach (var battler in battlers)
				battler.on(BaseBattlerCallback.RoundEnd, round);
		}

		/// <summary>
		/// 战斗结束回调
		/// </summary>
		protected virtual void _onBattleEnd() {
			foreach (var battler in battlers)
				battler.on(BaseBattlerCallback.BattleEnd);
		}

		#endregion

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		void initialize() {
			initializeStates();
			initializeCallbacks();
			initializeOthers();
		}

		/// <summary>
		/// 初始化状态
		/// </summary>
		protected virtual void initializeCallbacks() {
			cbManager = new CallbackManager(cbType);
		}

		/// <summary>
		/// 初始化状态
		/// </summary>
		protected virtual void initializeStates() {
			stateMachine = new StateMachine(stateType);
		}

		/// <summary>
		/// 初始化其他
		/// </summary>
		protected virtual void initializeOthers() { }

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="actors"></param>
		/// <param name="enemies"></param>
		public void setup(IEnumerable<BaseRuntimeBattler> actors) {
			setup(actors, generateEnemies());
		}
		public void setup(IEnumerable<BaseRuntimeBattler> actors,
			IEnumerable<BaseRuntimeBattler> enemies) {
			foreach (var actor in actors) addActor(actor);
			foreach (var enemy in enemies) addEnemy(enemy);
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseRuntimeBattle() { initialize(); }
		public BaseRuntimeBattle(
			IEnumerable<BaseRuntimeBattler> actors) 
			: this(){ setup(actors); }
		public BaseRuntimeBattle(
			IEnumerable<BaseRuntimeBattler> actors,
			IEnumerable<BaseRuntimeBattler> enemies) 
			: this() { setup(actors, enemies); }
	}

	///// <summary>
	///// 默认战斗管理器 TODO: 待实现
	///// </summary>
	//public class DefaultBattleManager : RuntimeBattleManager<DefaultBattleManager.State> {

	//	/// <summary>
	//	/// 默认战斗状态机（普通回合制战斗）
	//	/// </summary>
	//	public partial class State {
	//		public static string
	//			BattleStart, // 战斗开始
	//			RoundStart, // 回合开始
	//			Inputing, // 行动输入
	//			Acting, // 行动进行
	//			RoundEnd, // 回合结束
	//			BattleEnd // 结算
	//		;
	//	}

	//	/// <summary>
	//	/// 初始化
	//	/// </summary>
	//	protected override void initialize() {
	//		base.initialize();
	//		stateMachine.addStateDict(State.Inputing, updateInputing);
	//	}

	//	#region 更新

	//	/// <summary>
	//	/// 更新
	//	/// </summary>
	//	protected virtual void updateInputing() {

	//	}

	//	#endregion

	//	#region 输入控制



	//	#endregion

	//}
}
