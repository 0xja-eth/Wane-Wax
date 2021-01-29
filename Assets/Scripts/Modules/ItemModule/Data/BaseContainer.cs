
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ExerComps.Controls.ParamDisplays;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 基本容器数据
	/// </summary>
	public interface IBaseContainer : IBaseData {

		/// <summary>
		/// 物品类型
		/// </summary>
		Type contItemType { get; }

		/// <summary>
		/// 属性
		/// </summary>
		int type { get; }
		int capacity { get; }

		#region 数据操作

		/// <summary>
		/// 容器项数量
		/// </summary>
		int count { get; }

		/// <summary>
		/// 默认容量（为-1则不限）
		/// </summary>
		/// <returns></returns>
		int defaultCapacity { get; }

		/// <summary>
		/// 剩余空格数
		/// </summary>
		int emptyCount { get; }

		/// <summary>
		/// 无限容量
		/// </summary>
		bool isUnlimited { get; }

		/// <summary>
		/// 容器项已满
		/// </summary>
		/// <returns></returns>
		bool isFull { get; }

		/// <summary>
		/// 是否为空
		/// </summary>
		bool isEmpty { get; }

		#endregion

	}

	/// <summary>
	/// 基本容器数据
	/// </summary>
	public abstract class BaseContainer : BaseData, IBaseContainer {

		/// <summary>
		/// 物品类型
		/// </summary>
		public abstract Type contItemType { get; }

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int type { get; protected set; }
		[AutoConvert]
		public int capacity { get; protected set; }

		///// <summary>
		///// 关联类型
		///// </summary>
		//public abstract Type contItemType { get; }
		//public abstract Type itemType { get; }

		#region 数据操作

		/// <summary>
		/// 容器项数量
		/// </summary>
		public abstract int count { get; }

		/// <summary>
		/// 默认容量（为-1则不限）
		/// </summary>
		/// <returns></returns>
		public virtual int defaultCapacity => -1;

		/// <summary>
		/// 剩余空格数
		/// </summary>
		public int emptyCount => capacity - count;

		/// <summary>
		/// 无限容量
		/// </summary>
		public bool isUnlimited => capacity == -1;

		/// <summary>
		/// 容器项已满
		/// </summary>
		/// <returns></returns>
		public bool isFull => !isUnlimited && count >= capacity;

		/// <summary>
		/// 是否为空
		/// </summary>
		public bool isEmpty => count <= 0;

		#endregion
		
		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseContainer() { capacity = defaultCapacity; }

	}

	/// <summary>
	/// 基本容器数据
	/// </summary>
	public abstract class BaseContainer<T> : BaseContainer where T : BaseContItem, new() {

		/// <summary>
		/// 容器项类型 -> -> BaseContItem
		/// </summary>
		public static Type ContItemType => typeof(T);
		public override Type contItemType => typeof(T);

		/// <summary>
		/// 属性
		/// </summary>
		//[AutoConvert]
		public List<T> items { get; protected set; } = new List<T>();

		/// <summary>
		/// 容器项数量
		/// </summary>
		public override int count => items.Count;

		#region 数据查询

		/// <summary>
		/// 获得一个物品
		/// </summary>
		/// <param name="p">条件</param>
		/// <returns>物品</returns>
		public T contItem(Predicate<T> p) {
			return items.Find(p);
		}
		public T contItem(int index) {
			return contItem(item => item.index == index);
		}

		/// <summary>
		/// 获得多个物品
		/// </summary>
		/// <param name="p">条件</param>
		/// <returns>物品列表</returns>
		public List<T> contItems(Predicate<T> p) {
			return items.FindAll(p);
		}

		/// <summary>
		/// 条件计数
		/// </summary>
		/// <param name="p">条件</param>
		/// <returns>物品列表</returns>
		public int contItemCount(Predicate<T> p) {
			return contItems(p).Count;
		}

		/// <summary>
		/// 是否包含某容器项
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool containItem(T item) {
			return items.Contains(item);
		}

		/// <summary>
		/// 获取随机物品
		/// </summary>
		/// <returns>物品容器项</returns>
		public T randomContItem() {
			var cnt = items.Count;
			if (cnt <= 0) return null;
			var index = Random.Range(0, cnt);
			return items[index];
		}

		#endregion

		#region 数据读取

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			var data = DataLoader.load(json, "items");

			if (data != null && data.IsArray) {
				items.Clear();
				for (int i = 0; i < data.Count; ++i) {
					var item = loadItem(data[i]);
					if (item != null) items.Add(item);
				}
			}
		}

		/// <summary>
		/// 读取单个物品
		/// </summary>
		/// <param name="json">数据</param>
		protected virtual T loadItem(JsonData json) {
			var res = DataLoader.load<T>(json);
			res.container = this;
			return res;
		}

		/// <summary>
		/// 转化自定义数据
		/// </summary>
		/// <param name="json">数据</param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);
			json["items"] = DataLoader.convert(items);
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseContainer() { }

	}

}
