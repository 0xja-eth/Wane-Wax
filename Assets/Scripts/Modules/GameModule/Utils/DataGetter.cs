using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Core.Data;
using Core.Utils;

namespace GameModule.Utils {

	using Services;

	/// <summary>
	/// 数据获取类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class DataGetter<T> where T : BaseData {

		/// <summary>
		/// ALF配置
		/// </summary>
		static AutoListFieldSetting alfSetting_;
		static AutoListFieldSetting getAlfSetting() {
			return alfSetting_ = alfSetting_ ?? typeof(T).
				GetCustomAttribute<AutoListFieldSetting>();
		}

		/// <summary>
		/// ALF数据
		/// </summary>
		static AutoListFieldsData alfData_;
		static AutoListFieldsData getAlfData() {
			if (alfData_ == null) {
				var setting = getAlfSetting();
				var dType = setting.alfDataType;

				var member = dType.GetMember(
					"Get", ReflectionUtils.DefaultStaticFlags)[0];

				switch (member.MemberType) {
					case MemberTypes.Method:
						alfData_ = (member as MethodInfo).
							Invoke(null, null) as AutoListFieldsData;
						break;
					case MemberTypes.Property:
						alfData_ = (member as PropertyInfo).GetGetMethod().
							Invoke(null, null) as AutoListFieldsData;
						break;
				}
			}
			return alfData_;
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static T get(int id) {
			var collection = getCollection();
			if (collection == null) return null;
			return collectionGet(collection, id);
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static List<T> getCollection() {
			return getAlfData()?.getField<T>();
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static int indexOf(int id) {
			var collection = getAlfData()?.getField<T>();
			if (collection == null) return -1;
			return collectionIndexOf(collection, id);
		}

		#region 工具函数

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="collection">数据集合</param>
		/// <param name="id">ID</param>
		/// <returns>目标数据</returns>
		public static T collectionGet<T>(T[] collection, int id) where T : BaseData {
			foreach (var element in collection)
				if (element.id == id) return element;
			return default;
		}
		public static T collectionGet<T>(List<T> collection, int id) where T : BaseData {
			return collection.Find((d) => d.id == id);
		}
		public static BaseData collectionGet(IList collection, int id) {
			foreach (BaseData element in collection)
				if (element.id == id) return element;
			return default;
		}

		/// <summary>
		/// 获取数据下标
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="collection">数据集合</param>
		/// <param name="id">ID</param>
		/// <returns>目标数据下标</returns>
		public static int collectionIndexOf<T>(T[] collection, int id) where T : BaseData {
			for (int i = 0; i < collection.Length; ++i)
				if (collection[i].id == id) return i;
			return -1;
		}
		public static int collectionIndexOf<T>(List<T> collection, int id) where T : BaseData {
			return collection.FindIndex(c => c.id == id);
		}
		public static int collectionIndexOf(IList collection, int id) {
			for (int i = 0; i < collection.Count; ++i)
				if ((collection[i] as BaseData).id == id) return i;
			return -1;
		}

		#endregion
	}

	/// <summary>
	/// 数据获取类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class DataGetter {

		/// <summary>
		/// 获取对应的DataGetter类
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		static Type getDataGetterType(Type type) {
			var gType = typeof(DataGetter<>);
			return gType.MakeGenericType(type);
		}

		/// <summary>
		/// 获取对应的DataGetter类
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		static MethodInfo getDataGetterMethod(Type type, string method) {
			return getDataGetterType(type).GetMethod(method);
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static BaseData get(Type type, int id) {
			var gMethod = getDataGetterMethod(type, "get");
			return gMethod.Invoke(null, new object[] { id }) as BaseData;
		}

		/// <summary>
		/// 获取数据
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static List<BaseData> getCollection(Type type) {
			var gMethod = getDataGetterMethod(type, "getCollection");
			var iList = gMethod.Invoke(null, null) as IList;

			var res = new List<BaseData>(iList.Count);
			foreach (var item in iList) res.Add(item as BaseData);

			return res;
		}

		/// <summary>
		/// 获取下标
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static int indexOf(Type type, int id) {
			var gMethod = getDataGetterMethod(type, "indexOf");
			return (int)gMethod.Invoke(null, new object[] { id });
		}
		
		/// <summary>
		/// 获取数据
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="collection">数据集合</param>
		/// <param name="id">ID</param>
		/// <returns>目标数据</returns>
		public static Tuple<int, string> collectionGet(Tuple<int, string>[] collection, int id) {
			foreach (var element in collection)
				if (element.Item1 == id) return element;
			return default;
		}

		/// <summary>
		/// 获取数据下标
		/// </summary>
		/// <typeparam name="T">数据类型</typeparam>
		/// <param name="collection">数据集合</param>
		/// <param name="id">ID</param>
		/// <returns>目标数据下标</returns>
		public static int collectionIndexOf(Tuple<int, string>[] collection, int id) {
			for (int i = 0; i < collection.Length; ++i)
				if (collection[i].Item1 == id) return i;
			return -1;
		}
	}
}
