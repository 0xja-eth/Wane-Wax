
using System;

using LitJson;

using Core.Data;

using Core.Utils;

/// <summary>
/// 物品模块数据
/// </summary>
namespace BattleModule.Processors {

	using Data;

	/// <summary>
	/// 使用效果数据
	/// </summary>
	public abstract class BaseEffectsProcessor {

		/// <summary>
		/// 行动
		/// </summary>
		protected RuntimeAction action;

		/// <summary>
		/// 目标
		/// </summary>
		protected BaseRuntimeBattler _object;

		/// <summary>
		/// 快捷访问
		/// </summary>
		protected BaseRuntimeBattler subject => action.subject;
		protected BaseRuntimeBattler[] objects => action.objects;

		/// <summary>
		/// 生成结果
		/// </summary>
		/// <returns></returns>
		public BaseRuntimeResult makeResult(RuntimeAction action, BaseRuntimeBattler _object) {
			this.action = action; this._object = _object;
			return makeBaseResult();
		}

		/// <summary>
		/// 生成结果
		/// </summary>
		protected abstract BaseRuntimeResult makeBaseResult();
	}

	/// <summary>
	/// 使用效果数据
	/// </summary>
	public abstract class BaseEffectsProcessor<E, R> :
		BaseEffectsProcessor where E : EffectData where R : BaseRuntimeResult {

		/// <summary>
		/// 生成结果
		/// </summary>
		protected sealed override BaseRuntimeResult makeBaseResult() {
			return makeResult() as BaseRuntimeResult;
		}

		/// <summary>
		/// 生成结果
		/// </summary>
		/// <returns></returns>
		protected abstract R makeResult();
	}

}
