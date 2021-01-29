using System;
using System.Collections.Generic;

using AssetModule.Services;

using Core.Services;

namespace ItemModule {

	/// <summary>
	/// 物品配置
	/// </summary>
	public static class ItemConfig {

		/// <summary>
		/// 图标资源
		/// </summary>
		public static readonly AssetSetting Icon =
			new AssetSetting(AssetService.SystemPath, "IconSet");

		/// <summary>
		/// 操作文本设定
		/// </summary>
		public static readonly Interface GetPack = new Interface(
			"获取数据", "item/packcontainer/get");
		public static readonly Interface GetSlot = new Interface(
			"获取数据", "item/slotcontainer/get");

		public static readonly Interface GainItem = new Interface(
			"获得物品", "item/packcontainer/gain");
		public static readonly Interface LostItem = new Interface(
			"失去物品", "item/packcontainer/gain");
		public static readonly Interface LostContItems = new Interface(
			"失去物品", "item/packcontainer/lost_contitems");

		//public static readonly Interface TransferItems = new Interface(
		//	"转移物品", "item/packcontainer/transfer");
		//public static readonly Interface SplitItem = new Interface(
		//	"分解物品", "item/packcontainer/split");
		//public static readonly Interface MergeItems = new Interface(
		//	"组合物品", "item/packcontainer/merge");

		//public static readonly Interface DiscardItem = new Interface(
		//	"丢弃物品", "item/packcontainer/discard");

		//public static readonly Operation UseItem = new Operation(
		//	"使用物品", "item/packcontainer/use");

		//public static readonly Operation SellItem = new Operation(
		//	"出售物品", "item/packcontainer/sell");

		//public static readonly Operation BuyItem = new Operation(
		//	"购买物品", "item/shop/buy");
		//public static readonly Operation GetShop = new Operation(
		//	"获取商品", "item/shop/get");

	}

	/// <summary>
	/// 物品类型
	/// </summary>
	public partial class ItemType {
		public static int
			Unset = 0,  // 未设置

			ExerProItem = 301,  // 特训物品
			ExerProPotion = 302,  // 特训药水
			ExerProCard = 303,  // 特训卡片

			ExerProEnemy = 310,  // 特训敌人
			ExerProStatus = 311  // 特训状态
		;
	}

	/// <summary>
	/// 容器项类型
	/// </summary>
	public partial class ContainerType {
		public static int
			Unset = 0  // 未设置
		;
	}

	/// <summary>
	/// 容器项类型
	/// </summary>
	public partial class ContItemType {
		public static int
			Unset = 0,  // 未设置

			ExerProPackItem = 401, // 特训背包物品
			ExerProPackCard = 402, // 特训背包卡片
			ExerProPackPotion = 403, // 特训背包药水

			ExerProSlotPotion = 404 // 特训槽药水
		;
	}

}
