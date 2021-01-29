using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

namespace Core.Utils {

	/// <summary>
	/// 内部定义的UnityEvent
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ExerEvent : UnityEvent { }
	public class ExerEvent<T0> : UnityEvent<T0> { }
	public class ExerEvent<T0, T1> : UnityEvent<T0, T1> { }
	public class ExerEvent<T0, T1, T2> : UnityEvent<T0, T1, T2> { }
	public class ExerEvent<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3> { }

	/// <summary>
	/// 回调设置
	/// </summary>
	public abstract class BaseCallbackItem<T> {

		/// <summary>
		/// 最大参数数量
		/// </summary>
		const int MaxParamCount = 4;

		/// <summary>
		/// 回调名称
		/// </summary>
		protected T name;

		/// <summary>
		/// 处理函数（立刻） Type[] → UnityEvent
		/// </summary> 
		protected List<Tuple<Type[], UnityEventBase>> events
			= new List<Tuple<Type[], UnityEventBase>>();

		/// <summary>
		/// 函数委托映射（缓存）
		/// </summary>
		protected Dictionary<Tuple<object, MethodInfo>, Delegate> actions
			= new Dictionary<Tuple<object, MethodInfo>, Delegate>();

		/// <summary>
		/// 回调发生标志
		/// </summary>
		public bool flag = false;

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="name"></param>
		public void setup(T name) { this.name = name; }

		/// <summary>
		/// 函数名
		/// </summary>
		protected virtual string methodName => "";

		#region 对象管理

		/// <summary>
		/// 注册对象
		/// </summary>
		/// <param name="object_"></param>
		public void registerObject(object obj) {
			processObject(obj, true);
		}

		/// <summary>
		/// 移除对象
		/// </summary>
		/// <param name="object_"></param>
		public void removeObject(object obj) {
			processObject(obj, false);
		}

		/// <summary>
		/// 处理函数
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="action"></param>
		void processObject(object obj, bool add) {
			if (methodName == "") return;

			var oType = obj.GetType();
			var methods = oType.GetMethods(ReflectionUtils.DefaultFlags);

			foreach (var method in methods) {
				if (method.Name != methodName) continue;
				processMethod(obj, method, add);
			}
		}

		#endregion

		#region 委托管理

		/// <summary>
		/// 创建委托
		/// </summary>
		/// <param name="method"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		Delegate createAction(object obj, MethodInfo method) {
			var params_ = method.GetParameters();
			if (params_.Length > MaxParamCount) return null;

			var types = CallbackManager.getTypes(params_);
			return createAction(obj, method, types);
		}
		Delegate createAction(object obj, MethodInfo method, Type[] types) {
			var aType = getActionType(types);
			var action = method.CreateDelegate(aType, obj);

			return addAction(obj, method, action);
		}

		/// <summary>
		/// 记录委托
		/// </summary>
		/// <param name="method"></param>
		/// <param name="obj"></param>
		/// <param name="action"></param>
		Delegate addAction(object obj, MethodInfo method, Delegate action) {
			var key = new Tuple<object, MethodInfo>(obj, method);
			return actions[key] = action;
		}

		/// <summary>
		/// 获取委托
		/// </summary>
		/// <param name="method"></param>
		/// <param name="obj"></param>
		/// <param name="action"></param>
		Delegate getAction(object obj, MethodInfo method) {
			var key = new Tuple<object, MethodInfo>(obj, method);
			if (actions.ContainsKey(key)) return actions[key];
			return createAction(obj, method);
		}

		#endregion

		#region 注册/移除

		/// <summary>
		/// 注册函数
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="action"></param>
		void registerMethod(object obj, MethodInfo method) {
			processMethod(obj, method, true);
		}

		/// <summary>
		/// 移除函数
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="action"></param>
		void removeMethod(object obj, MethodInfo method) {
			processMethod(obj, method, false);
		}

		/// <summary>
		/// 获取事件和委托
		/// </summary>
		void getEventNAction(object obj, MethodInfo method,
			out Delegate action, out UnityEventBase event_) {
			var params_ = method.GetParameters();
			if (params_.Length > MaxParamCount)
				throw new Exception("参数超出限制"); // TODO: 封装异常模块

			var types = CallbackManager.getTypes(params_);

			action = getAction(obj, method);
			event_ = getEvent(types);
		}

		/// <summary>
		/// 处理函数
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="action"></param>
		void processMethod(object obj, MethodInfo method, bool add) {
			Delegate action; UnityEventBase event_;
			getEventNAction(obj, method, out action, out event_);
			processAction(event_, action, add);
		}

		/// <summary>
		/// 处理委托
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="action"></param>
		void processAction(UnityEventBase event_, Delegate action, bool add) {
			var eType = event_.GetType();
			var method = add ? "AddListener" : "RemoveListener";
			var process = eType.GetMethod(method);

			process.Invoke(event_, new object[] { action }); // 添加/删除回调函数
		}

		#endregion

		#region 事件管理

