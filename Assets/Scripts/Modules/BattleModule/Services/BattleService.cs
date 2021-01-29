using System;
using System.Collections.Generic;

using Core.Services;
using Core.Utils;

namespace BattleModule.Services {

	using Data;
	using Processors;

	/// <summary>
	/// 战斗服务
	/// </summary>
	public class BattleService : BaseService<BattleService> {

		/// <summary>
		/// 战斗管理器
		/// </summary>
		BaseRuntimeBattle battleManager;

		/// <summary>
		/// 处理器映射表
		/// </summary>
		Dictionary<Type, Type> effectsProcessors = new Dictionary<Type, Type>();
		Dictionary<Type, Type> traitsProcessors = new Dictionary<Type, Type>();
		Dictionary<Type, Type> resultProcessors = new Dictionary<Type, Type>();

		#region 初始化

		/// <summary>
		/// 初始化处理器类型
		/// </summary>
		protected override void initializeProcessors() {
			setupProcessors<BaseEffectsProcessor>(effectsProcessors);
			setupProcessors<BaseTraitsProcessor>(traitsProcessors);
			setupProcessors<BaseResultProcessor>(resultProcessors);
		}

		#endregion

		#region 控制器

		/// <summary>
		/// 效果处理器类
		/// </summary>
		/// <typeparam name="E"></typeparam>
		/// <returns></returns>
		public Type effectsProcessorType<E>() where E : EffectData {
			return effectsProcessorType(typeof(E));
		}
		public Type effectsProcessorType(Type eType) {
			return processorType(eType, effectsProcessors);
		}

		/// <summary>
		/// 特性处理器类
		/// </summary>
		/// <typeparam name="E"></typeparam>
		/// <returns></returns>
		public Type traitsProcessorType<T>() where T : TraitData {
			return traitsProcessorType(typeof(T));
		}
		public Type traitsProcessorType(Type tType) {
			return processorType(tType, traitsProcessors);
		}

		/// <summary>
		/// 效果处理器类
		/// </summary>
		/// <typeparam name="E"></typeparam>
		/// <returns></returns>
		public Type resultProcessorType<R>() where R : BaseRuntimeResult {
			return resultProcessorType(typeof(R));
		}
		public Type resultProcessorType(Type rType) {
			return processorType(rType, resultProcessors);
		}
		
		#endregion

		#region 战斗进程

		/// <summary>
		/// 更新
		/// </summary>
		public override void update() {
			base.update();
			battleManager?.update();
		}

		/// <summary>
		/// 开始战斗
		/// </summary>
		/// <param name="battle"></param>
		public void startBattle(BaseRuntimeBattle battle) {
			battleManager = battle;
			battleManager.start();
		}

		#endregion
	}
}
