
using System;
using System.Collections.Generic;

using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;

using Core.Utils;

using AssetModule.Services;

using GameModule.Services;
using PlayerModule.Services;

using ItemModule.Data;
using UnityEngine;

/// <summary>
/// 物品模块服务
/// </summary>
namespace ItemModule.Services {

	/// <summary>
	/// 物品服务
	/// </summary>
	public partial class ItemService : BaseService<ItemService> {

		/// <summary>
		/// 物品类型字典
		/// </summary>
		Dictionary<int, Type> itemTypes = new Dictionary<int, Type>();
		Dictionary<int, Type> containerTypes = new Dictionary<int, Type>();
		Dictionary<int, Type> contItemTypes = new Dictionary<int, Type>();

		/// <summary>
		/// 物品-背包类型字典
		/// </summary>
		//Dictionary<Type, Type> itemPackMap = new Dictionary<Type, Type>();

		/// <summary>
		/// 外部系统设置
		/// </summary>
		DataService dataSer;
		PlayerService playerSer;

		#region 初始化

		/// <summary>
		/// 初始化物品系统
		/// </summary>
		protected override void initializeElements() {
			setupSystemElement<BaseItem, ItemType>(itemTypes);
			setupSystemElement<BaseContainer, ContainerType>(containerTypes);
			setupSystemElement<BaseContItem, ContItemType>(contItemTypes);
		}

		#endregion

		#region 元素

		/// <summary>
		/// 获取物品类型
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Type getItemType(int value) {
			return getType(itemTypes, value);
		}

		/// <summary>
		/// 获取容器类型
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Type getContainerType(int value) {
			return getType(containerTypes, value);
		}

		/// <summary>
		/// 获取容器项类型
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Type getContItemType(int value) {
			return getType(contItemTypes, value);
		}

		/// <summary>
		/// 获取物品类型
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public BaseData getItem(int value, int id) {
			return dataSer.get(getItemType(value), id);
		}

		/// <summary>
		/// 获取类型枚举
		/// </summary>
		/// <typeparam name="T">物品/容器/容器项类型</typeparam>
		/// <returns></returns>
		public int getTypeEnum<T>() {
			return getTypeEnum(typeof(T));
		}
		/// <param name="type">物品/容器/容器项类型</param>
		public int getTypeEnum(Type type) {
			Dictionary<int, Type> dict;
			if (type.IsSubclassOf(typeof(BaseItem)))
				dict = itemTypes;
			else if (type.IsSubclassOf(typeof(BaseContainer)))
				dict = containerTypes;
			else if (type.IsSubclassOf(typeof(BaseContItem)))
				dict = contItemTypes;
			else return -1;

			return getTypeEnum(dict, type);
		}

		#endregion

		#region 操作控制

