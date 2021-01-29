using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Events;

namespace Core.Utils {

	/// <summary>
	/// 字典拓展
	/// </summary>
	public class Dict<T1, T2> : 
		IEnumerable<KeyValuePair<T1, T2>> where T2 : new() {

		/// <summary>
		/// 回调设置
		/// </summary>
		protected Dictionary<T1, T2> data = new Dictionary<T1, T2>();

		/// <summary>
		/// 获取原始数据
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		public T2 this[T1 key] {
			get => get(key, false);
			set { set(key, value); }
		}

		/// <summary>
		/// 设置元素
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		public T2 set(T1 key, T2 value) {
			return data[key] = value;
		}

		/// <summary>
		/// 移除元素
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		public void remove(T1 key) {
			data.Remove(key);
		}

		/// <summary>
		/// 清除全部
		/// </summary>
		public void clearAll() {
			data.Clear();
		}

		/// <summary>
		/// 包含键
		/// </summary>
		/// <param name="key">键</param>
		public bool contains(T1 key, bool includeNull = false) {
			if (!data.ContainsKey(key)) return false;
			return includeNull || data[key] != null;
		}

		/// <summary>
		/// 包含键
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="create">创建值</param>
		public T2 get(T1 key, bool create = true) {
			if (data.ContainsKey(key)) return data[key];
			if (create) return this.create(key);
			return default;
		}

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual T2 create(T1 key) {
			return set(key, new T2());
		}

		/// <summary>
		/// 接口
		/// </summary>
		/// <returns></returns>
		IEnumerator<KeyValuePair<T1, T2>> IEnumerable<KeyValuePair<T1, T2>>.GetEnumerator() {
			return data.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}
	}

	/// <summary>
	/// 回调字典
	/// </summary>
	public class DictCallback<TK, TV> : Dict<TK, TV> 
		where TV : BaseCallbackItem<TK>, new() {

		/// <summary>
		/// 创建
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public override TV create(TK key) {
			var res = base.create(key);
			res.setup(key); return res;
		}

		/// <summary>
		/// 唤起回调
		/// </summary>
		/// <param name="type"></param>
		/// <param name="params_"></param>
		public void on(TK key, params object[] params_) {
			this[key]?.on(params_);
		}

		/// <summary>
		/// 唤起回调
		/// </summary>
		/// <param name="type"></param>
		/// <param name="params_"></param>
		public bool judge(TK key) {
			var item = this[key];
			if (item == null) return false;

			var flag = item.flag; item.flag = false;
			return flag;
		}

		/// <summary>
		/// 注册回调
		/// </summary>
		/// <param name="action"></param>
		public void add(TK key, Delegate action) {
			get(key)?.register(action);
		}
		public void add<T0>(TK key, UnityAction<T0> action) {
			get(key)?.register(action);
		}
		public void add<T0, T1>(TK key, UnityAction<T0, T1> action) {
			get(key)?.register(action);
		}
		public void add<T0, T1, T2>(TK key, UnityAction<T0, T1, T2> action) {
			get(key)?.register(action);
		}
		public void add<T0, T1, T2, T3>(TK key, UnityAction<T0, T1, T2, T3> action) {
			get(key)?.register(action);
		}

		/// <summary>
		/// 注册回调
		/// </summary>
		/// <param name="action"></param>
		public void remove(TK key, Delegate action) {
			this[key]?.remove(action);
		}
		public void remove<T0>(TK key, UnityAction<T0> action) {
			this[key]?.remove(action);
		}
		public void remove<T0, T1>(TK key, UnityAction<T0, T1> action) {
			this[key]?.remove(action);
		}
		public void remove<T0, T1, T2>(TK key, UnityAction<T0, T1, T2> action) {
			this[key]?.remove(action);
		}
		public void remove<T0, T1, T2, T3>(TK key, UnityAction<T0, T1, T2, T3> action) {
			this[key]?.remove(action);
		}

		/// <summary>
		/// 注册对象
		/// </summary>
		public void registerObject(object obj) {
			foreach (var cb in data)
				cb.Value.registerObject(obj);
		}

		/// <summary>
		/// 移除对象
		/// </summary>
		public void removeObject(object obj) {
			foreach (var cb in data)
				cb.Value.removeObject(obj);
		}

	}

	/// <summary>
	/// List字典
	/// </summary>
	public class DictList<T1, T2> : Dict<T1, LinkedList<T2>> {

		/// <summary>
		/// 添加元素
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		public void add(T1 key, T2 value) {
			//addListDict(data, key, value);
			get(key, true).AddLast(value);
		}

		/// <summary>
		/// 移除元素
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		public void remove(T1 key, T2 value) {
			//removeListDict(data, key, value);
			get(key, true).Remove(value);
		}

		/// <summary>
		/// 移除元素
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">值</param>
		public void clear(T1 key) {
			//clearListDict(data, key);
			get(key, true).Clear();
		}

		///// <summary>
		///// 包含键
		///// </summary>
		///// <param name="key">键</param>
		//public bool contains(T1 key, bool includeNull = false) {
		//	return hasListDict(data, key, includeNull);
		//}

		///// <summary>
		///// 包含键
		///// </summary>
		///// <param name="key">键</param>
		///// <param name="create">创建值</param>
		//public override LinkedList<T2> get(T1 key, bool create = true) {
		//	return getListDict(data, key, create);
		//}

		//#region 列表字典工具函数

		///// <summary>
		///// 添加一个列表字典
		///// </summary>
		//public static void addListDict(
		//	Dictionary<T1, LinkedList<T2>> dict, T1 key, T2 value) {
		//	getListDict(dict, key, true).AddLast(value);
		//}

		///// <summary>
		///// 添加一个列表字典
		///// </summary>
		//public static void removeListDict(
		//	Dictionary<T1, LinkedList<T2>> dict, T1 key, T2 value) {
		//	getListDict(dict, key, true).Remove(value);
		//}

		///// <summary>
		///// 清空一个列表字典
		///// </summary>
		//public static void clearListDict(
		//	Dictionary<T1, LinkedList<T2>> dict, T1 key) {
		//	getListDict(dict, key, true).Clear();
		//}

		///// <summary>
		///// 是否存在键
		///// </summary>
		//public static bool hasListDict(
		//	Dictionary<T1, LinkedList<T2>> dict, T1 key, bool includeNull = false) {
		//	if (!dict.ContainsKey(key)) return false;
		//	return includeNull || dict[key] != null;
		//}

		///// <summary>
		///// 获取值（数组）（如果没有键可以创建）
		///// </summary>
		//public static LinkedList<T2> getListDict(
		//	Dictionary<T1, LinkedList<T2>> dict, T1 key, bool create = true) {
		//	if (dict.ContainsKey(key)) return dict[key];
		//	if (create) return dict[key] = new LinkedList<T2>();
		//	return null;
		//}

		//#endregion
	}
}
