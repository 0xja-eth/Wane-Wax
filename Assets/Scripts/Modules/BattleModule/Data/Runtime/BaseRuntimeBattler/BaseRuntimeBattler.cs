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
	public abstract partial class BaseRuntimeBattler : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int order { get; set; } // TODO: 序号

		/// <summary>
		/// 战斗图
		/// </summary>
		public virtual Sprite picture => null; 

		/// <summary>
		/// 是否玩家
		/// </summary>
		/// <returns></returns>
		public virtual bool isActor => false;

		/// <summary>
		/// 是否敌人
		/// </summary>
		/// <returns></returns>
		public virtual bool isEnemy => false;

		#region 数据控制

		/// <summary>
		/// 重置
		/// </summary>
		public virtual void reset() {
			hp = mhp; resetBuffs();
		}

		/// <summary>
		/// 重置其他
		/// </summary>
		partial void resetBuffs();

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		public void update() {
			updateStateMachine();
			updateOthers();
		}

		/// <summary>
		/// 更新其他
		/// </summary>
		protected virtual void updateOthers() { }

		#endregion

		#region 初始化控制

		/// <summary>
		/// 初始化
		/// </summary>
		void initialize() {
			initializeCallbacks();
			initializeStates();
			initializeParams();
			initializeOthers();
		}

		/// <summary>
		/// 初始化其他
		/// </summary>
		protected virtual void initializeOthers() { }

		/// <summary>
		/// 初始化其他
		/// </summary>
		partial void initializeParams();

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseRuntimeBattler() {
			initialize();
		}

	}
}
