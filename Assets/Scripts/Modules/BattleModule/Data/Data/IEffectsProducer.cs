using System;
using System.Collections.Generic;

namespace BattleModule.Data {

	/// <summary>
	/// 效果生成接口
	/// </summary>
	public partial interface IEffectsProducer {

		/// <summary>
		/// 消耗品
		/// </summary>
		bool consumable { get; }

		/// <summary>
		/// 目标类型
		/// </summary>
		int target { get; }

		/// <summary>
		/// 效果数组
		/// </summary>
		EffectData[] baseEffects { get; }
	}

	/// <summary>
	/// 效果生成接口
	/// </summary>
	public partial interface IEffectsProducer<E> : 
		IEffectsProducer where E : EffectData {

		/// <summary>
		/// 效果数组
		/// </summary>
		E[] effects { get; }
	}
}
