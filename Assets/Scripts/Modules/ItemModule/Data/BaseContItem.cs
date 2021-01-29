
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
	/// 基本容器项接口
	/// </summary>
	public interface IBaseContItem : IBaseData {

		/// <summary>
		/// 属性
		/// </summary>
		int index { get; }
		int type { get; }

		/// <summary>
		/// 容器
		/// </summary>
		IBaseContainer container { get; }

		/// <summary>
		/// 是否为空
		/// </summary>
		bool isNullItem { get; }
	}

	/// <summary>
	/// 基本容器项
	/// </summary>
	public abstract class BaseContItem : RuntimeData, IBaseContItem {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int index { get; protected set; }
		[AutoConvert]
		public int type { get; protected set; }

		/// <summary>
		/// 对应的容器
		/// </summary>
		public BaseContainer container;
		IBaseContainer IBaseContItem.container => container;

		///// <summary>
		///// 关联类型
		///// </summary>
		//public abstract Type contItemType { get; }
		//public abstract Type itemType { get; }

		/// <summary>
		/// 默认类型
		/// </summary>
		/// <returns></returns>
		//public abstract Type defaultType();

		/// <summary>
		/// 是否为空
		/// </summary>
		/// <returns></returns>
		public virtual bool isNullItem => false;

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseContItem() {
			var typeName = GetType().Name;
			type = (int)Enum.Parse(typeof(Type), typeName);
		}
	}

}
