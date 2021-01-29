using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using Core.Utils;

namespace BattleModule.Data {

	/// <summary>
	/// 战斗者
	/// </summary>
	public partial class BaseRuntimeBattler : RuntimeData {

		/// <summary>
		/// 状态机
		/// </summary>
		public StateMachine stateMachine;

		/// <summary>
		/// 状态
		/// </summary>
		[AutoConvert]
		public string state {
			get => stateMachine.state;
			set { changeState(value, true); }
		}

		#region 状态控制

		/// <summary>
		/// 状态类
		/// </summary>
		public virtual Type stateType => typeof(BaseBattlerState);

		/// <summary>
		/// 初始化状态
		/// </summary>
		protected virtual void initializeStates() {
			stateMachine = new StateMachine(stateType, this);
		}

		/// <summary>
		/// 是否处于状态
		/// </summary>
		/// <param name="state">状态名</param>
		/// <returns>是否存在</returns>
		public bool isState(string state) {
			return stateMachine?.isState(state) ?? false;
		}
		public bool isState(Enum state) {
			return stateMachine?.isState(state) ?? false;
		}

		/// <summary>
		/// 状态是否改变
		/// </summary>
		/// <returns>状态改变</returns>
		public bool isStateChanged() {
			return stateMachine?.isStateChanged() ?? false;
		}

		/// <summary>
		/// 改变状态
		/// </summary>
		/// <param name="state">新状态</param>
		public void changeState(string state, bool force = false) {
			stateMachine?.changeState(state, force, this);
		}
		public void changeState(Enum state, bool force = false) {
			stateMachine?.changeState(state, force, this);
		}

		/// <summary>
		/// 战斗中
		/// </summary>
		public virtual bool inBattle => true;

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新状态机
		/// </summary>
		void updateStateMachine() {
			stateMachine.update();
			updateAnyState();
		}

		/// <summary>
		/// 任意状态更新
		/// </summary>
		protected virtual void updateAnyState() {
			if (!inBattle) changeState(BaseBattlerState.NotInBattle);
		}

		/// <summary>
		/// 更新非战斗状态
		/// </summary>
		protected virtual void _updateNotInBattle() {
			if (inBattle) changeState(BaseBattlerState.Idle);
		}

		/// <summary>
		/// 更新待命
		/// </summary>
		protected virtual void _updateIdle() { }

		/// <summary>
		/// 更新思考
		/// </summary>
		protected virtual void _updateThinking() { }

		/// <summary>
		/// 更新移动
		/// </summary>
		protected virtual void _updateMoving() { }

		/// <summary>
		/// 更新行动
		/// </summary>
		protected virtual void _updateActing() { }

		/// <summary>
		/// 更新受击
		/// </summary>
		protected virtual void _updateHitting() { }

		#endregion

	}
}
