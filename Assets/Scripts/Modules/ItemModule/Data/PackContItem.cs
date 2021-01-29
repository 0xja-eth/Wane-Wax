
using System;
using System.Collections.Generic;

using UnityEngine;

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
	/// 基本容器项接口
	/// </summary>
	public interface IPackContItem : IBaseContItem {

		/// <summary>
		/// 类型
		/// </summary>
		Type itemType { get; }

		/// <summary>
		/// 属性
		/// </summary>
		int itemId { get; }
		int count { get; }
		bool equiped { get; }

		/// <summary>
		/// 物品
		/// </summary>
		BaseItem baseItem { get; }

		/// <summary>
		/// 容器项容量（-1为无限）
		/// </summary>
		/// <returns></returns>
		int capacity { get; }

		/// <summary>
		/// 剩余数量
		/// </summary>
		int emptyCount { get; }

		/// <summary>
		/// 无限容量
		/// </summary>
		bool isUnlimited { get; }

		/// <summary>
		/// 是否已满
		/// </summary>
		/// <returns></returns>
		bool isFull { get; }

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		bool isEmpty { get; }

		/// <summary>
		/// 移入数量
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>剩余不够移入的数量</returns>
		int enter(int count);

		/// <summary>
		/// 移出数量
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>剩余不够移出的数量</returns>
		int leave(int count);

		/// <summary>
		/// 装备
		/// </summary>
		void doEquip();

		/// <summary>
		/// 卸下
		/// </summary>
		void doDequip();

		/// <summary>
		/// 设置个数
		/// </summary>
		/// <param name="count">个数</param>
		/// <returns></returns>
		void setCount(int count);
	}

	/// <summary>
	/// 背包类容器项
	/// </summary>
	public abstract class PackContItem : BaseContItem, IPackContItem {

		/// <summary>
		/// 物品类型
		/// </summary>
		public abstract Type itemType { get; }

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int itemId { get; set; }
		[AutoConvert]
		public int count { get; protected set; } // 数量，-1为无穷大
		[AutoConvert]
		public bool equiped { get; protected set; } = false;

		/// <summary>
		/// 物品
		/// </summary>
		public abstract BaseItem baseItem { get; }

		/// <summary>
		/// 容器项容量（-1为无限）
		/// </summary>
		/// <returns></returns>
		public virtual int capacity => 1;

		/// <summary>
		/// 剩余数量
		/// </summary>
		public int emptyCount => capacity - count;

		/// <summary>
		/// 无限容量
		/// </summary>
		public bool isUnlimited => capacity == -1;

		/// <summary>
		/// 是否已满
		/// </summary>
		/// <returns></returns>
		public bool isFull => !isUnlimited && count >= capacity;

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		public bool isEmpty => count == 0;

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		public override bool isNullItem => itemId == 0;

		/// <summary>
		/// 移入数量
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>剩余不够移入的数量</returns>
		public int enter(int count) {
			if (this.count == -1) return 0;

			this.count += count;
			if (isUnlimited) return 0; // 如果可以无限叠加

			var res = this.count - capacity;
			if (this.count > capacity)
				this.count = capacity;
			return Math.Max(0, res);
		}

		/// <summary>
		/// 移出数量
		/// </summary>
		/// <param name="count">数量</param>
		/// <returns>剩余不够移出的数量</returns>
		public int leave(int count) {
			if (this.count == -1) return 0;

			this.count -= count;
			var res = -this.count;
			if (this.count < 0) this.count = 0;
			return Math.Max(0, res);
		}

		/// <summary>
		/// 装备
		/// </summary>
		public void doEquip() { equiped = true; }

		/// <summary>
		/// 卸下
		/// </summary>
		public void doDequip() { equiped = false; }

		/// <summary>
		/// 设置个数
		/// </summary>
		/// <param name="count">个数</param>
		/// <returns></returns>
		public void setCount(int count) {
			this.count = 0;
			if (count < 0) this.count = -1;
			else enter(count);
		}
		
	}
	
	/// <summary>
	/// 背包类容器项
	/// </summary>
	public abstract class PackContItem<T> : PackContItem,
		ParamDisplay.IDisplayDataConvertable where T : BaseItem {

		/// <summary>
		/// 物品类型
		/// </summary>
		public static Type ItemType => typeof(T);
		public override Type itemType => typeof(T);

		#region 属性显示数据生成

		/// <summary>
		/// 转化为属性信息集
		/// </summary>
		/// <returns>属性信息集</returns>
		public JsonData convertToDisplayData(string type = "") {
			return item.convertToDisplayData(type);
		}

		#endregion

		/// <summary>
		/// 物品
		/// </summary>
		/// <returns></returns>
		public T item {
			get => DataService.Get().get<T>(itemId);
			set { itemId = value.id; }
		}
		public override BaseItem baseItem => item;

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		public override bool isNullItem => base.isNullItem || item == null;

		/// <summary>
		/// 容器项容量（-1为无限）
		/// </summary>
		/// <returns></returns>
		public override int capacity => item.maxCount;

	}

}
