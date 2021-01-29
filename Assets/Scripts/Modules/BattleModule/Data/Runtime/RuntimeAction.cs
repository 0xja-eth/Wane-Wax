using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

namespace BattleModule.Data {

	using Processors;
	using Services;

	/// <summary>
	/// 行动
	/// </summary>
	public class RuntimeAction : RuntimeData {

		/// <summary>
		/// 发起者
		/// </summary>
		public BaseRuntimeBattler subject { get; protected set; } 

		/// <summary>
		/// 目标数组
		/// </summary>
		public new BaseRuntimeBattler[] objects { get; protected set; }

		/// <summary>
		/// 效果数组
		/// </summary>
		public EffectData[] effects { get; protected set; }

		/// <summary>
		/// 结果数组
		/// </summary>
		public BaseRuntimeResult[] results { get; protected set; }

		/// <summary>
		/// 效果类型
		/// </summary>
		public Type effectType => effects.GetType().GetElementType();

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeAction() { }
		public RuntimeAction(BaseRuntimeBattler subject, BaseRuntimeBattler[] objects, EffectData[] effects) {
			this.subject = subject; this.objects = objects; this.effects = effects;
		}
		public RuntimeAction(BaseRuntimeBattler subject, BaseRuntimeBattler[] objects, IEffectsProducer producer)
			: this(subject, objects, producer.baseEffects) { }

		/// <summary>
		/// 效果处理器
		/// </summary>
		/// <returns></returns>		
		BaseEffectsProcessor _processor;
		BaseEffectsProcessor processor() {
			if (_processor == null) {
				var pType = BattleService.Get().effectsProcessorType(effectType);
				_processor = Activator.CreateInstance(pType) as BaseEffectsProcessor;
			}
			return _processor;
		}

		/// <summary>
		/// 生成结果
		/// </summary>
		public void makeResults() {
			var processor = this.processor();

			results = new BaseRuntimeResult[objects.Length];
			for(var i = 0; i < objects.Length; ++i) 
				results[i] = processor?.makeResult(this, objects[i]);
		}

		/// <summary>
		/// 执行效果
		/// </summary>
		public void applyResults() {
			foreach (var result in results) result.apply();
		}
	}
}
