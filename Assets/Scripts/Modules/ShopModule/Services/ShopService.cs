using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

using Core.Services;
using Core.Utils;

using ItemModule.Data;

namespace ShopModule.Services {

	using Data;
	using Processors;

	/// <summary>
	/// 商店服务
	/// </summary>
	public partial class ShopService : BaseService<ShopService> {

		/// <summary>
		/// 元素类型字典
		/// </summary>
		Dictionary<int, Type> shopTypes = new Dictionary<int, Type>();
		Dictionary<int, Type> shopItemTypes = new Dictionary<int, Type>();

		/// <summary>
		/// 处理器映射表
		/// </summary>
		Dictionary<Type, Type> shopGenerators = new Dictionary<Type, Type>();

		/// <summary>
		/// 回调函数
		/// </summary>
		public IBaseShop currentShop { get; protected set; }

		#region 模块配置

		// TODO: 加入联网
		/// <summary>
		/// 联网
		/// </summary>
		public override bool networkEnable => false;

		#endregion

		#region 初始化

		/// <summary>
		/// 初始化商店系统
		/// </summary>
		protected override void initializeElements() {
			setupSystemElement<IBaseShop, ShopType>(shopTypes);
			setupSystemElement<IBaseShopItem, ShopItemType>(shopItemTypes);
		}

		/// <summary>
		/// 初始化处理器类型
		/// </summary>
		protected override void initializeProcessors() {
			setupProcessors<BaseShopGenerator>(shopGenerators);
		}

		#endregion

		#region 元素&控制器

		/// <summary>
		/// 效果处理器类
		/// </summary>
		/// <typeparam name="S"></typeparam>
		/// <returns></returns>
		public Type shopGeneratorType<S>() where S : IBaseShop {
			return shopGeneratorType(typeof(S));
		}
		public Type shopGeneratorType(Type sType) {
			return processorType(sType, shopGenerators);
		}

		#endregion

		#region 商店操作

		/// <summary>
		/// 是否处于商店状态
		/// </summary>
		public bool isShop => currentShop != null;

		/// <summary>
		/// 进入商店
		/// </summary>
		/// <param name="shop"></param>
		public void startShop(IBaseShop shop) {
			currentShop = shop;
		}

		/// <summary>
		/// 离开商店
		/// </summary>
		/// <param name="shop"></param>
		public void terminateShop(IBaseShop shop) {
			currentShop = null;
		}
		
		///// <summary>
		///// 生成商店
		///// </summary>
		///// <typeparam name="S">商品类型</typeparam>
		//public void generateShop<S>() where S : IBaseShop {
		//	if (isConnected) return; 
		//	else generateShopLocal<S>();
		//}

		///// <summary>
		///// 生成商店
		///// </summary>
		///// <typeparam name="SI">商品项类型</typeparam>
		///// <param name="shopItem">商品项</param>
		///// <param name="count">数量</param>
		//public void buyItem(IBaseShopItem shopItem, int count) {
		//	if (!isItemBuyable(shopItem, count)) return; // 异常处理

		//	if (isConnected) return;
		//	else buyItemLocal(shopItem, count);
		//}

		/// <summary>
		/// 能否购买
		/// </summary>
		/// <param name="shopItem">商品项</param>
		/// <param name="count">数量</param>
		/// <returns>能否购买</returns>
		public bool isItemBuyable(IBaseShopItem shopItem, int count) {
			if (shopItem.shop != currentShop) return false;

			var money = player?.money;
			if (money == null) return false;

			return money >= shopItem.price * count;
		}

		#endregion

		#region 联网操作

		///// <summary>
		///// 服务器生成商店
		///// </summary>
		//void generateShopServer<S>() where S : IShop {

		//}

		#endregion

		#region 本地操作

		///// <summary>
		///// 本地生成商店
		///// </summary>
		///// <typeparam name="S"></typeparam>
		//void generateShopLocal<S>() where S : IBaseShop {
		//	var gType = shopGeneratorType<S>();
		//	var generator = Activator.CreateInstance(gType) as BaseShopGenerator;

		//	if (generator != null) {
		//		generator?.generate();
		//		onShopGenerateSuccess.Invoke(generator.baseShop);
		//	}
		//}

		///// <summary>
		///// 生成商店
		///// </summary>
		//void buyItemLocal(IBaseShopItem shopItem, int count) {
		//	player?.lostMoney(shopItem.price * count);
		//	player?.gainItem(shopItem.baseItem, count);

		//	shopItem.buy(count);
		//}

		#endregion
	}
}
