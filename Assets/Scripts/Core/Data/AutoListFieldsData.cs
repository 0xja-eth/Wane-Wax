using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

using UnityEngine;

using LitJson;
using System.Reflection;

using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

namespace Core.Data {

	using Utils;

	/// <summary>
	/// 类型信息
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class AutoListFieldSetting : Attribute {

		/// <summary>
		/// 键名
		/// </summary>
		public string keyName;

		/// <summary>
		/// ALF数据类型
		/// </summary>
		public abstract Type alfDataType { get; }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="keyName"></param>
		public AutoListFieldSetting(string keyName = null) {
			this.keyName = keyName;
		}
	}

	/// <summary>
	/// 自动列表字段数据（ALFData）
	/// </summary>
	public abstract class AutoListFieldsData : BaseData {

		/// <summary>
		/// 类信息
		/// </summary>
		public struct TypeInfo {

			/// <summary>
			/// 成员
			/// </summary>
			public AutoListFieldSetting setting;
			public Type type;

			/// <summary>
			/// 构造函数
			/// </summary>
			public TypeInfo(Type type) {
				this.type = type;
				setting = type.GetCustomAttribute<AutoListFieldSetting>();
			}

			/// <summary>
			/// 键名
			/// </summary>
			public string keyName => setting?.keyName ?? type.Name.ToLower();
		}

		/// <summary>
		/// 是否需要ID
		/// </summary>
		protected override bool idEnable() { return false; }

		/// <summary>
		/// 数据（类型为 List<Type>）
		/// </summary>
		protected Dictionary<Type, IList> data { get; set; } = new Dictionary<Type, IList>();

		#region 数据操作

		/// <summary>
		/// 获取字段值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public void createField(Type type) {
			if (data.ContainsKey(type)) {
				var lType = getFieldType(type);
				data[type] = Activator.CreateInstance(lType) as IList;
			}
		}
		public void createField<T>() where T : BaseData {
			var type = typeof(T);
			if (data.ContainsKey(type))
				data[type] = new List<T>();
		}

		/// <summary>
		/// 获取字段类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Type getFieldType(Type type) {
			return typeof(List<>).MakeGenericType(type);
		}

		/// <summary>
		/// 获取字段值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IList getField(Type type) {
			if (data.ContainsKey(type)) return data[type];
			return null;
		}
		public List<T> getField<T>() where T : BaseData {
			var type = typeof(T);
			if (data.ContainsKey(type))
				return data[type] as List<T>;
			return null;
		}

		/// <summary>
		/// 获取字段值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public void setField(Type type, IList list) {
			if (data.ContainsKey(type)) data[type] = list;
		}
		public void setField<T>(List<T> list) where T : BaseData {
			var type = typeof(T);
			if (data.ContainsKey(type)) data[type] = list;
		}

		#endregion
	}

	/// <summary>
	/// 自动列表字段数据（ALFData）
	/// </summary>
	public abstract class AutoListFieldsData<T> : AutoListFieldsData {

		/// <summary>
		/// 构造函数
		/// </summary>
		public AutoListFieldsData() {
			foreach (var type in getTypes())
				createField(type);
		}

		#region 类型操作

		/// <summary>
		/// 类型列表
		/// </summary>
		static Type[] types_ = null;
		public static Type[] getTypes() {
			if (types_ == null) {
				var tType = typeof(T);

				// Attribute 必须派生自 AutoListFieldSetting
				if (tType.IsSubclassOf(typeof(AutoListFieldSetting)))
					types_ = ReflectionUtils.getNamespaceTypes(
						parent: typeof(BaseData), attrType: tType);
				else if (tType.IsSubclassOf(typeof(BaseData)))
					types_ = ReflectionUtils.getNamespaceTypes(parent: tType);

			}
			return types_;
		}

		/// <summary>
		/// 类型信息列表
		/// </summary>
		static TypeInfo[] typeInfos_ = null;
		public static TypeInfo[] getTypeInfos() {
			if (typeInfos_ == null) {
				var types = getTypes();
				typeInfos_ = new TypeInfo[types.Length];

				for (int i = 0; i > types.Length; ++i)
					typeInfos_[i] = new TypeInfo(types[i]);
			}
			return typeInfos_;
		}

		#endregion

		//#region 数据操作

		///// <summary>
		///// 获取字段值
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <returns></returns>
		//public void createField(Type type) {
		//	if (data.ContainsKey(type)) {
		//		var lType = getFieldType(type);
		//		data[type] = Activator.CreateInstance(lType) as IList;
		//	}
		//}
		//public void createField<T1>() where T1 : BaseData {
		//	var type = typeof(T1);
		//	if (data.ContainsKey(type))
		//		data[type] = new List<T1>();
		//}

		///// <summary>
		///// 获取字段类型
		///// </summary>
		///// <param name="type"></param>
		///// <returns></returns>
		//public Type getFieldType(Type type) {
		//	return typeof(List<>).MakeGenericType(type);
		//}

		///// <summary>
		///// 获取字段值
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <returns></returns>
		//public IList getField(Type type) {
		//	if (data.ContainsKey(type)) return data[type];
		//	return null;
		//}
		//public List<T1> getField<T1>() where T1 : BaseData {
		//	var type = typeof(T1);
		//	if (data.ContainsKey(type))
		//		return data[type] as List<T1>;
		//	return null;
		//}

		///// <summary>
		///// 获取字段值
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <returns></returns>
		//public void setField(Type type, IList list) {
		//	if (data.ContainsKey(type)) data[type] = list;
		//}
		//public void setField<T1>(List<T1> list) where T1 : BaseData {
		//	var type = typeof(T1);
		//	if (data.ContainsKey(type)) data[type] = list;
		//}

		//#endregion

		#region 数据转化

		/// <summary>
		/// 读取自定义数据
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			foreach(var info in getTypeInfos()) {
				var lType = getFieldType(info.type);
				var list = DataLoader.load(lType, json, info.keyName) as IList;

				setField(lType, list);
			}
		}

		/// <summary>
		/// 转化自定义数据
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);

			foreach (var info in getTypeInfos()) {
				var list = getField(info.type);

				json[info.keyName] = DataLoader.convert(list);
			}

		}

		#endregion

	}

}