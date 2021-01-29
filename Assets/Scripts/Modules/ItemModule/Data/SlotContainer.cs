
using System;
using System.Collections.Generic;

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
	/// 槽容器接口
	/// </summary>
	public interface ISlotContainer : IBaseContainer {

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <param name="slotItem"></param>
		/// <returns></returns>
		PackContItem getEquip(SlotContItem slotItem, Type eType);
		E getEquip<E>(SlotContItem slotItem);

		/// <summary>
		/// 设置装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		//void setEquip(PackContItem equipItem = null);
		//void setEquip(SlotContItem slotItem, Type type, PackContItem equipItem = null);
		//void setEquip(int slotIndex, Type type, PackContItem equipItem = null);
		void setEquip<E>(E equipItem = null) where E : PackContItem, new();
		void setEquip<E>(SlotContItem slotItem, E equipItem = null) where E : PackContItem, new();
		void setEquip<E>(int slotIndex, E equipItem = null) where E : PackContItem, new();
		void setEquip(Type eType, PackContItem equipItem = null);
		void setEquip(SlotContItem slotItem, Type eType, PackContItem equipItem = null);
		void setEquip(int slotIndex, Type eType, PackContItem equipItem = null);

		/// <summary>
		/// 通过槽索引获取槽
		/// </summary>
		/// <returns>返回对应索引的装备槽项</returns>
		SlotContItem getSlotItem(int slotIndex);

		/// <summary>
		/// 通过索引获取槽
		/// </summary>
		/// <param name="index">下标</param>
		/// <returns>返回对应索引的装备槽项</returns>
		int getSlotIndexByItem(PackContItem item);
	}

	/// <summary>
	/// 背包类容器数据
	/// </summary>
	public abstract class SlotContainer<T> : BaseContainer<T>, 
		ISlotContainer where T : SlotContItem, new() {

		#region 接口实现

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <param name="slotItem"></param>
		/// <returns></returns>
		public PackContItem getEquip(SlotContItem slotItem, Type eType) {
			return getEquip(slotItem as T, eType);
		}
		public E getEquip<E>(SlotContItem slotItem) {
			return getEquip<E>(slotItem as T);
		}

		/// <summary>
		/// 设置装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		public void setEquip(Type eType, PackContItem equipItem = null) {
			setEquip(getSlotItemByEquipItem(equipItem), eType, equipItem);
		}
		public void setEquip(SlotContItem slotItem, Type eType, PackContItem equipItem = null) {
			setEquip(slotItem as T, eType, equipItem);
		}
		public void setEquip(int slotIndex, Type eType, PackContItem equipItem = null) {
			setEquip(getSlotItem(slotIndex), eType, equipItem);
		}
		public void setEquip<E>(E equipItem = null) where E : PackContItem, new() {
			setEquip(getSlotItemByEquipItem(equipItem), equipItem);
		}
		public void setEquip<E>(SlotContItem slotItem, E equipItem = null) where E : PackContItem, new() {
			setEquip(slotItem as T, equipItem);
		}
		public void setEquip<E>(int slotIndex, E equipItem = null) where E : PackContItem, new() {
			setEquip(getSlotItem(slotIndex), equipItem);
		}

		/// <summary>
		/// 通过槽索引获取槽
		/// </summary>
		/// <returns>返回对应索引的装备槽项</returns>
		SlotContItem ISlotContainer.getSlotItem(int slotIndex) {
			return getSlotItem(slotIndex);
		}

		/// <summary>
		/// 通过槽项获取索引值
		/// </summary>
		/// <returns>返回装备槽项的对应索引</returns>
		public virtual int getSlotIndexByItem(PackContItem item) {
			foreach (var slot in items)
				if (slot.getEquip(item.GetType()) == item)
					return items.IndexOf(slot);
			return -1;
		}

		#endregion

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <returns>装备</returns>
		public E getEquip<E>(T slotItem) where E : PackContItem, new() {
			return getEquip(slotItem, typeof(E)) as E; // slotItem.getEquip<E>();
		}
		public PackContItem getEquip(T slotItem, Type eType) {
			if (slotItem.container != this) return null;
			return slotItem.getEquip(eType);
		}

		/// <summary>
		/// 设置装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="container">装备容器</param>
		/// <param name="equipItem">装备物品</param>
		//public void setEquip<E>(PackContainer<E> container, 
		//	E equipItem = null) where E : PackContItem, new() {
		//	setEquip(getSlotItemByEquipItem(equipItem), container, equipItem);
		//}
		//public void setEquip<E>(T slotItem, PackContainer<E> container, 
		//	E equipItem = null) where E : PackContItem, new() {
		//	setEquip(slotItem, equipItem); // 设置新装备
		//}
		//public void setEquip<E>(int slotIndex, PackContainer<E> container, 
		//	E equipItem = null) where E : PackContItem, new() {
		//	setEquip(getSlotItem(slotIndex), container, equipItem);
		//}
		public void setEquip(T slotItem, Type eType, PackContItem equipItem = null) {
			slotItem?.setEquip(eType, equipItem);
		}
		public void setEquip<E>(T slotItem, E equipItem = null) where E : PackContItem, new() {
			setEquip(slotItem, typeof(E), equipItem);
		}

		/// <summary>
		/// 通过槽索引获取槽
		/// </summary>
		/// <returns>返回对应索引的装备槽项</returns>
		public virtual T getSlotItem(int slotIndex) {
			return contItem(item => item.slotIndex == slotIndex);
		}

		/// <summary>
		/// 通过顺序索引获取槽
		/// </summary>
		/// <returns>返回对应索引的装备槽项</returns>
		public virtual T getSlotItemByIndex(int index) {
			return contItem(item => item.index == index);
		}

		/// <summary>
		/// 通过槽项获取索引值
		/// </summary>
		/// <returns>返回装备槽项的对应索引</returns>
		//public virtual int getSlotIndexByItem<E>(E item) where E : PackContItem, new() {
		//	foreach (var slot in items) {
		//		if (slot.getEquip<E>() == item) return items.IndexOf(slot);
		//	}
		//	return -1;
		//}

		/// <summary>
		/// 通过装备物品获取槽
		/// </summary>
		/// <typeparam name="E">装备物品类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		/// <returns>槽ID</returns>
		public abstract T getSlotItemByEquipItem(PackContItem equipItem);

		/// <summary>
		/// 查找一个空的容器项
		/// </summary>
		/// <returns>返回第一个空的容器项</returns>
		public T emptySlotItem() {
			foreach (var item in items)
				if (item.isNullItem) return item;
			return items[0];
		}

	}
}
