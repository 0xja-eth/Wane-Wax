using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;

using ItemModule.Data;

using GameModule.Services;

namespace ShopModule.Data {

	/// <summary>
	/// 商店接口
	/// </summary>
	public interface IBaseShop : IPackContainer {

		/// <summary>
		/// 商品列表
		/// </summary>
		List<IBaseShopItem> shopItems { get; }

		/// <summary>
		/// 商品不重复
		/// </summary>
		bool uniqueItem { get; }

		/// <summary>
		/// 添加商品
		/// </summary>
		/// <param name="item"></param>
		bool addShopItem(IBaseShopItem item);
	}

	/// <summary>
	/// 商品
	/// </summary>
	/// <typeparam name="I"></typeparam>
	public abstract class BaseShop<SI, I> : PackContainer<SI, I>, IBaseShop
		where SI : BaseShopItem<I>, new() where I: BaseItem, new() {

		/// <summary>
		/// 商品列表
		/// </summary>
		public List<IBaseShopItem> shopItems {
			get {
				var res = new List<IBaseShopItem>();
				foreach (var item in items) res.Add(item);
				return res;
			}
		}

		/// <summary>
		/// 商品不重复
		/// </summary>
		public virtual bool uniqueItem => true;

		/// <summary>
		/// 添加商品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool addShopItem(IBaseShopItem item) {
			return pushItem(item as SI);
		}
	}
}
