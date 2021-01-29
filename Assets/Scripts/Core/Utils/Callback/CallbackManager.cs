using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

namespace Core.Utils {
	
	/// <summary>
	/// 回调管理类 V1.0
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CallbackManager : EnumTypeProcessor {

		/// <summary>
		/// 回调项
		/// </summary>
		public class CallbackItem : BaseCallbackItem<string> {

			/// <summary>
			/// 函数名
			/// </summary>
			protected override string methodName => "_on" + name;

		}

		/// <summary>
		/// 回调设置
		/// </summary>
		DictCallback<string, CallbackItem> callbacks = new DictCallback<string, CallbackItem>();

		#region 工具函数

		/// <summary>
		/// 提取类型数组
		/// </summary>
		/// <param name="params_"></param>
		/// <returns></returns>
		public static Type[] getTypes(params object[] params_) {
			var res = new Type[params_.Length];
			for (int i = 0; i < params_.Length; ++i)
				res[i] = params_[i].GetType();

			return res;
		}
		public static Type[] getTypes(params ParameterInfo[] params_) {
			var res = new Type[params_.Length];
			for (int i = 0; i < params_.Length; ++i)
				res[i] = params_[i].ParameterType;

			return res;
		}

		/// <summary>
		/// 获取方法
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MethodInfo getMethod(object obj, string name, params Type[] types) {
			return obj.GetType().GetMethod(name,
				ReflectionUtils.DefaultFlags, null, types, null);
		}

		#endregion

		#region 注册对象管理

		/// <summary>
		/// 注册对象
		/// </summary>
		public void registerObject(object obj) {
			callbacks.registerObject(obj);
		}

		/// <summary>
		/// 移除对象
		/// </summary>
		public void removeObject(object obj) {
			callbacks.removeObject(obj);
		}

		#endregion

		#region 调用

		/// <summary>
		/// 注册回调
		/// </summary>
		/// <param name="action"></param>
		public void register(Enum type, Delegate action) {
			register(type.ToString(), action);
		}
		public void register(string name, Delegate action) {
			callbacks.add(name, action);
		}
		public void register(Enum type, UnityAction action) {
			register(type.ToString(), action);
		}
		public void register(string name, UnityAction action) {
			callbacks.add(name, action);
		}
		public void register<T0>(Enum type, UnityAction<T0> action) {
			register(type.ToString(), action);
		}
		public void register<T0>(string name, UnityAction<T0> action) {
			callbacks.add(name, action);
		}
		public void register<T0, T1>(Enum type, UnityAction<T0, T1> action) {
			register(type.ToString(), action);
		}
		public void register<T0, T1>(string name, UnityAction<T0, T1> action) {
			callbacks.add(name, action);
		}
		public void register<T0, T1, T2>(Enum type, UnityAction<T0, T1, T2> action) {
			register(type.ToString(), action);
		}
		public void register<T0, T1, T2>(string name, UnityAction<T0, T1, T2> action) {
			callbacks.add(name, action);
		}
		public void register<T0, T1, T2, T3>(Enum type, UnityAction<T0, T1, T2, T3> action) {
			register(type.ToString(), action);
		}
		public void register<T0, T1, T2, T3>(string name, UnityAction<T0, T1, T2, T3> action) {
			callbacks.add(name, action);
		}

		/// <summary>
		/// 注册回调
		/// </summary>
		/// <param name="action"></param>
		public void remove(Enum type, Delegate action) {
			remove(type.ToString(), action);
		}
		public void remove(string name, Delegate action) {
			callbacks.remove(name, action);
		}
		public void remove(Enum type, UnityAction action) {
			remove(type.ToString(), action);
		}
		public void remove(string name, UnityAction action) {
			callbacks.remove(name, action);
		}
		public void remove<T0>(Enum type, UnityAction<T0> action) {
			remove(type.ToString(), action);
		}
		public void remove<T0>(string name, UnityAction<T0> action) {
			callbacks.remove(name, action);
		}
		public void remove<T0, T1>(Enum type, UnityAction<T0, T1> action) {
			remove(type.ToString(), action);
		}
		public void remove<T0, T1>(string name, UnityAction<T0, T1> action) {
			callbacks.remove(name, action);
		}
		public void remove<T0, T1, T2>(Enum type, UnityAction<T0, T1, T2> action) {
			remove(type.ToString(), action);
		}
		public void remove<T0, T1, T2>(string name, UnityAction<T0, T1, T2> action) {
			callbacks.remove(name, action);
		}
		public void remove<T0, T1, T2, T3>(Enum type, UnityAction<T0, T1, T2, T3> action) {
			remove(type.ToString(), action);
		}
		public void remove<T0, T1, T2, T3>(string name, UnityAction<T0, T1, T2, T3> action) {
			callbacks.remove(name, action);
		}

		/// <summary>
		/// 唤起回调
		/// </summary>
		/// <param name="type"></param>
		public void on(Enum type, params object[] params_) {
			on(type.ToString(), params_);
		}
		public void on(string name, params object[] params_) {
			callbacks.on(name, params_);
		}
		//public void on(Enum type) {
		//	on(type.ToString());
		//}
		//public void on(string name) {
		//	getCallbackItem(name)?.on();
		//}
		//public void on<T0>(Enum type, T0 p1) {
		//	on(type.ToString(), p1);
		//}
		//public void on<T0>(string name, T0 p1) {
		//	getCallbackItem(name)?.on(p1);
		//}
		//public void on<T0, T1>(Enum type, T0 p1, T1 p2) {
		//	on(type.ToString(), p1, p2);
		//}
		//public void on<T0, T1>(string name, T0 p1, T1 p2) {
		//	getCallbackItem(name)?.on(p1, p2);
		//}
		//public void on<T0, T1, T2>(Enum type, T0 p1, T1 p2, T2 p3) {
		//	on(type.ToString(), p1, p2, p3);
		//}
		//public void on<T0, T1, T2>(string name, T0 p1, T1 p2, T2 p3) {
		//	getCallbackItem(name)?.on(p1, p2, p3);
		//}
		//public void on<T0, T1, T2, T3>(Enum type, T0 p1, T1 p2, T2 p3, T3 p4) {
		//	on(type.ToString(), p1, p2, p3, p4);
		//}
		//public void on<T0, T1, T2, T3>(string name, T0 p1, T1 p2, T2 p3, T3 p4) {
		//	getCallbackItem(name)?.on(p1, p2, p3, p4);
		//}

		/// <summary>
		/// 判断回调
		/// </summary>
		/// <param name="type"></param>
		public bool judge(Enum type) {
			return judge(type.ToString());
		}
		public bool judge(string name) {
			return callbacks.judge(name);
		}

		/// <summary>
		/// 重置
		/// </summary>
		public void reset() {
			foreach (var cb in callbacks) cb.Value.flag = false;
		}

		#endregion

		/// <summary>
		/// 处理值
		/// </summary>
		/// <param name="val"></param>
		protected override void processValue(string val) {
			callbacks.create(val);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public CallbackManager() { }
		public CallbackManager(Type type, params object[] objs) : base(type) {
			foreach (var obj in objs) registerObject(obj);
		}
	}

}