		///// <summary>
		///// 获取背包容器
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void getPack<T>(PackContainer<T> container,
		//	UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	getPack(container.type, container.id, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="cid">容器ID</param>
		//public void getPack(int type, int cid,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData();
		//	data["type"] = type; data["cid"] = cid;
		//	sendRequest(ItemConfig.GetPack, data, onSuccess, onError);
		//}

		///// <summary>
		///// 获取自身背包容器
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void getSelfPack<T>(PackContainer<T> container,
		//	UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	getSelfPack(container.type, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		//public void getSelfPack(int type,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData(); data["type"] = type;
		//	sendRequest(ItemConfig.GetPack, data, onSuccess, onError);
		//}

		///// <summary>
		///// 获取槽容器
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void getSlot<T>(SlotContainer<T> container,
		//	UnityAction onSuccess, UnityAction onError = null) where T : SlotContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	getSlot(container.type, container.id, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="cid">容器ID</param>
		//public void getSlot(int type, int cid,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData();
		//	data["type"] = type; data["cid"] = cid;
		//	sendRequest(ItemConfig.GetSlot, data, onSuccess, onError);
		//}

		///// <summary>
		///// 获取自身槽容器
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void getSelfSlot<T>(SlotContainer<T> container,
		//	UnityAction onSuccess, UnityAction onError = null) where T : SlotContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	getSelfSlot(container.type, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		//public void getSelfSlot(int type,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData(); data["type"] = type;
		//	sendRequest(ItemConfig.GetSlot, data, onSuccess, onError);
		//}

		///// <summary>
		///// 获得物品
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="item">物品</param>
		///// <param name="count">数量</param>
		///// <param name="realTime">实时刷新</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void gainItem<T>(PackContainer<T> container, BaseItem item, int count, bool realTime,
		//	UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

		//	if (count == 0) { onSuccess?.Invoke(); return; }

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		if (realTime) container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	gainItem(container.type, container.id, item.type, item.id,
		//		count, realTime, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="cid">容器ID</param>
		///// <param name="iType">物品类型</param>
		///// <param name="itemId">物品ID</param>
		//public void gainItem(int type, int cid, int iType, int itemId, int count, bool realTime,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

		//	JsonData data = new JsonData();
		//	data["type"] = type; data["cid"] = cid;
		//	data["i_type"] = iType; data["item_id"] = itemId;
		//	data["count"] = count; data["refresh"] = realTime;
		//	var oper = count >= 0 ? ItemConfig.GainItem : ItemConfig.LostItem;
		//	sendRequest(oper, data, onSuccess, onError);
		//}

		///// <summary>
		///// 失去物品
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="item">物品</param>
		///// <param name="count">数量</param>
		///// <param name="realTime">实时刷新</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void lostItem<T>(PackContainer<T> container, BaseItem item, int count, bool realTime,
		//	UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {
		//	gainItem(container, item, -count, realTime, onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="cid">容器ID</param>
		///// <param name="iType">物品类型</param>
		///// <param name="itemId">物品ID</param>
		//public void lostItem(int type, int cid, int iType, int itemId, int count, bool realTime,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	gainItem(type, cid, iType, itemId, -count, realTime, onSuccess, onError);
		//}

		///// <summary>
		///// 获得容器项
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="contItem">容器项</param>
		///// <param name="realTime">实时刷新</param>
		///// <param name="fixed_">整体操作</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void gainContItem<T>(PackContainer<T> container, T contItem,
		//	bool realTime = true, bool fixed_ = false, UnityAction onSuccess = null,
		//	UnityAction onError = null) where T : PackContItem, new() {
		//	var contItems = new T[] { contItem };
		//	gainContItems(container, contItems, realTime, fixed_, onSuccess, onError);
		//}
		///// <param name="contItems">容器项集</param>
		//public void gainContItems<T>(PackContainer<T> container, T[] contItems,
		//	bool realTime = true, bool fixed_ = false, UnityAction onSuccess = null,
		//	UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		if (realTime) container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	var count = contItems.Length;
		//	var ciTypes = new int[count];
		//	var contItemIds = new int[count];
		//	for (int i = 0; i < count; i++) {
		//		ciTypes[i] = contItems[i].type;
		//		contItemIds[i] = contItems[i].id;
		//	}

		//	gainContItems(container.type, ciTypes, contItemIds,
		//		realTime, fixed_, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="ciTypes">容器项类型集</param>
		///// <param name="contItemIds">容器项ID集</param>
		//public void gainContItems(int type, int[] ciTypes, int[] contItemIds, bool realTime, bool fixed_,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

		//	JsonData data = new JsonData();
		//	data["type"] = type;
		//	data["ci_types"] = DataLoader.convert(ciTypes);
		//	data["contitem_ids"] = DataLoader.convert(contItemIds);
		//	data["fixed"] = fixed_; data["refresh"] = realTime;

		//	sendRequest(ItemConfig.GainContItems, data, onSuccess, onError);
		//}

		///// <summary>
		///// 失去容器项
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="contItem">容器项</param>
		///// <param name="count">失去数目</param>
		///// <param name="realTime">实时刷新</param>
		///// <param name="fixed_">整体操作</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void lostContItem<T>(PackContainer<T> container, T contItem, int count = -1,
		//	bool realTime = true, bool fixed_ = false, UnityAction onSuccess = null,
		//	UnityAction onError = null) where T : PackContItem, new() {
		//	var contItems = new T[] { contItem };
		//	var counts = count >= 0 ? new int[] { count } : null;
		//	lostContItems(container, contItems, counts, realTime, fixed_, onSuccess, onError);
		//}
		///// <param name="contItems">容器项集</param>
		///// <param name="counts">每个容器项失去的数目</param>
		//public void lostContItems<T>(PackContainer<T> container, T[] contItems,
		//	int[] counts = null, bool realTime = true, bool fixed_ = false,
		//	UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		if (realTime) container = DataLoader.load(container, res, "container");
		//		onSuccess?.Invoke();
		//	};

		//	var count = contItems.Length;
		//	var ciTypes = new int[count];
		//	var contItemIds = new int[count];
		//	for (int i = 0; i < count; i++) {
		//		ciTypes[i] = contItems[i].type;
		//		contItemIds[i] = contItems[i].id;
		//	}

		//	lostContItems(container.type, ciTypes, contItemIds, counts,
		//		realTime, fixed_, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="ciTypes">容器项类型集</param>
		///// <param name="contItemIds">容器项ID集</param>
		//public void lostContItems(int type, int[] ciTypes, int[] contItemIds, int[] counts,
		//	bool realTime, bool fixed_, NetworkSystem.RequestObject.SuccessAction onSuccess,
		//	UnityAction onError = null) {

		//	JsonData data = new JsonData();
		//	data["type"] = type;
		//	data["ci_types"] = DataLoader.convert(ciTypes);
		//	data["contitem_ids"] = DataLoader.convert(contItemIds);
		//	data["fixed"] = fixed_; data["refresh"] = realTime;

		//	if (counts != null) data["counts"] = DataLoader.convert(counts);

		//	sendRequest(ItemConfig.LostContItems, data, onSuccess, onError);
		//}

		/* TODO: 补充实现

		/// <summary>
		/// 转移物品
		/// </summary>
		/// <param name="container">容器</param>
		/// <param name="target">目标容器</param>
		/// <param name="contItem">容器项</param>
		/// <param name="count">转移数量</param>
		/// <param name="onSuccess">成功回调</param>
		/// <param name="onError">失败回调</param>
		public void transferItem<T>(PackContainer<T> container, PackContainer<T> target, T contItem, int count = 1,
			UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {
			var contItems = new T[] { contItem };
			var counts = count >= 0 ? new int[] { count } : null;
			transferItems(container, target, contItems, counts, onSuccess, onError);
		}
		/// <param name="targetCid">目标容器ID</param>
		public void transferItem<T>(PackContainer<T> container, int targetCid, T contItem, int count = -1,
			UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {
			var contItems = new T[] { contItem };
			var counts = count >= 0 ? new int[] { count } : null;
			transferItems(container, targetCid, contItems, counts, onSuccess, onError);
		}
		/// <param name="contItems">容器项集</param>
		/// <param name="counts">每个容器项转移的数量</param>
		public void transferItems<T>(PackContainer<T> container,
			PackContainer<T> target, T[] contItems, int[] counts = null,
			UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

			NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
				container = DataLoader.load(container, res, "container");
				target = DataLoader.load(target, res, "target");
				onSuccess?.Invoke();
			};

			var count = contItems.Length;
			var ciTypes = new int[count];
			var contItemIds = new int[count];
			for (int i = 0; i < count; i++) {
				ciTypes[i] = contItems[i].type;
				contItemIds[i] = contItems[i].id;
			}

			transferItems(container.type, target.id, ciTypes,
				contItemIds, counts, _onSuccess, onError);
		}
		public void transferItems<T>(PackContainer<T> container,
			int targetCid, T[] contItems, int[] counts = null,
			UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

			NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
				container = DataLoader.load(container, res, "container");
				onSuccess?.Invoke();
			};

			var count = contItems.Length;
			var ciTypes = new int[count];
			var contItemIds = new int[count];
			for (int i = 0; i < count; i++) {
				ciTypes[i] = contItems[i].type;
				contItemIds[i] = contItems[i].id;
			}

			transferItems(container.type, targetCid, ciTypes,
				contItemIds, counts, _onSuccess, onError);
		}
		/// <param name="type">容器类型</param>
		/// <param name="ciTypes">容器项类型集</param>
		/// <param name="contItemIds">容器项ID集</param>
		public void transferItems(int type, int targetCid,
			int[] ciTypes, int[] contItemIds, int[] counts,
			NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
			JsonData data = new JsonData();
			data["type"] = type;
			data["target_cid"] = targetCid;
			data["ci_types"] = DataLoader.convert(ciTypes);
			data["contitem_ids"] = DataLoader.convert(contItemIds);

			if (counts != null) data["counts"] = DataLoader.convert(counts);

			sendRequest(ItemConfig.TransferItems, data, onSuccess, onError);
		}

		/// <summary>
		/// 拆分物品
		/// </summary>
		/// <param name="container">容器</param>
		/// <param name="contItem">容器项</param>
		/// <param name="count">拆分数量</param>
		/// <param name="onSuccess">成功回调</param>
		/// <param name="onError">失败回调</param>
		public void splitItem<T>(PackContainer<T> container, T contItem, int count,
			UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

			NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
				container = DataLoader.load(container, res, "container");
				onSuccess?.Invoke();
			};

			splitItem(container.type, contItem.type,
				contItem.id, count, _onSuccess, onError);
		}
		/// <param name="type">容器类型</param>
		/// <param name="ciType">容器项类</param>
		/// <param name="contItemId">容器项ID</param>
		public void splitItem(int type, int ciType, int contItemId, int count,
			NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
			JsonData data = new JsonData();
			data["type"] = type; data["count"] = count;
			data["contitem_id"] = contItemId;
			sendRequest(ItemConfig.SplitItem, data, onSuccess, onError);
		}

		/// <summary>
		/// 合并物品
		/// </summary>
		/// <param name="container">容器</param>
		/// <param name="contItems">容器项</param>
		/// <param name="onSuccess">成功回调</param>
		/// <param name="onError">失败回调</param>
		public void mergeItems<T>(PackContainer<T> container, T[] contItems,
			UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

			NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
				container = DataLoader.load(container, res, "container");
				onSuccess?.Invoke();
			};

			var count = contItems.Length;
			var ciTypes = new int[count];
			var contItemIds = new int[count];
			for (int i = 0; i < count; i++) {
				ciTypes[i] = contItems[i].type;
				contItemIds[i] = contItems[i].id;
			}

			mergeItems(container.type, ciTypes, contItemIds, _onSuccess, onError);
		}
		/// <param name="type">容器类型</param>
		/// <param name="ciTypes">容器项类型集</param>
		/// <param name="contItemIds">容器项ID集</param>
		public void mergeItems(int type, int[] ciTypes, int[] contItemIds,
			NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
			JsonData data = new JsonData();
			data["type"] = type;
			data["ci_types"] = DataLoader.convert(ciTypes);
			data["contitem_ids"] = DataLoader.convert(contItemIds);
			sendRequest(ItemConfig.MergeItems, data, onSuccess, onError);
		}

		/// <summary>
		/// 丢弃物品
		/// </summary>
		/// <param name="container">容器</param>
		/// <param name="contItem">容器项</param>
		/// <param name="count">丢弃数量</param>
		/// <param name="onSuccess">成功回调</param>
		/// <param name="onError">失败回调</param>
		public void discardItem<T>(PackContainer<T> container, T contItem, int count,
			UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

			NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
				container = DataLoader.load(container, res, "container");
				onSuccess?.Invoke();
			};

			discardItem(container.type, contItem.type,
				contItem.id, count, _onSuccess, onError);
		}
		/// <param name="type">容器类型</param>
		/// <param name="ciType">容器项类型</param>
		/// <param name="contItemId">容器项ID</param>
		public void discardItem(int type, int ciType, int contItemId, int count,
			NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
			JsonData data = new JsonData();
			data["type"] = type;
			data["count"] = count;
			data["ci_type"] = ciType;
			data["contitem_id"] = contItemId;
			sendRequest(ItemConfig.DiscardItem, data, onSuccess, onError);
		}
		*/

		///// <summary>
		///// 出售物品
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="contItem">容器项</param>
		///// <param name="count">丢弃数量</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void sellItem<T>(PackContainer<T> container, T contItem, int count,
		//	UnityAction onSuccess, UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		getPlayer().loadMoney(DataLoader.load(res, "money"));
		//		onSuccess?.Invoke();
		//	};

		//	sellItem(container.type, contItem.type,
		//		contItem.id, count, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="ciType">容器项类型</param>
		///// <param name="contItemId">容器项ID</param>
		//public void sellItem(int type, int ciType, int contItemId, int count,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData();
		//	data["type"] = type;
		//	data["count"] = count;
		//	data["ci_type"] = ciType;
		//	data["contitem_id"] = contItemId;
		//	sendRequest(ItemConfig.SellItem, data, onSuccess, onError);
		//}

		///// <summary>
		///// 使用物品
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="contItem">容器项</param>
		///// <param name="count">丢弃数量</param>
		///// <param name="occasion">使用场合</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void useItem<T>(PackContainer<T> container, T contItem,
		//	int count, ItemUseOccasion occasion, PlayerExermon playerExer,
		//	UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		playerSer.loadPlayer(DataLoader.load(res, "player"));
		//		onSuccess?.Invoke();
		//	};

		//	useItem(container.type, contItem.type, contItem.id,
		//		count, (int)occasion, playerExer.id, _onSuccess, onError);
		//}
		///// <param name="target">目标</param>
		//public void useItem<T>(PackContainer<T> container, T contItem,
		//	int count, ItemUseOccasion occasion, UnityAction onSuccess = null,
		//	UnityAction onError = null) where T : PackContItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		playerSer.loadPlayer(DataLoader.load(res, "player"));
		//		onSuccess?.Invoke();
		//	};

		//	useItem(container.type, contItem.type, contItem.id,
		//		count, (int)occasion, 0, _onSuccess, onError);
		//}
		///// <param name="type">容器类型</param>
		///// <param name="ciType">容器项类型</param>
		///// <param name="contItemId">容器项ID</param>
		//public void useItem(int type, int ciType, int contItemId, int count, int occasion, int target,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData();
		//	data["type"] = type; data["ci_type"] = ciType;
		//	data["contitem_id"] = contItemId; data["count"] = count;
		//	data["occasion"] = occasion; data["target"] = target;
		//	sendRequest(UseItem, data, onSuccess, onError);
		//}

		///// <summary>
		///// 购买物品
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="item">物品</param>
		///// <param name="count">购买数量</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void buyItem<T>(PackContainer<PackContItem> container, T item, int count, int buyType,
		//	UnityAction onSuccess, UnityAction onError = null) where T : BaseItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		getPlayer().loadMoney(DataLoader.load(res, "money"));
		//		onSuccess?.Invoke();
		//	};

		//	buyItem(item.type, item.id, count, buyType, _onSuccess, onError);
		//}
		///// <param name="type">物品类型</param>
		///// <param name="itemId">物品ID</param>
		//public void buyItem(int type, int itemId, int count, int buyType,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData(); data["type"] = type;
		//	data["item_id"] = itemId; data["count"] = count; data["buy_type"] = buyType;
		//	sendRequest(BuyItem, data, onSuccess, onError);
		//}

		///// <summary>
		///// 获取商品
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="item">物品</param>
		///// <param name="count">丢弃数量</param>
		///// <param name="onSuccess">成功回调</param>
		///// <param name="onError">失败回调</param>
		//public void getShop<T>(UnityAction<ShopItem<T>[]> onSuccess,
		//	UnityAction onError = null) where T : BaseItem, new() {

		//	NetworkSystem.RequestObject.SuccessAction _onSuccess = (res) => {
		//		var items = DataLoader.load<ShopItem<T>[]>(res, "items");
		//		Debug.Log("GetShop: " + string.Join(",", (object[])items));
		//		onSuccess?.Invoke(items);
		//	};

		//	var typeName = typeof(T).Name;
		//	var type = (int)Enum.Parse(typeof(BaseItem.Type), typeName);

		//	getShop(type, _onSuccess, onError);
		//}
		///// <param name="type">物品类型</param>
		///// <param name="itemId">物品ID</param>
		//public void getShop(int type,
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {
		//	JsonData data = new JsonData(); data["type"] = type;
		//	sendRequest(GetShop, data, onSuccess, onError);
		//}

		///// <summary>
		///// 槽操作
		///// </summary>
		///// <param name="container">容器</param>
		///// <param name="slot">槽</param>
		///// <param name="onSuccess">成功回调</param>
		//public NetworkSystem.RequestObject.SuccessAction slotOperationSuccess<T, E>(
		//	PackContainer<T> container, SlotContainer<E> slot, UnityAction onSuccess)
		//	where T : PackContItem, new() where E : SlotContItem, new() {
		//	return (res) => {
		//		container = DataLoader.load(container, res, "container");
		//		slot = DataLoader.load(slot, res, "slot");
		//		onSuccess?.Invoke();
		//	};
		//}

		#endregion
	}
}