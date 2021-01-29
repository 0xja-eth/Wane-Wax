
using System;
using System.Collections.Generic;

using LitJson;

using Core.Data;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;

/// <summary>
/// 物品模块数据
/// </summary>
namespace BattleModule.Data {

	/// <summary>
	/// 可用物品数据
	/// </summary>
	public abstract partial class UsableItem : LimitedItem, IEffectsProducer {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public bool consumable { get; protected set; }
		[AutoConvert]
		public int target { get; protected set; }

		/// <summary>
		/// 使用效果
		/// </summary>
		public abstract EffectData[] baseEffects { get; }

		/// <summary>
		/// 目标类型文本
		/// </summary>
		/// <returns>目标类型文本</returns>
		public TargetType targetEnum() {
			return (TargetType)target;
		}

		/// <summary>
		/// 目标类型文本
		/// </summary>
		/// <returns>目标类型文本</returns>
		public string targetText() {
			return GameConfigure.Get.targetTypes[target];
		}

	}

	/// <summary>
	/// 可用物品数据
	/// </summary>
	public abstract partial class UsableItem<E> : UsableItem, 
		IEffectsProducer<E> where E : EffectData {

		/// <summary>
		/// 使用效果
		/// </summary>
		[AutoConvert]
		public E[] effects { get; protected set; }

		/// <summary>
		/// 使用效果
		/// </summary>
		public override EffectData[] baseEffects => effects as EffectData[];
	}

}
