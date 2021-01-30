using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using Core.Utils;

/// <summary>
/// 核心系统
/// </summary>
namespace Core.Systems {

	/// <summary>
	/// BaseSystem<>父类
	/// </summary>
	public class BaseSystem : StateMachine {

		/// <summary>
		/// 获取BaseSystem类型的实例
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static BaseSystem getSystemInstance(Type type) {
			if (type.IsAbstract || type.IsGenericType) return null;
			var getFunc = type.GetMethod("Get",
				ReflectionUtils.DefaultStaticFlags);
			return getFunc.Invoke(null, null) as BaseSystem;
		}

		/// <summary>
		/// 初始化标志
		/// </summary>
		public static bool initialized { get; protected set; } = false;
		public bool isInitialized() { return initialized; }

		/// <summary>
		/// 状态类型
		/// </summary>
		protected virtual Type stateType => null;

		/// <summary>
		/// 初始化（只执行一次）
		/// </summary>
		protected void initialize() {
			initialized = true;
			initializeStateMachine();
			initializeSystems();
			initializeOthers();
		}

		/// <summary>
		/// 初始化状态字典
		/// </summary>
		protected virtual void initializeStateMachine() {
			if (stateType == null) return;

			setupType(stateType);
			registerObject(this);
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		void initializeSystems() {
			ReflectionUtils.processMember<FieldInfo, BaseSystem>(
				GetType(), field => field.SetValue(
					this, getSystemInstance(field.FieldType))
				);
		}

		/// <summary>
		/// 其他初始化工作
		/// </summary>
		protected virtual void initializeOthers() { }

		#region 更新控制

		/// <summary>
		/// 更新（每帧）
		/// </summary>
		public override void update() {
			base.update();
			updateOthers();
			updateSystems();
		}

		/// <summary>
		/// 更新外部系统
		/// </summary>
		protected virtual void updateSystems() { }

		/// <summary>
		/// 更新其他
		/// </summary>
		protected virtual void updateOthers() { }

		#endregion
		
	}

	/// <summary>
	/// 业务控制类（父类）（单例模式）
	/// </summary>
	public class BaseSystem<T> : BaseSystem where T : BaseSystem<T>, new() {

		/// <summary>
		/// 多例错误
		/// </summary>
		class MultCaseException : Exception {
			const string ErrorText = "单例模式下不允许多例存在！";
			public MultCaseException() : base(ErrorText) { }
		}

		/// <summary>
		/// 单例函数
		/// </summary>
		protected static T _self;
        public static T Get() {
            if (_self == null) {
                _self = new T();
                _self.initialize();
            }
            return _self;
        }
		
		/// <summary>
		/// 初始化（不能在这里进行初始化操作，必须在initialize中进行）
		/// </summary>
		protected BaseSystem() {
			if (_self != null) throw new MultCaseException();
		}

	}
}