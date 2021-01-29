
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
	public abstract class BaseResultProcessor {

		/// <summary>
		/// 目标
		/// </summary>
		protected BaseRuntimeResult result;

		/// <summary>
		/// 快捷访问
		/// </summary>
		protected RuntimeAction action => result.action;
		protected BaseRuntimeBattler subject => action.subject;
		protected BaseRuntimeBattler _object => result._object;
		protected BaseRuntimeBattler[] objects => action.objects;

		/// <summary>
		/// 执行
		/// </summary>
		/// <param name="result"></param>
		public void apply(BaseRuntimeResult result) {
			this.result = result; apply();
		}

		/// <summary>
		/// 执行
		/// </summary>
		protected abstract void apply();
	}

	/// <summary>
	/// 使用效果数据
	/// </summary>
	public abstract class BaseResultProcessor<R> :
		BaseResultProcessor where R : BaseRuntimeResult {

	}

}
