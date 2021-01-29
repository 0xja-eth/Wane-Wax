
using UnityEngine;

using LitJson;

using Core.Data;

using GameModule.Services;
using AssetModule.Services;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 有限物品数据
	/// </summary>
	public abstract partial class LimitedItem : BaseItem {
		
		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int starId { get; protected set; }
		[AutoConvert]
		public int iconIndex { get; protected set; }
		[AutoConvert("max_count")]
		public int maxCount_ { get; protected set; }
		[AutoConvert]
		public bool discardable { get; protected set; }
		[AutoConvert]
		public bool tradable { get; protected set; }

		/// <summary>
		/// 物品图标
		/// </summary>
		public Sprite icon { get; protected set; }

		#region 数据转换

		/// <summary>
		/// 转化为属性信息集
		/// </summary>
		/// <returns>属性信息集</returns>
		public override JsonData convertToDisplayData(string type = "") {
			var res = base.convertToDisplayData(type);

			res["discardable"] = discardable;
			res["tradable"] = tradable;

			return res;
		}

		#endregion

		/// <summary>
		/// 最大叠加数量
		/// </summary>
		/// <returns></returns>
		public override int maxCount => maxCount_;

		/// <summary>
		/// 获取星级实例
		/// </summary>
		/// <returns></returns>
		public ItemStar star() {
			return DataService.Get().get<ItemStar>(starId);
		}

		/// <summary>
		/// 加载自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);
			icon = loadIcon(); 
		}

		/// <summary>
		/// 读取图标
		/// </summary>
		protected virtual Sprite loadIcon() {
			return AssetService.Get().loadAssetFromGroup<Sprite>(
				ItemConfig.Icon, iconIndex);
		}
	}

}
