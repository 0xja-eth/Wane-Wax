using System;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core;

using Core.Data;
using Core.Data.Loaders;

using ItemModule.Data;
using ItemModule.Services;

/// <summary>
/// 基本系统
/// </summary>
namespace ShopModule.Services {

	using Data;
	using Processors;

    /// <summary>
    /// 商店服务类
    /// </summary>
    public partial class ShopService {

		/// <summary>
		/// 生成商品
		/// </summary>
		public class GenerateShop : Operation {

			/// <summary>
			/// 结果
			/// </summary>
			IBaseShop shop;

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="shop"></param>
			/// <param name="onSuccess"></param>
			/// <param name="onError"></param>
			public void invoke(IBaseShop shop, 
				UnityAction onSuccess = null, UnityAction onError = null) {
				this.shop = shop; invoke(onSuccess, onError);
			}

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() { return shop != null; }

			/// <summary>
			/// 处理请求成功
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				base.processSuccess(data);
				DataLoader.load(shop.GetType(), shop, data, "shop");
			}

			/// <summary>
			/// 处理本地
			/// </summary>
			protected override void processLocal() {
				var sType = shop.GetType();
				var gType = service.shopGeneratorType(sType);
				var generator = Activator.CreateInstance(gType) as BaseShopGenerator;

				generator?.generate(shop);
			}
		}
		public GenerateShop generateShop => new GenerateShop();

		/// <summary>
		/// 生成商品
		/// </summary>
		public class BuyItem : ItemService.GetPack {

			/// <summary>
			/// 结果
			/// </summary>
			[AutoConvert]
			public int index { get; set; }
			[AutoConvert]
			public int count { get; set; }

			/// <summary>
			/// 商店服务
			/// </summary>
			ShopService shopSer => Get();

			/// <summary>
			/// 商品项
			/// </summary>
			IBaseShopItem shopItem;

			/// <summary>
			/// 执行
			/// </summary>
			public void invoke(IBaseShopItem shopItem, int count,
				UnityAction onSuccess = null, UnityAction onError = null) {
				this.shopItem = shopItem; this.count = count;
				invoke(shopItem.shop, onSuccess, onError);
			}

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() {
				return base.isValid() && shopItem != null &&
					shopSer.isItemBuyable(shopItem, count);
			}

			/// <summary>
			/// 配置请求参数
			/// </summary>
			protected override void setupRequestParams() {
				base.setupRequestParams();
				index = shopItem.index;
			}

			/// <summary>
			/// 处理本地
			/// </summary>
			protected override void processLocal() {
				shopSer.player?.lostMoney(shopItem.price * count);
				shopSer.player?.gainItem(shopItem.baseItem, count);

				shopItem.buy(count);
			}
		}
		BuyItem buyItem => new BuyItem();
		//public void buyItem<SI, I>(SI shopItem, int count,
		//	UnityAction onSuccess = null, UnityAction onError = null)
		//	where SI : BaseShopItem<I>, new() where I : BaseItem, new() {
		//	new BuyItem<SI, I>().invoke(shopItem, count, onSuccess, onError);
		//}

	}

}