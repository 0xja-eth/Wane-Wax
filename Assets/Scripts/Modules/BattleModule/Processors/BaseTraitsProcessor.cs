
using System;
using System.Collections.Generic;

using LitJson;

using Core.Data;

using Core.Utils;

/// <summary>
/// 物品模块数据
/// </summary>
namespace BattleModule.Processors {

	using Data;

	/// <summary>
	/// 使用效果数据
	/// </summary>
	public abstract class BaseTraitsProcessor { }

	/// <summary>
	/// 使用效果数据
	/// </summary>
	public abstract class BaseTraitsProcessor<T> :
		BaseTraitsProcessor where T : TraitData {

		/// <summary>
		/// 目标
		/// </summary>
		protected BaseRuntimeBattler<T> battler;

		/// <summary>
		/// 特性数组
		/// </summary>
		protected List<T> traits => battler.traits();

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="battler"></param>
		public BaseTraitsProcessor(BaseRuntimeBattler<T> battler) {
			this.battler = battler;
		}

	}

}
