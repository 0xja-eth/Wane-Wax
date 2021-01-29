using System;
using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine.Events;

namespace Core.Utils {

	/// <summary>
	/// 请求项
	/// </summary>
	public class RequestItem {

		/// <summary>
		/// 属性
		/// </summary>
		public StackTrace stack { get; } // 请求回调

		/// <summary>
		/// 初始化
		/// </summary>
		public RequestItem() {
			stack = new StackTrace();
		}
	}

	/// <summary>
	/// 请求管理类（内部请求）
	/// </summary>
	public class RequestManager {

		/// <summary>
		/// 请求字典
		/// </summary>
		DictList<string, RequestItem> requestItems = 
			new DictList<string, RequestItem>();

		/// <summary>
		/// 显示Debug
		/// </summary>
		public bool debug;

		/// <summary>
		/// 构造函数
		/// </summary>
		public RequestManager(bool debug = false) { this.debug = debug; }

		#region 基本操作

		/// <summary>
		/// 请求
		/// </summary>
		/// <param name="type"></param>
		/// <param name="req"></param>
		public void request(string type, RequestItem req = null) {
			requestItems.add(type, req ?? new RequestItem());
		}
		public void request<T>(T req = null) where T: RequestItem {
			request(typeof(T).Name, req);
		}

		/// <summary>
		/// 是否请求
		/// </summary>
		/// <param name="type"></param>
		public bool isRequested(string type) {
			return requestItems.contains(type);
		}
		public bool isRequested<T>() where T : RequestItem {
			return isRequested(typeof(T).Name);
		}

		/// <summary>
		/// 获取请求
		/// </summary>
		/// <param name="type"></param>
		public List<RequestItem> getRequests(string type) {
			var reqs = requestItems.get(type);
			return new List<RequestItem>(reqs);
		}
		public List<T> getRequests<T>() where T : RequestItem {
			var reqs = requestItems.get(typeof(T).Name);

			var res = new List<T>(reqs.Count);
			foreach (var req in reqs) res.Add(req as T);

			return res;
		}

		/// <summary>
		/// 清空请求
		/// </summary>
		/// <param name="type"></param>
		public void clear(string type) {
			requestItems.clear(type);
		}
		public void clear<T>() where T : RequestItem {
			clear(typeof(T).Name);
		}

		/// <summary>
		/// 清除全部
		/// </summary>
		public void clearAll() {
			requestItems.clearAll();
		}

		#endregion

		#region 调用堆栈

		/// <summary>
		/// 获取调用堆栈
		/// </summary>
		/// <param name="type"></param>
		public List<StackTrace> getStackTraces(string type) {
			var reqs = getRequests(type);
			var res = new List<StackTrace>(reqs.Count);
			foreach (var req in reqs) res.Add(req.stack);

			return res;
		}
		public List<StackTrace> getStackTraces<T>() where T : RequestItem {
			return getStackTraces(typeof(T).Name);
		}

		/// <summary>
		/// 生成调用堆栈日志
		/// </summary>
		/// <param name="type"></param>
		public string genStackTracesLog(string type) {
			var stacks = getStackTraces(type);
			return genStackTracesLog(stacks, type);
		}
		public string genStackTracesLog<T>() where T : RequestItem {
			return genStackTracesLog(typeof(T).Name);
		}
		string genStackTracesLog(List<StackTrace> stacks, string title) {
			var logText = title + ": ";
			foreach (var stack in stacks)
				logText += "\n" + stack.ToString();

			return logText;
		}

		public string genStackTraceLog(RequestItem req, string type) {
			return genStackTraceLog(req.stack, type);
		}
		public string genStackTraceLog<T>(T req) where T : RequestItem {
			return genStackTraceLog(req.stack, typeof(T).Name);
		}
		string genStackTraceLog(StackTrace stack, string title) {
			return title + ": \n" + stack.ToString();
		}

		#endregion

		#region 执行

		/// <summary>
		/// 获取请求链表
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		LinkedList<RequestItem> getRawRequests(string type) {
			return requestItems.get(type);
		}
		LinkedList<RequestItem> getRawRequests<T>() where T : RequestItem {
			return getRawRequests(typeof(T).Name);
		}

		/// <summary>
		/// 执行单次请求
		/// </summary>
		public void invokeOnce(string type, UnityAction action,
			UnityAction<string> logFunc = null) {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests(type);
			if (reqs.Count > 0) {
				if (debug) logFunc(genStackTracesLog(type));
				action.Invoke(); reqs.Clear();
			}
		}

		/// <summary>
		/// 执行初次请求
		/// </summary>
		public void invokeFirst<T>(UnityAction<T> action,
			UnityAction<string> logFunc = null) where T : RequestItem {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests<T>();
			if (reqs.Count > 0) {
				var req = reqs.First.Value as T;
				if (debug) logFunc(genStackTracesLog<T>());
				action.Invoke(req); reqs.Clear();
			}
		}

		/// <summary>
		/// 执行最后请求
		/// </summary>
		public void invokeLast<T>(UnityAction<T> action,
			UnityAction<string> logFunc = null) where T : RequestItem {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests<T>();
			if (reqs.Count > 0) {
				var req = reqs.Last.Value as T;
				if (debug) logFunc(genStackTracesLog<T>());
				action.Invoke(req); reqs.Clear();
			}
		}

		/// <summary>
		/// 队列式执行请求
		/// </summary>
		public void invokeQueue(string type, UnityAction action,
			UnityAction<string> logFunc = null) {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests(type);
			if (reqs.Count > 0) {
				var req = reqs.First.Value;
				if (debug) logFunc(genStackTraceLog(req, type));
				action.Invoke(); reqs.RemoveFirst();
			}
		}
		public void invokeQueue<T>(UnityAction<T> action,
			UnityAction<string> logFunc = null) where T : RequestItem {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests<T>();
			if (reqs.Count > 0) {
				var req = reqs.First.Value as T;
				if (debug) logFunc(genStackTraceLog(req));
				action.Invoke(req); reqs.RemoveFirst();
			}
		}

		/// <summary>
		/// 堆栈式执行请求
		/// </summary>
		public void invokeStack(string type, UnityAction action,
			UnityAction<string> logFunc = null) {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests(type);
			if (reqs.Count > 0) {
				var req = reqs.Last.Value;
				if (debug) logFunc(genStackTraceLog(req, type));
				action.Invoke(); reqs.RemoveLast();
			}
		}
		public void invokeStack<T>(UnityAction<T> action,
			UnityAction<string> logFunc = null) where T : RequestItem {
			logFunc = logFunc ?? UnityEngine.Debug.Log;
			var reqs = getRawRequests<T>();
			if (reqs.Count > 0) {
				var req = reqs.Last.Value as T;
				if (debug) logFunc(genStackTraceLog(req));
				action.Invoke(req); reqs.RemoveLast();
			}
		}

		#endregion
    }

}