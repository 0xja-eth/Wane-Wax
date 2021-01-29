
using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;

using GameModule.Data;
using AssetModule.Services;

using ItemModule;
using ItemModule.Data;

/// <summary>
/// 物品模块数据
/// </summary>
namespace BattleModule.Data {

	/// <summary>
	/// 可用物品数据
	/// </summary>
	public abstract partial class BaseState : BaseItem, ITraitsObject {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int iconIndex { get; protected set; }
		[AutoConvert]
		public int maxTurns { get; protected set; } // 最大状态叠加回合数
		[AutoConvert]
		public bool isNega { get; protected set; } // 是否负面

		/// <summary>
		/// 持有特性
		/// </summary>
		public abstract TraitData[] baseTraits { get; }

		/// <summary>
		/// 状态图标
		/// </summary>
		public Sprite icon { get; protected set; }

		/// <summary>
		/// 加载自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
			icon = AssetService.Get().loadAssetFromGroup
				<Sprite>(ItemConfig.Icon, iconIndex);
		}
	}

	/// <summary>
	/// 可用物品数据
	/// </summary>
	public abstract partial class BaseState<T> : BaseState, 
		ITraitsObject<T> where T : TraitData {

		/// <summary>
		/// 持有特性
		/// </summary>
		[AutoConvert]
		public T[] traits { get; protected set; }

		/// <summary>
		/// 持有特性
		/// </summary>
		public override TraitData[] baseTraits => traits as TraitData[];
	}

}
