using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

namespace BattleModule.Data {

	/// <summary>
	/// 战斗者
	/// </summary>
	public abstract partial class BaseRuntimeBattler : RuntimeData {

		/// <summary>
		/// 行动序列
		/// </summary>
		Queue<RuntimeAction> actions = new Queue<RuntimeAction>();

		#region 逃跑控制

		/// <summary>
		/// 是否逃离
		/// </summary>
		public bool isEscaped => isState(BaseBattlerState.Escaped);

		/// <summary>
		/// 逃跑
		/// </summary>
		public void escape() {
			changeState(BaseBattlerState.Escaped);
			on(BaseBattlerCallback.Escape);
		}

		#endregion

		#region 行动控制

		#region 限制

		/// <summary>
		/// 是否处于可移动状态
		/// </summary>
		/// <returns></returns>
		public virtual bool isMovable() {
			return !isEscaped && !isDead();
		}

		#endregion

		/// <summary>
		/// 增加行动
		/// </summary>
		/// <param name="action">行动</param>
		public RuntimeAction addAction(RuntimeAction action) {
			actions.Enqueue(action);
			on(BaseBattlerCallback.ActionAdded, action);
			return action;
		}
		public RuntimeAction addAction(BaseRuntimeBattler[] objects, EffectData[] effects) {
			return addAction(new RuntimeAction(this, objects, effects));
		}
		public RuntimeAction addAction(BaseRuntimeBattler[] objects, IEffectsProducer producer) {
			return addAction(new RuntimeAction(this, objects, producer));
		}

		/// <summary>
		/// 增加行动
		/// </summary>
		/// <param name="action">行动</param>
		public RuntimeAction startAction(RuntimeAction action) {
			on(BaseBattlerCallback.ActionStart, action);
			return action;
		}

		/// <summary>
		/// 处理指定行动
		/// </summary>
		public virtual void processAction(RuntimeAction action) {
			if (!isMovable()) return;

			action.makeResults();
			action.applyResults();

			on(BaseBattlerCallback.ActionEnd, action);
		}

		/// <summary>
		/// 当前行动
		/// </summary>
		/// <returns></returns>
		public virtual RuntimeAction currentAction() {
			if (actions.Count <= 0) return null;
			var action = actions.Dequeue();
			if (!isMovable()) return null;
			return action;
		}

		/// <summary>
		/// 开始行动
		/// </summary>
		public RuntimeAction startCurrentAction() {
			return startAction(currentAction());
		}
		
		/// <summary>
		/// 处理当前行动（处理后移出队列）
		/// </summary>
		/// <param name="action"></param>
		public virtual void processCurrentAction() {
			processAction(currentAction());
		}

		/// <summary>
		/// 清除所有行动
		/// </summary>
		public void clearActions() {
			actions.Clear();
		}

		#endregion

		#region 结果控制

		/// <summary>
		/// 当前结果
		/// </summary>
		BaseRuntimeResult currentResult = null;

		/// <summary>
		/// 应用结果
		/// </summary>
		/// <param name="result"></param>
		public void setResult(BaseRuntimeResult result) {
			currentResult = result;
		}

		/// <summary>
		/// 获取当前结果（用于显示）
		/// </summary>
		/// <returns></returns>
		public BaseRuntimeResult getResult() {
			var res = currentResult;
			clearResult();
			return res;
		}

		/// <summary>
		/// 清除当前结果
		/// </summary>
		public void clearResult() {
			currentResult = null;
		}

		#endregion

		#region 回调

		/// <summary>
		/// 逃离回调
		/// </summary>
		protected virtual void _onEscape() { }

		/// <summary>
		/// 行动添加回调
		/// </summary>
		/// <param name="action"></param>
		protected virtual void _onActionAdded(RuntimeAction action) {
			Debug.Log(this + " addAction " + action?.toJson().ToJson());
		}

		/// <summary>
		/// 行动开始回调
		/// </summary>
		/// <param name="action"></param>
		protected virtual void _onActionStart(RuntimeAction action) { }

		/// <summary>
		/// 行动结束回调
		/// </summary>
		/// <param name="action"></param>
		protected virtual void _onActionEnd(RuntimeAction action) { }

		#endregion

	}
}
