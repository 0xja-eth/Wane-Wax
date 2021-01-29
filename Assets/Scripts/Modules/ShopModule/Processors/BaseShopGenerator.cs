using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using GameModule.Services;

using ItemModule.Data;

namespace ShopModule.Processors {

	using Data;

	/// <summary>
	/// 生僻生成器
	/// </summary>
	public abstract class BaseShopGenerator {

		/// <summary>
		/// 商店
		/// </summary>
		public abstract IBaseShop baseShop { get; }

		/// <summary>
		/// 生成
		/// </summary>
		/// <returns></returns>
		public abstract void generate(IBaseShop baseShop);

	}

	/// <summary>
	/// 商品生成器
	/// </summary>
	/// <typeparam name="S"></typeparam>
	public abstract class BaseShopGenerator<S, SI, I> : 
		BaseShopGenerator where S : BaseShop<SI, I>, new()
		where SI : BaseShopItem<I>, new() where I : BaseItem, new() {

		/// <summary>
		/// 数据服务
		/// </summary>
		protected static DataService dataSer => DataService.Get();

		/// <summary>
		/// 商店
		/// </summary>
		public S shop { get; protected set; }

		/// <summary>
		/// 商店（父类实现）
		/// </summary>
		public override IBaseShop baseShop => shop;

		#region 数据

		/// <summary>
		/// 所有物品
		/// </summary>
		protected static List<I> allItems => dataSer.getCollection<I>();

		#endregion

		#region 重载

		/// <summary>
		/// 生成（默认生成策略）
		/// </summary>
		/// <returns></returns>
		public override void generate(IBaseShop baseShop) {
			shop = baseShop as S;
			for (int i = 0; i < shopItemCount; ++i) addItem();
		}

		/// <summary>
		/// 商品数量（由 defaultCapacity 定义）
		/// </summary>
		protected virtual int shopItemCount => shop.capacity;

		/// <summary>
		/// 生成物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual I generateItem() {
			return generateRandomItem(allItems);
		}

		/// <summary>
		/// 生成随机物品
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		protected I generateRandomItem(List<I> items) {
			if (items.Count <= 0) return null;

			int itemCnt = items.Count, cnt = 0;

			I res;
			do {
				var index = Random.Range(0, itemCnt);
				res = items[index]; cnt++;
			} while (!isItemEnable(res) && cnt <= 10000);

			return res;
		}

		/// <summary>
		/// 商品是否有效
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual bool isItemEnable(I item) {
			return item.isBuyable && (!shop.uniqueItem || !shop.hasItem(item));
		}

		/// <summary>
		/// 生成物品数量
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual int generateItemCount(I item) { return -1; }

		/// <summary>
		/// 生成物品价格
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual Money generateItemPrice(I item) { return new Money(); }

		#endregion

		#region 使用

		/// <summary>
		/// 添加商品
		/// </summary>
		/// <param name="item"></param>
		/// <param name="count"></param>
		protected bool addItem() {
			return addItem(generateItem());
		}
		protected bool addItem(I item) {
			return addItem(item, generateItemCount(item));
		}
		protected bool addItem(I item, int count) {
			return addItem(item, generateItemPrice(item), count);
		}
		protected bool addItem(I item, Money price, int count) {
			return shop.pushItem(makeShopItem(item, price, count));
		}

		/// <summary>
		/// 创建商品
		/// </summary>
		/// <param name="item">物品</param>
		/// <param name="count">数量</param>
		/// <returns></returns>
		protected SI makeShopItem(I item, Money price, int count) {
			var res = new SI();
			res.setup(item, price, count);

			return res;
		}

		#endregion

	}
}
