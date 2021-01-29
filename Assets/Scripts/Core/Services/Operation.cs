using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine.Events;

using LitJson;

namespace Core.Services {

	using Data;
	using Utils;

	/// <summary>
	/// 接口
	/// </summary>
	public class Interface {

		/// <summary>
		/// 数据
		/// </summary>
		public string name;
		public string route;
		public bool emit;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">操作名</param>
		/// <param name="route">路由</param>
		/// <param name="emit">是否为发射操作</param>
		public Interface(string name, string route, bool emit = false) {
			this.name = name; this.route = route;
			this.emit = emit;
		}
	}

	/// <summary>
	/// 业务控制类（父类）（单例模式）
	/// </summary>
	public abstract partial class BaseService<T> {

		/// <summary>
		/// 操作（抽象类型）
		/// </summary>
		public abstract class Operation : RuntimeData {

			/// <summary>
			/// 服务
			/// </summary>
			public T service => Get();

			/// <summary>
			/// 自定义回调函数
			/// </summary>
			UnityAction _onSuccess, _onError;

			#region 配置

			/// <summary>
			/// 对应的接口（自动获取配置类中相同名称的Interface对象）
			/// </summary>
			public virtual Interface interface_ =>
				service.getConfig<Interface>(GetType().Name);

			/// <summary>
			/// 是否支持服务/本地端
			/// </summary>
			public bool isServerEnable => interface_ != null;
			public virtual bool isLocalEnable => true;

			protected virtual string waitFormat => WaitTextFormat;
			protected virtual string failFormat => FailTextFormat;

			#endregion

			#region 调用

			/// <summary>
			/// 处理
			/// </summary>
			public void invoke(UnityAction onSuccess = null, UnityAction onError = null) {
				// 配置，通过重载自定义配置
				_onSuccess = onSuccess; _onError = onError;

				doInvoke(); // 执行
			}
			void doInvoke() {
				try {
					// TODO: 异常处理：改为函数内部抛出异常
					if (!isValid()) return;

					preprocess();

					if (isServerEnable && service.isConnected) doServer();
					else if (isLocalEnable) doLocal();
					else throw new Exception("操作不成功"); // TODO: 异常处理

				} catch { // TODO: 异常处理
					onError();
				}
			}
			// 通过重载 invoke 函数添加参数处理

			/// <summary>
			/// 预处理
			/// </summary>
			protected virtual void preprocess() { }

			/// <summary>
			/// 条件判断
			/// </summary>
			/// <returns></returns>
			protected virtual bool isValid() { return true; }

			/// <summary>
			/// 执行服务器请求
			/// </summary>
			void doServer() {
				setupRequestParams();

				service.sendRequest(interface_,
					toJson(), onSuccess, onError, 
					waitFormat, failFormat);
			}

			/// <summary>
			/// 配置请求参数
			/// </summary>
			protected virtual void setupRequestParams() { }

			/// <summary>
			/// 执行本地请求
			/// </summary>
			void doLocal() {
				processLocal();
				invokeOnSuccess();
			}

			/// <summary>
			/// 处理本地请求
			/// </summary>
			protected virtual void processLocal() {
				_onSuccess?.Invoke();
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			void onSuccess(JsonData data) {
				processSuccess(data);
				invokeOnSuccess();
			}

			/// <summary>
			/// 处理成功结果
			/// </summary>
			protected virtual void processSuccess(JsonData data) { }

			/// <summary>
			/// 执行成功回调（最终调用）
			/// </summary>
			protected virtual void invokeOnSuccess() {
				_onSuccess?.Invoke();
			}

			/// <summary>
			/// 失败回调
			/// </summary>
			/// <param name="data"></param>
			void onError() {
				processError();
				invokeOnError();
			}

			/// <summary>
			/// 处理失败结果
			/// </summary>
			/// <param name="data"></param>
			protected virtual void processError() { }

			/// <summary>
			/// 执行失败回调（最终调用）
			/// </summary>
			protected virtual void invokeOnError() {
				_onError?.Invoke();
			}

			#endregion

		}

		///// <summary>
		///// 操作（继承、重载，并使用构造函数来配置参数）
		///// </summary>
		//public abstract class Operation<O> : Operation
		//	where O : Operation<O>, new() {

		//	/// <summary>
		//	/// 直接调用
		//	/// </summary>
		//	/// <param name="params_"></param>
		//	public static bool Invoke(params object[] params_) {
		//		if (params_.Length <= 0) return new O().invoke();

		//		var oper = Activator.CreateInstance(typeof(O), params_) as O;

		//		return oper.invoke();
		//	}
		//}
	}

}
