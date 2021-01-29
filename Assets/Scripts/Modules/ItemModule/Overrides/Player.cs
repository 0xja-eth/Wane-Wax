
using System;
using System.Collections.Generic;

using UnityEngine;

using ItemModule.Data;

/// <summary>
/// 信息模块数据
/// </summary>
namespace PlayerModule.Data {

	/// <summary>
	/// 玩家数据（一个存档的数据，包含角色数据、存档数据、关卡状态等）
	/// </summary>
	public partial class Player {

		#region 容器管理

		/// <summary>
		/// 注册的容器
		/// </summary>
		Dictionary<Type, BaseContainer> _containers = null;
		Dictionary<Type, BaseContainer> containers =>
			fetchPropValueDict(ref _containers);

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public BaseContainer getContainer(Type type) {
			if (containers.ContainsKey(type)) return containers[type];
			return null;
		}

		/// <summary>
		/// 获取背包
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		/// <returns></returns>
		public IPackContainer getPack(BaseItem item) {
			if (item == null) return null;

			var pType = item.defaultPackType;
			var res = getContainer(pType) as IPackContainer;
			if (res != null) return res;

			// 遍历查找合适项
			foreach(var container in containers) {
				var pack = container.Value as IPackContainer;
				if (pack?.itemType == item.GetType()) return pack;
			}

			return null;
		}

		/// <summary>
		/// 注册容器
		/// </summary>
		/// <param name="container"></param>
		public void registerContainer(BaseContainer container) {
			containers[container.GetType()] = container;
		}

		#endregion

		#region 物品操作

		/// <summary>
		/// 获得物品
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public bool gainItem(BaseItem item, int count) {
			return getPack(item)?.gainItem(item, count) ?? false;
		}

		/// <summary>
		/// 失去物品
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public bool lostItem(BaseItem item, int count) {
			return getPack(item)?.lostItem(item, count) ?? false;
		}

		#endregion
	}
}