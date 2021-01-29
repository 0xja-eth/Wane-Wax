
using System;
using System.Collections.Generic;

using LitJson;

using Core.Utils;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 背包类容器接口
	/// </summary>
	public interface IPackContainer : IBaseContainer {

		/// <summary>
		/// 关联类型
		/// </summary>
		Type itemType { get; }

		/// <summary>
		/// 添加/移除容器项
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		bool pushItem(PackContItem contItem);
		PackContItem removeItem(PackContItem contItem);

		/// <summary>
		/// 获得物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		bool gainItem(BaseItem item, int count);
		bool lostItem(BaseItem item, int count);

		/// <summary>
		/// 能否获得/失去物品
		/// </summary>
		/// <param name="item"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		bool isItemGainEnable(BaseItem item, int count);
		bool isItemLostEnable(BaseItem item, int count);
	}

	/// <summary>
	/// 背包类容器数据
	/// </summary>
	public abstract class PackContainer<T> : BaseContainer<T>, 
		IPackContainer where T : PackContItem, new() {

		#region 接口实现

		/// <summary>
		/// 物品类型
		/// </summary>
		public abstract Type itemType { get; }

		/// <summary>
		/// 获得/失去物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract bool gainItem(BaseItem item, int count);
		public abstract bool lostItem(BaseItem item, int count);

		/// <summary>
		/// 能否获得/失去物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public abstract bool isItemGainEnable(BaseItem item, int count);
		public abstract bool isItemLostEnable(BaseItem item, int count);

		/// <summary>
		/// 添加/移除容器项
		/// </summary>
		/// <param name="contItem"></param>
		/// <returns></returns>
		public bool pushItem(PackContItem contItem) {
			return pushItem(contItem as T);
		}
		public PackContItem removeItem(PackContItem contItem) {
			return removeItem(contItem as T);
		}

		#endregion

		#region 数据操作

		/// <summary>
		/// 添加物品
		/// </summary>
		/// <param name="item">物品</param>
		public bool pushItem(T item) {
			if (isFull) return false;
			if (item == null) return false;
			if (containItem(item)) return false;
			items.Add(item); return true;
		}

		/// <summary>
		/// 移除物品
		/// </summary>
		/// <param name="item">物品</param>
		public T removeItem(T item) {
			if (item == null) return null;
			if (!containItem(item)) return null;
			items.Remove(item);
			return item;
		}

		/*
		/// <summary>
		/// 拆分物品
		/// </summary>
		/// <param name="item">物品</param>
		/// <param name="count">数目</param>
		public void splitItem(T item, int count) {
			if (count >= item.count) return;
			var copyItem = (T)item.copy(false);
			copyItem.setCount(count);
			pushItem(copyItem);
			item.leave(count);
		}

		/// <summary>
		/// 合并物品
		/// </summary>
		/// <param name="items">物品数组</param>
		public void mergeItems(T[] items) {
			var leftIndex = 0;
			var leftItem = items[leftIndex];
			for (int i = 1; i < items.Length; i++) {
				var item = items[i];
				item.setCount(leftItem.enter(item.count));
				if (leftItem.isFull()) // 如果最左物品已满，切换之
					leftItem = items[++leftIndex];
			}
			for (int i = items.Length - 1; i > leftIndex; --i)
				removeItem(items[i]);
		}
		*/

		/// <summary>
		/// 转移物品
		/// </summary>
		/// <param name="container"></param>
		public bool transferItem(PackContainer<T> container, T item) {
			return container.acceptItem(prepareItem(item));
		}

		/// <summary>
		/// 准备物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual T prepareItem(T item) {
			return removeItem(item);
		}

		/// <summary>
		/// 接受物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual bool acceptItem(T item) {
			return pushItem(item);
		}

		#endregion
	}
	
	/// <summary>
	/// 背包类容器数据
	/// </summary>
	public abstract class PackContainer<CI, I> : PackContainer<CI>
		where CI : PackContItem<I>, new() where I : BaseItem, new() {

		/// <summary>
		/// 物品类型
		/// </summary>
		public static Type ItemType => typeof(I);
		public override Type itemType => typeof(I);

		#region 接口实现

		/// <summary>
		/// 获取/失去物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool gainItem(BaseItem item, int count) {
			return gainItem(item as I, count);
		}
		public override bool lostItem(BaseItem item, int count) {
			return lostItem(item as I, count);
		}

		/// <summary>
		/// 能否获得/失去物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isItemGainEnable(BaseItem item, int count) {
			return isItemGainEnable(item as I, count);
		}
		public override bool isItemLostEnable(BaseItem item, int count) {
			return isItemLostEnable(item as I, count);
		}

		#endregion

		#region 数据查询

		/// <summary>
		/// 获取指定物品的容器项（第一个）
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public CI contItem(I item) {
			return contItem(ci => ci.itemId == item.id);
		}

		/// <summary>
		/// 获取指定物品的容器项列表
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public List<CI> contItems(I item) {
			return contItems(contItem => contItem.itemId == item.id);
		}

		/// <summary>
		/// 物品数量
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int itemCount(I item) {
			var res = 0;
			foreach (var contItem in contItems(item))
				if (contItem.count >= 0) res += contItem.count;
				else return -1;
			return res;
		}

		/// <summary>
		/// 物品数量
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool hasItem(I item) {
			var cnt = itemCount(item);
			return cnt == -1 || cnt > 0;
		}

		#endregion

		#region 数据判断

		/// <summary>
		/// 能否获得物品
		/// </summary>
		/// <param name="item"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public virtual bool isItemGainEnable(I item, int count) {
			if (item == null) return false;
			if (count < 0) return false;

			if (isUnlimited) return true;

			var tmpContItem = createContItem(item);
			var contItemCap = tmpContItem.capacity;

			if (contItemCap == 0) return true; // 无限

			// 减去剩余格子可容纳的最大数量
			count -= emptyCount * contItemCap;

			if (count <= 0) return true; // 没有剩余

			// 遍历每一个容器项，减去剩余数量
			foreach (var contItem in contItems(item))
				// 无限容量 || 扣除完毕
				if (contItem.capacity == 0 ||
					(count -= contItem.emptyCount) < 0)
					return true;

			return count <= 0;
		}

		/// <summary>
		/// 能否获得物品
		/// </summary>
		/// <param name="item"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public virtual bool isItemLostEnable(I item, int count) {
			if (item == null) return false;

			return count <= itemCount(item);
		}

		#endregion

		#region 数据操作

		/// <summary>
		/// 获得物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool gainItem(I item, int count, bool force = false) {
			if (!force && !isItemGainEnable(item, count)) return false;

			// 从已有的容器项中增加
			foreach (var contItem in contItems(item))
				if ((count = contItem.enter(count)) <= 0) break;

			// 还有需要获得的数量
			while (count > 0)
				pushItem(createContItem(item, ref count));

			return true;
		}

		/// <summary>
		/// 失去物品
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool lostItem(I item, int count, bool force = false) {
			if (!force && !isItemLostEnable(item, count)) return false;

			// 从已有的容器项中移除
			var contItems = this.contItems(item);
			foreach (var contItem in contItems) {
				count = contItem.leave(count);

				if (contItem.isEmpty) removeItem(contItem);

				if (count <= 0) break;
			}

			return true;
		}

		/// <summary>
		/// 创建容器项
		/// </summary>
		/// <param name="item"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		CI createContItem(I item) {
			var res = new CI();
			res.item = item;
			return res;
		}
		CI createContItem(I item, ref int count) {
			var res = createContItem(item);
			count = res.enter(count);
			return res;
		}

		// TODO: 更多逻辑之后实现

		#endregion

	}
}