		/// <summary>
		/// 获取事件
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		UnityEventBase getEvent(Type[] types, bool create = true) {
			var tuple = events.Find(t => isTypesEquals(t.Item1, types));

			if (tuple == null && create) {
				var eType = getEventType(types);
				var event_ = Activator.CreateInstance(eType) as UnityEventBase;

				events.Add(tuple = new Tuple<Type[], UnityEventBase>(types, event_));
			}

			return tuple?.Item2;
		}
		UnityEventBase getEvent(Type eType, bool create = true) {
			return getEvent(eType.GetGenericArguments(), create);
		}
		E getEvent<E>(bool create = true) where E : UnityEventBase, new() {
			return getEvent(typeof(E), create) as E;
		}

		#endregion

		#region 工具函数

		/// <summary>
		/// 获取委托类型
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		static Type getActionType(Type[] types) {
			switch (types.Length) {
				case 0: return typeof(UnityAction);
				case 1: return typeof(UnityAction<>).MakeGenericType(types);
				case 2: return typeof(UnityAction<,>).MakeGenericType(types);
				case 3: return typeof(UnityAction<,,>).MakeGenericType(types);
				case 4: return typeof(UnityAction<,,,>).MakeGenericType(types);
				default: return null;
			}
		}

		/// <summary>
		/// 获取事件类型
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		static Type getEventType(Type[] types) {
			switch (types.Length) {
				case 0: return typeof(ExerEvent);
				case 1: return typeof(ExerEvent<>).MakeGenericType(types);
				case 2: return typeof(ExerEvent<,>).MakeGenericType(types);
				case 3: return typeof(ExerEvent<,,>).MakeGenericType(types);
				case 4: return typeof(ExerEvent<,,,>).MakeGenericType(types);
				default: return null;
			}
		}

		/// <summary>
		/// 类型是否相等
		/// </summary>
		static bool isTypesEquals(Type[] types1, Type[] types2) {
			if (types1.Length != types2.Length) return false;
			for (int i = 0; i < types1.Length; ++i)
				if (types2[i] != types1[i]) return false;
			return true;
		}

		#endregion

		#region 单独注册

		/// <summary>
		/// 注册回调
		/// </summary>
		/// <param name="action"></param>
		public void register(Delegate action) {
			if (action == null) return;

			var obj = action.Target;
			var method = action.Method;

			registerMethod(obj, method);
		}
		public void register<T0>(UnityAction<T0> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0>>();
			event_.AddListener(action);
		}
		public void register<T0, T1>(UnityAction<T0, T1> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0, T1>>();
			event_.AddListener(action);
		}
		public void register<T0, T1, T2>(UnityAction<T0, T1, T2> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0, T1, T2>>();
			event_.AddListener(action);
		}
		public void register<T0, T1, T2, T3>(UnityAction<T0, T1, T2, T3> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0, T1, T2, T3>>();
			event_.AddListener(action);
		}

		/// <summary>
		/// 注销回调
		/// </summary>
		/// <param name="action"></param>
		public void remove(Delegate action) {
			if (action == null) return;

			var obj = action.Target;
			var method = action.Method;

			removeMethod(obj, method);
		}
		public void remove<T0>(UnityAction<T0> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0>>();
			event_.RemoveListener(action);
		}
		public void remove<T0, T1>(UnityAction<T0, T1> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0, T1>>();
			event_.RemoveListener(action);
		}
		public void remove<T0, T1, T2>(UnityAction<T0, T1, T2> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0, T1, T2>>();
			event_.RemoveListener(action);
		}
		public void remove<T0, T1, T2, T3>(UnityAction<T0, T1, T2, T3> action) {
			if (action == null) return;
			var event_ = getEvent<ExerEvent<T0, T1, T2, T3>>();
			event_.RemoveListener(action);
		}

		#endregion

		#region 调用

		/// <summary>
		/// 唤起
		/// </summary>
		public void on(params object[] params_) {
			var types = CallbackManager.getTypes(params_);
			var eType = getEventType(types);
			var invoke = eType.GetMethod("Invoke", types);

			var event_ = getEvent(types);

			invoke?.Invoke(event_, params_);

			onInvoked();
		}
		public void on() {
			var event_ = getEvent<ExerEvent>();
			event_?.Invoke();
			onInvoked();
		}
		public void on<T0>(T0 p1) {
			var event_ = getEvent<ExerEvent<T0>>();
			event_?.Invoke(p1);
			onInvoked();
		}
		public void on<T0, T1>(T0 p1, T1 p2) {
			var event_ = getEvent<ExerEvent<T0, T1>>();
			event_?.Invoke(p1, p2);
			onInvoked();
		}
		public void on<T0, T1, T2>(T0 p1, T1 p2, T2 p3) {
			var event_ = getEvent<ExerEvent<T0, T1, T2>>();
			event_?.Invoke(p1, p2, p3);
			onInvoked();
		}
		public void on<T0, T1, T2, T3>(T0 p1, T1 p2, T2 p3, T3 p4) {
			var event_ = getEvent<ExerEvent<T0, T1, T2, T3>>();
			event_?.Invoke(p1, p2, p3, p4);
			onInvoked();
		}

		/// <summary>
		/// 调用后
		/// </summary>
		void onInvoked() {
			flag = true;
		}

		#endregion
	}
}
