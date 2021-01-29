using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using ItemModule.Data;

using GameModule.Services;

namespace ShopModule.Data {

	/// <summary>
	/// 商品接口
	/// </summary>
	public interface IBaseShopItem : IPackContItem {

		/// <summary>
		/// 价格
		/// </summary>
		Money price { get; }

		/// <summary>
		/// 对应的商店
		/// </summary>
		IBaseShop shop { get; set; }

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="price"></param>
		/// <param name="count"></param>
		void setup(int itemId, Money price, int count);
		void setup(Money price, int count);

		/// <summary>
		/// 购买
		/// </summary>
		void buy();
		void buy(int count);

	}

	/// <summary>
	/// 商品
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseShopItem<T> : PackContItem<T>, 
		IBaseShopItem where T : BaseItem, new() {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public Money price { get; protected set; }

		/// <summary>
		/// 对应的商店
		/// </summary>
		public IBaseShop shop {
			get => container as IBaseShop;
			set { container = value as BaseContainer; }
		}

		/// <summary>
		/// 容器项容量（-1为无限）
		/// </summary>
		/// <returns></returns>
		public override int capacity => -1;

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="item">物品</param>
		/// <param name="price">价格</param>
		/// <param name="count">数量</param>
		public void setup(T item, Money price, int count) {
			setup(item.id, price, count);
		}
		public void setup(int itemId, Money price, int count) {
			this.itemId = itemId;
			setup(price, count);
		}
		public void setup(Money price, int count) {
			this.price = price;
			this.count = count;
		}

		/// <summary>
		/// 购买
		/// </summary>
		public void buy() { buy(count); }
		/// <param name="count">购买数量</param>
		public virtual void buy(int count) {
			leave(count);
		}

	}
}
