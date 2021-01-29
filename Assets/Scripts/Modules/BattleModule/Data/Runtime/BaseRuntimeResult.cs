using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

namespace BattleModule.Data {

	using Processors;
	using Services;

	/// <summary>
	/// 行动结果
	/// </summary>
	public abstract class BaseRuntimeResult : RuntimeData {

		/// <summary>
		/// 行动
		/// </summary>
		public RuntimeAction action { get; protected set; }

		/// <summary>
		/// 所属目标
		/// </summary>
		public BaseRuntimeBattler _object { get; protected set; }

		/// <summary>
		/// 构造函数
		/// </summary>
		public BaseRuntimeResult(RuntimeAction action, BaseRuntimeBattler _object) {
			this.action = action; this._object = _object;
		}

		/// <summary>
		/// 效果处理器
		/// </summary>
		/// <returns></returns>		
		BaseResultProcessor _processor;
		BaseResultProcessor processor() {
			if (_processor == null) {
				var pType = BattleService.Get().resultProcessorType(GetType());
				_processor = Activator.CreateInstance(pType) as BaseResultProcessor;
			}
			return _processor;
		}

		/// <summary>
		/// 执行
		/// </summary>
		public void apply() {
			processor()?.apply(this);
			_object.setResult(this);
		}
	}

	///// <summary>
	///// 行动结果
	///// </summary>
	//public abstract class RuntimeResult<E> : RuntimeResult where E : EffectData {

	//	/// <summary>
	//	/// 构造函数
	//	/// </summary>
	//	public RuntimeResult(RuntimeAction action, RuntimeBattler _object) 
	//		: base(action, _object) { }

	//}
}
