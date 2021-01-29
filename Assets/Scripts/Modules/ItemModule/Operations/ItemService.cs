using System;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core;

using Core.Data;
using Core.Data.Loaders;

using Core.Services;
using Core.Systems;

using Core.Components.Utils;

/// <summary>
/// 基本系统
/// </summary>
namespace ItemModule.Services {

	using Data;

    /// <summary>
    /// 玩家服务类
    /// </summary>
    public partial class ItemService {

		/// <summary>
		/// 容器获取
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public abstract class ContainerOperation : Operation {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public int type { get; set; }
			[AutoConvert]
			public int cid { get; set; }

			/// <summary>
			/// 自身容器
			/// </summary>
			bool self = false;

			/// <summary>
			/// 容器
			/// </summary>
			protected BaseContainer container;

			/// <summary>
			/// 配置
			/// </summary>
			public override bool isLocalEnable => false;

			/// <summary>
			/// 有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() { return container != null; }

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="container">容器</param>
			protected void invoke(BaseContainer container,
				UnityAction onSuccess = null, UnityAction onError = null) {
				this.container = container; invoke(onSuccess, onError);
			}

			/// <summary>
			/// 配置请求参数
			/// </summary>
			protected override void setupRequestParams() {
				type = container.type; cid = self ? 0 : container.id;
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				container = DataLoader.load(container, data, "container");
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="self"></param>
			public ContainerOperation(bool self = false) { this.self = self; }
		}

		/// <summary>
		/// 背包获取
		/// </summary>
		public class GetPack : ContainerOperation {

			/// <summary>
			/// 容器
			/// </summary>
			protected IPackContainer packContainer => container as IPackContainer;

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="container"></param>
			/// <param name="onSuccess"></param>
			/// <param name="onError"></param>
			public void invoke(IPackContainer container,
				UnityAction onSuccess = null, UnityAction onError = null) {
				invoke(container, onSuccess, onError);
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="self"></param>
			public GetPack(bool self = false) : base(self) { }
		}
		public GetPack getPack => new GetPack();
		public GetPack getSelfPack => new GetPack(true);

		/// <summary>
		/// 背包获取
		/// </summary>
		public class GetSlot : ContainerOperation {

			/// <summary>
			/// 容器
			/// </summary>
			protected ISlotContainer slotContainer => container as ISlotContainer;

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="container"></param>
			/// <param name="onSuccess"></param>
			/// <param name="onError"></param>
			public void invoke(ISlotContainer container,
				UnityAction onSuccess = null, UnityAction onError = null) {
				invoke(container, onSuccess, onError);
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="self"></param>
			public GetSlot(bool self = false) : base(self) { }
		}
		public GetSlot getSlot => new GetSlot();
		public GetSlot getSelfSlot => new GetSlot(true);

		/// <summary>
		/// 容器获取
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public class GainItem : GetPack {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public int iType { get; set; }
			[AutoConvert]
			public int itemId { get; set; }
			[AutoConvert]
			public int count { get; set; }
			[AutoConvert]
			public bool refresh { get; set; }

			/// <summary>
			/// 容器
			/// </summary>
			protected BaseItem item;

			/// <summary>
			/// 配置
			/// </summary>
			public override bool isLocalEnable => true;
			public override Interface interface_ => isGain ? 
				ItemConfig.GainItem : ItemConfig.LostItem;

			/// <summary>
			/// 获得/失去
			/// </summary>
			bool isGain => count >= 0;
			bool isLost => count < 0;

			/// <summary>
			/// 有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() {
				if (base.isValid()) return false;
				if (packContainer == null || item == null) return false;

				if (isGain) return packContainer.isItemGainEnable(item, count);
				if (isLost) return packContainer.isItemLostEnable(item, -count);

				return true;
			}

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="container">容器</param>
			public virtual void invoke(IPackContainer container, BaseItem item, int count, bool refresh = false,
				UnityAction onSuccess = null, UnityAction onError = null) {

				this.item = item; this.count = count;
				this.refresh = refresh;

				invoke(container, onSuccess, onError);
			}

			/// <summary>
			/// 配置请求参数
			/// </summary>
			protected override void setupRequestParams() {
				base.setupRequestParams();
				iType = item.type; itemId = item.id;
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				if (refresh) base.processSuccess(data);
				else processLocal();
			}

			/// <summary>
			/// 处理本地
			/// </summary>
			protected override void processLocal() {
				if (isGain) packContainer.gainItem(item, count);
				if (isLost) packContainer.lostItem(item, -count);
			}
		}
		GainItem gainItem => new GainItem();
		//public void gainItem<CI, I>(
		//	PackContainer<CI, I> container, I item, int count, bool refresh = false,
		//	UnityAction onSuccess = null, UnityAction onError = null)
		//	where CI : PackContItem<I>, new() where I : BaseItem, new() {

		//	new GainItem<CI, I>().invoke(container, item, count, refresh, onSuccess, onError);
		//}

		/// <summary>
		/// 容器获取
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public class LostItem : GainItem {

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="container">容器</param>
			public override void invoke(IPackContainer container, BaseItem item, int count, bool refresh = false,
				UnityAction onSuccess = null, UnityAction onError = null) {
				base.invoke(container, item, -count, refresh, onSuccess, onError);
			}
		}
		LostItem lostItem => new LostItem();

		//public void lostItem<CI, I>(
		//	PackContainer<CI, I> container, I item, int count, bool refresh = false,
		//	UnityAction onSuccess = null, UnityAction onError = null)
		//	where CI : PackContItem<I>, new() where I : BaseItem, new() {

		//	gainItem(container, item, -count, refresh, onSuccess, onError);
		//}

		/// <summary>
		/// 容器获取
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public class LostContItems : GetPack {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public int[] indices { get; set; }
			[AutoConvert]
			public int[] counts { get; set; }
			[AutoConvert]
			public bool refresh { get; set; }

			/// <summary>
			/// 容器
			/// </summary>
			protected PackContItem[] contItems;

			/// <summary>
			/// 配置
			/// </summary>
			public override bool isLocalEnable => true;

			/// <summary>
			/// 有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() {
				if (contItems == null) return false;
				if (base.isValid()) return false;

				var cnt = contItems.Length;
				if (counts.Length != cnt) return false;

				for (int i = 0; i < cnt; ++i)
					if (!isValid(contItems[i], counts[i])) return false;

				return true;
			}
			bool isValid(PackContItem contItem, int count) {
				return contItem.count < 0 || contItem.count >= count;
			}

			/// <summary>
			/// 执行
			/// </summary>
			/// <param name="container">容器</param>
			public void invoke(IPackContainer container,
				PackContItem[] contItems, int[] counts, bool refresh = false,
				UnityAction onSuccess = null, UnityAction onError = null) {

				this.contItems = contItems;
				this.counts = counts; this.refresh = refresh;

				invoke(container, onSuccess, onError);
			}
			public void invoke(IPackContainer container,
				PackContItem contItem, int count, bool refresh = false,
				UnityAction onSuccess = null, UnityAction onError = null) {

				var counts = new int[] { count };
				var contItems = new PackContItem[] { contItem };
				invoke(container, contItems, counts, refresh, onSuccess, onError);
			}

			/// <summary>
			/// 配置请求参数
			/// </summary>
			protected override void setupRequestParams() {
				base.setupRequestParams();
				var cnt = contItems.Length;

				indices = new int[cnt];
				for (int i = 0; i < cnt; ++i)
					indices[i] = contItems[i].index;
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				if (refresh) base.processSuccess(data);
				else processLocal();
			}

			/// <summary>
			/// 处理本地
			/// </summary>
			protected override void processLocal() {
				for (int i = 0; i < contItems.Length; ++i) {
					var contItem = contItems[i];
					contItem.leave(counts[i]);

					if (contItem.isEmpty)
						packContainer.removeItem(contItem);
				}
			}
		}
		LostContItems lostContItems => new LostContItems();
		//public void lostContItem<T>(PackContainer<T> container, 
		//	T contItem, int count, bool refresh = true, 
		//	UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

		//	var contItems = new T[] { contItem };
		//	var counts = count >= 0 ? new int[] { count } : null;
		//	lostContItems(container, contItems, counts, refresh, onSuccess, onError);
		//}
		///// <param name="contItems">容器项集</param>
		///// <param name="counts">每个容器项失去的数目</param>
		//public void lostContItems<T>(PackContainer<T> container, 
		//	T[] contItems, int[] counts, bool refresh = true,
		//	UnityAction onSuccess = null, UnityAction onError = null) where T : PackContItem, new() {

		//	new LostContItems<T>().invoke(container,
		//		contItems, counts, refresh, onSuccess, onError);
		//}

	}

}