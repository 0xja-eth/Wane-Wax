
using System;
using System.Collections.Generic;

using LitJson;

using Core.Utils;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 基本容器项接口
	/// </summary>
	public interface ISlotContItem : IBaseContItem {

		/// <summary>
		/// 容器项类型 -> PackContItem
		/// </summary>
		Type equipItem1Type { get; }
		Type equipItem2Type { get; }

		/// <summary>
		/// 属性
		/// </summary>
		int slotIndex { get; }

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <returns>装备</returns>
		E getEquip<E>() where E : PackContItem, new();
		PackContItem getEquip(Type eType);

		/// <summary>
		/// 设置装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		void setEquip<E>(E equipItem) where E : PackContItem, new();
		void setEquip(Type type, PackContItem equipItem = null);

	}

	/// <summary>
	/// 槽类容器项
	/// </summary>
	public abstract class SlotContItem : BaseContItem, ISlotContItem {

		/// <summary>
		/// 容器项类型 -> PackContItem
		/// </summary>
		public virtual Type equipItem1Type => null;
		public virtual Type equipItem2Type => null;

		/// <summary>
		/// 属性
		/// </summary>
		//[AutoConvert]
		//public int index { get; protected set; }

		/// <summary>
		/// 槽索引
		/// </summary>
		public virtual int slotIndex {
			get { return index; }
			set { index = value; }
		}

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <returns>装备</returns>
		public E getEquip<E>() where E : PackContItem, new() {
			return getEquip(typeof(E)) as E;
		}
		public virtual PackContItem getEquip(Type eType) { return null; }

		/// <summary>
		/// 设置装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		public void setEquip<E>(E equipItem) where E : PackContItem, new() {
			setEquip(typeof(E), equipItem as PackContItem);
		}
		public virtual void setEquip(Type type, PackContItem equipItem = null) { }

		/// <summary>
		/// 构造函数
		/// </summary>
		public SlotContItem() : base() { }
	}

	/// <summary>
	/// 槽类容器项（单装备）
	/// </summary>
	public abstract class SlotContItem<T> : SlotContItem where T : PackContItem, new() {

		/// <summary>
		/// 容器项类型 -> PackContItem
		/// </summary>
		public static Type EquipItem1Type => typeof(T);
		public override Type equipItem1Type => typeof(T);

		/// <summary>
		/// 装备
		/// </summary>
		public abstract T equip1 { get; protected set; }

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		public override bool isNullItem => equip1 == null || equip1.isNullItem;

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <returns>装备</returns>
		public override PackContItem getEquip(Type eType) {
			var tt = typeof(T);
			if (eType == tt || tt.IsSubclassOf(eType)) return equip1;
			return null;
		}

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		public override void setEquip(Type eType, PackContItem equipItem) {
			var tt = typeof(T);
			if (eType == tt || tt.IsSubclassOf(eType)) {
				T lastEquip = equip1, newEquip = equipItem as T;
				if (lastEquip != newEquip) {
					lastEquip?.doDequip();
					equip1 = newEquip;
					equip1?.doEquip();
					onEquipChanged();
				}
			}
		}

		/// <summary>
		/// 装备改变回调
		/// </summary>
		protected virtual void onEquipChanged() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		public SlotContItem() : base() { }
	}

	/// <summary>
	/// 槽类容器项（双装备）
	/// </summary>
	public abstract class SlotContItem<T1, T2> : SlotContItem<T1>
		where T1 : PackContItem, new() where T2 : PackContItem, new() {

		/// <summary>
		/// 容器项类型
		/// </summary>
		public static Type EquipItem2Type => typeof(T2);
		public override Type equipItem2Type => typeof(T2);

		/// <summary>
		/// 第二装备
		/// </summary>
		public abstract T2 equip2 { get; protected set; }

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		public override bool isNullItem => 
			base.isNullItem && (equip2 == null || equip2.isNullItem);

		/// <summary>
		/// 获取装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <returns>装备</returns>
		//public override E getEquip<E>() {
		//	var et = typeof(E); var t2t = typeof(T2);
		//	if (et == t2t || t2t.IsSubclassOf(et)) return (E)(object)equip2;
		//	return base.getEquip<E>();
		//}
		public override PackContItem getEquip(Type eType) {
			var t2t = typeof(T2);
			if (eType == t2t || t2t.IsSubclassOf(eType)) return equip2;
			return base.getEquip(eType);
		}

		/// <summary>
		/// 设置装备
		/// </summary>
		/// <typeparam name="E">装备类型</typeparam>
		/// <param name="equipItem">装备物品</param>
		public override void setEquip(Type eType, PackContItem equipItem) {
			var t2t = typeof(T2);
			if (eType == t2t || t2t.IsSubclassOf(eType)) {
				T2 lastEquip = equip2, newEquip = equipItem as T2;
				if (lastEquip != newEquip) {
					lastEquip?.doDequip();
					equip2 = newEquip;
					equip2?.doEquip();
					onEquipChanged();
				}
			} else base.setEquip(eType, equipItem);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public SlotContItem() : base() { }

	}

}
