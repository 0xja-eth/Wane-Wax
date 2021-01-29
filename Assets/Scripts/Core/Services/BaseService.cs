using System;
using System.Collections.Generic;
using System.Reflection;

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

/// <summary>
/// 核心服务
/// </summary>
namespace Core.Services {

	using Utils;
	using Systems;
	using Components.SceneFramework;

	/// <summary>
	/// 类型枚举值
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TypeEnumValue : Attribute {

		public int value = 0; // 枚举值

		public TypeEnumValue(int value) { this.value = value; }
	}

	/// <summary>
	/// 业务类接口
	/// </summary>
	public interface IBaseService {

		/// <summary>
		/// 能否联网
		/// </summary>
		bool networkEnable { get; }

		/// <summary>
		/// 是否在线/是否连接
		/// </summary>
		bool isOnline { get; }
		bool isConnected { get; }

	}

	/// <summary>
	/// 业务控制类（父类）（单例模式）
	/// </summary>
	public abstract partial class BaseService<T> : BaseSystem<T>, 
		IBaseService where T : BaseService<T>, new() {

		/// <summary>
		/// 操作文本设定
		/// </summary>
		public const string WaitTextFormat = "{0}中";
		public const string FailTextFormat = "{0}失败：\n{{0}}";
		
		/// <summary>
		/// 外部系统
		/// </summary>
		NetworkSystem networkSys;
		protected GameSystem gameSys;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOthers() {
			base.initializeOthers();
			initializeProcessors();
			initializeElements();
		}

		/// <summary>
		/// 初始化模块元素
		/// </summary>
		protected virtual void initializeElements() { }

		/// <summary>
		/// 初始化模块处理器
		/// </summary>
		protected virtual void initializeProcessors() { }

		#endregion

		#region 模块设置
		// TODO: 改用单例模式，避免使用反射

		/// <summary>
		/// 服务名
		/// </summary>
		public string serviceName => Regex.Replace(GetType().Name, "Service", "");

		public string moduleNamespace => Regex.Replace(GetType().Namespace, @"\.Services", "");
		public string moduleName => Regex.Replace(moduleNamespace, "Module", "");

		public string configName => serviceName + "Config";
		public string moduleConfigName => moduleName + "Config";

		/// <summary>
		/// 是否模块服务
		/// </summary>
		public bool isModuleService => serviceName == moduleName;

		/// <summary>
		/// 配置类型
		/// </summary>
		public virtual Type configType => getTypeUnderModule(configName);
		public virtual Type moduleConfigType => getTypeUnderModule(moduleConfigName);

		/// <summary>
		/// 获取模块下的指定类
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected Type getTypeUnderModule(string name) {
			var types = ReflectionUtils.getNamespaceTypes(moduleNamespace);

			foreach (var type in types) if (type.Name == name) return type;

			return null;
		}

		/// <summary>
		/// 配置访问
		/// </summary>
		/// <typeparam name="TV"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public TV getConfig<TV>(string key) {
			return (TV)configType?.GetField(key).GetValue(null);
		}
		public object getConfig(string key) {
			return configType?.GetField(key).GetValue(null);
		}
		public TV getModuleConfig<TV>(string key) {
			return (TV)moduleConfigType?.GetField(key).GetValue(null);
		}
		public object getModuleConfig(string key) {
			return moduleConfigType?.GetField(key).GetValue(null);
		}

		/// <summary>
		/// 网络是否有效
		/// </summary>
		/// <returns></returns>
		public virtual bool networkEnable =>
			getConfig<bool?>("NetworkEnable") ?? CoreConfig.NetworkEnable;

		/// <summary>
		/// 是否在线
		/// </summary>
		public bool isOnline => networkEnable && networkSys.online;
		public bool isConnected => isOnline && networkSys.isConnected();

		#endregion

		#region 模块注册工具函数

		/// <summary>
		/// 初始化系统元素
		/// </summary>
		/// <typeparam name="P">元素的父类</typeparam>
		/// <typeparam name="E">对应的枚举类</typeparam>
		/// <param name="dict">枚举-类对应关系字典</param>
		protected static void setupSystemElement<P, E>(
			Dictionary<int, Type> dict, UnityAction<Type> func = null) {
			setupSystemElement(typeof(P), typeof(E), dict, func);
		}
		/// <param name="tType">对应的枚举类</param>
		protected static void setupSystemElement<P>(Type tType, 
			Dictionary<int, Type> dict, UnityAction<Type> func = null) {
			setupSystemElement(typeof(P), tType, dict, func);
		}
		/// <param name="tType">元素的父类</param>
		protected static void setupSystemElement(Type pType, Type tType, 
			Dictionary<int, Type> dict, UnityAction<Type> func = null) {
			var iTypes = ReflectionUtils.getNamespaceTypes(parent: pType);

			foreach (var type in iTypes) {
				var ev = type.GetCustomAttribute<TypeEnumValue>();
				// 如果类型指定了枚举值
				if (ev != null) dict[ev.value] = type;
				else {
					// 根据类名获取对应的值
					var field = tType.GetField(
						type.Name, ReflectionUtils.DefaultStaticFlags);
					if (field == null) continue;

					var val = (int)field.GetValue(null);
					dict[val] = type;
				}
				func?.Invoke(type); // 拓展
			}
		}

