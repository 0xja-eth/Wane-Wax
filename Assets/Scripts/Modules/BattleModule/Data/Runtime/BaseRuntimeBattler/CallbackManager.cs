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
		/// 回调管理器
		/// </summary>
		protected CallbackManager cbManager;

		#region 回调管理

		/// <summary>
		/// 状态类
		/// </summary>
		public virtual Type cbType => typeof(BaseBattlerCallback);

		/// <summary>
		/// 初始化回调
		/// </summary>
		protected virtual void initializeCallbacks() {
			cbManager = new CallbackManager(cbType, this);
		}

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

		#region 回调控制

		/// <summary>
		/// 战斗开始回调
		/// </summary>
		protected virtual void _onBattleStart() {
			_onRoundStart(0);
			clearBuffs();
		}

		/// <summary>
		/// 回合开始回调
		/// </summary>
		/// <param name="round">回合数</param>
		protected virtual void _onRoundStart(int round) {
			_deltaHP = null;
			_addedBuffs.Clear();
			clearActions();
		}

		/// <summary>
		/// 回合结束回调
		/// </summary>
		/// <param name="round">回合数</param>
		protected virtual void _onRoundEnd(int round) {
			clearActions();
			processBuffsRoundEnd();
		}

		/// <summary>
		/// 战斗结束回调
		/// </summary>
		protected virtual void _onBattleEnd() {
			changeState(BaseBattlerState.NotInBattle);
		}

		/// <summary>
		/// 拓展
		/// </summary>
		partial void processBuffsRoundEnd();

		#endregion

	}
}
