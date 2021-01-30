using System;
using System.Collections.Generic;

using Core.Utils;

namespace BattleModule {

	/// <summary>
	/// 战斗配置
	/// </summary>
	public static class BattleConfig {

		/// <summary>
		/// 属性数量
		/// </summary>
		public const int ParamsCount = 1;

		public const int INF = 999999999; // 小于 1<<31

		/// <summary>
		/// 快捷属性ID
		/// </summary>
		public const int MHPParamId = 0;

		/// <summary>
		/// 默认属性
		/// </summary>
		public static readonly double[] DefaultParams =
			new double[ParamsCount] { 100 };

		/// <summary>
		/// 最大属性
		/// </summary>
		public static readonly double[] MaxParams =
			new double[ParamsCount] { INF };

		/// <summary>
		/// 最小属性
		/// </summary>
		public static readonly double[] MinParams =
			new double[ParamsCount] { 1 };

	}

	/// <summary>
	/// Battler回调类型
	/// </summary>
	public partial class BaseBattlerState : EnumExtend {
		public static string
			NotInBattle, // 非战斗状态

			Idle, // 待机
			Thinking, // 思考中

			Moving, // 移动中
			Acting, // 行动中

			Hitting, // 受击中

			Escaped, // 逃离
			Dead // 死亡
		;
	}

	/// <summary>
	/// Battler回调类型
	/// </summary>
	public partial class BaseBattlerCallback : EnumExtend {
		public static string
			BattleStart,  // 战斗开始
			BattleEnd,  // 战斗结束

			RoundStart,  // 回合开始
			RoundEnd,  // 回合结束

			ActionAdded,  // 行动添加
			ActionStart,  // 行动开始
			ActionEnd,  //行动结束

			Die, // 死亡
			Revive, // 复活
			Escape, // 逃跑

			BuffAdded, // Buff添加
			BuffRemoved, // Buff移除

			StateAdded, // 状态移除
			StateRemoved // 状态移除
		;
	}

	/// <summary>
	/// 战斗状态
	/// </summary>
	public partial class BaseBattleState : EnumExtend {
		public static string
			Start, // 战斗开始
			End // 战斗结束
		;
	}

	/// <summary>
	/// 战斗回调
	/// </summary>
	public partial class BaseBattleCallback : EnumExtend {
		public static string
			BattleStart, // 战斗开始
			RoundStart, // 回合开始
			RoundEnd, // 回合结束
			BattleEnd // 战斗结束
		;
	}

	/// <summary>
	/// 目标类型
	/// </summary>
	public enum TargetType {
		None, // 无
		Self, AllSelf, RandomSelf, // 己方
		Enemy, AllEnemy, RandomEnemy, // 敌方
		Any, All, Random // 全局
	}
}