		/// <summary>
		/// 获取类型
		/// </summary>
		/// <param name="dict">枚举-类对应关系字典</param>
		/// <param name="value">枚举值</param>
		/// <returns>枚举值对应的类型</returns>
		public static Type getType(Dictionary<int, Type> dict, int value) {
			return dict.ContainsKey(value) ? dict[value] : null;
		}

		/// <summary>
		/// 获取类型枚举
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dict">枚举-类对应关系字典</param>
		protected static int getTypeEnum<T>(Dictionary<int, Type> dict) {
			return getTypeEnum(dict, typeof(T));
		}
		/// <param name="dict">枚举-类对应关系字典</param>
		/// <param name="type">类型</param>
		/// <returns>类型对应的枚举值</returns>
		protected static int getTypeEnum(Dictionary<int, Type> dict, Type type) {
			foreach (var item in dict)
				if (item.Value == type) return item.Key;
			return -1;
		}

		#endregion

		#region 生成器/处理器模型

		/// <summary>
		/// 初始化处理器
		/// </summary>
		protected static void setupProcessors<P>(Dictionary<Type, Type> map) {
			var types = ReflectionUtils.getNamespaceTypes(parent: typeof(P));

			foreach (var type in types) {
				var gType = ReflectionUtils.findGenericParentType(type);
				gType = gType?.GetGenericArguments()[0];

				if (gType != null) map[gType] = type;
			}
		}

		/// <summary>
		/// 获取处理器类型
		/// </summary>
		/// <param name="eType"></param>
		/// <returns></returns>
		protected static Type processorType(Type eType, Dictionary<Type, Type> map) {
			return map.ContainsKey(eType) ? map[eType] : null;
		}

		#endregion

		#region 请求控制

		/// <summary>
		/// 发送请求（带有重试功能）
		/// </summary>
		/// <param name="key">操作字典键（枚举类型）</param>
		/// <param name="data">数据</param>
		/// <param name="onSuccess">成功回调</param>
		/// <param name="onError">失败回调</param>
		/*
		protected void sendRequest(Enum key, JsonData data = null,
			NetworkSystem.RequestObject.SuccessAction onSuccess = null, UnityAction onError = null, 
			string waitFormat = WaitTextFormat, string failFormat = FailTextFormat) {
			sendRequest(key.GetHashCode(), data, onSuccess, onError, waitFormat, failFormat);
		}
		protected void sendRequest(int key, JsonData data = null,
			NetworkSystem.RequestObject.SuccessAction onSuccess = null, UnityAction onError = null, 
			string waitFormat = WaitTextFormat, string failFormat = FailTextFormat) {
			if (operDict.ContainsKey(key)) 
				sendRequest(operDict[key], data, onSuccess, onError, waitFormat, failFormat);
			else Debug.LogError("未找到操作键 " + key + "，请检查操作字典");
		}
		*/
		/// <param name="oper">操作实例</param>
		protected void sendRequest(Interface oper, JsonData data = null,
			NetworkSystem.RequestObject.SuccessAction onSuccess = null, UnityAction onError = null,
			string waitFormat = WaitTextFormat, string failFormat = FailTextFormat) {
			if (oper == null) return;
			sendRequest(oper.name, oper.route, data, onSuccess, onError,
				waitFormat, failFormat, oper.emit);
		}
		/// <param name="oper">操作文本</param>
		/// <param name="route">路由</param>
		/// <param name="uid">是否需要携带玩家信息</param>
		/// <param name="emit">是否为发射操作</param>
		protected void sendRequest(string oper, string route, JsonData data = null,
			NetworkSystem.RequestObject.SuccessAction onSuccess = null,
			UnityAction onError = null, string waitFormat = WaitTextFormat,
			string failFormat = FailTextFormat, bool emit = false) {
			sendRequest(route, data, string.Format(waitFormat, oper),
				string.Format(failFormat, oper), onSuccess, onError, emit);
		}
		/// <param name="waitText">等待文本</param>
		/// <param name="failText">错误文本</param>
		protected void sendRequest(string route, JsonData data = null,
			string waitText = "", string failText = "",
			NetworkSystem.RequestObject.SuccessAction onSuccess = null,
			UnityAction onError = null, bool emit = false) {
			NetworkSystem.RequestObject.ErrorAction _onError = generateOnErrorFunc(failText,
				() => sendRequest(route, data, waitText, failText,
					onSuccess, onError, emit), onError);
			networkSys.setupRequest(route, data, onSuccess, _onError, true, waitText, emit);
		}

		/// <summary>
		/// 生成实际的 onError 函数
		/// </summary>
		/// <param name="format">文本格式</param>
		/// <param name="retry">重试函数</param>
		/// <param name="onError">失败回调</param>
		/// <returns>onError 函数</returns>
		NetworkSystem.RequestObject.ErrorAction generateOnErrorFunc(string format,
			UnityAction retry, UnityAction onError = null) {
			return (status, errmsg) => {
				var text = string.Format(format, errmsg);
				var type = AlertWindow.Type.Notice;
				gameSys.requestAlert(text, type);//, retry, onError);
				onError?.Invoke();
			};
		}

		#endregion

	}
}
