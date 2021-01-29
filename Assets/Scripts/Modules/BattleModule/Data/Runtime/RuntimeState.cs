using System;
using System.Collections.Generic;

using Core.Data;

using GameModule.Services;

using ItemModule.Services;

namespace BattleModule.Data {

	///// <summary>
	///// 运行时状态
	///// </summary>
	//public partial class RuntimeState : RuntimeData {

	//	/// <summary>
	//	/// 属性
	//	/// </summary>
	//	[AutoConvert]
	//	public int sType { get; protected set; } // 状态类型
	//	[AutoConvert]
	//	public int stateId { get; protected set; } // 状态ID
	//	[AutoConvert]
	//	public int turns { get; protected set; } // 状态回合

	//	/// <summary>
	//	/// 状态实例
	//	/// </summary>
	//	public BaseState state => ItemService.Get().
	//		getItem(sType, stateId) as BaseState;

	//	/// <summary>
	//	/// 特性
	//	/// </summary>
	//	/// <returns></returns>
	//	public TraitData[] baseTraits => state.baseTraits;

	//	/// <summary>
	//	/// 是否为负面状态
	//	/// </summary>
	//	/// <returns></returns>
	//	public bool isNega() {
	//		return state.isNega;
	//	}

	//	/// <summary>
	//	/// 状态是否过期
	//	/// </summary>
	//	/// <returns></returns>
	//	public bool isOutOfDate() {
	//		return turns <= 0;
	//	}

	//	/// <summary>
	//	/// 回合结束回调
	//	/// </summary>
	//	public void onRoundEnd() {
	//		if (turns > 0) turns--;
	//	}

	//	/// <summary>
	//	/// 移除状态回合
	//	/// </summary>
	//	/// <param name="turns">回合数</param>
	//	public void remove(int turns) {
	//		this.turns -= turns;
	//	}

	//	/// <summary>
	//	/// 增加状态回合
	//	/// </summary>
	//	/// <param name="turns">回合数</param>
	//	public void add(int turns) {
	//		this.turns += turns;
	//		var max = state.maxTurns;
	//		if (max > 0 && this.turns > max)
	//			this.turns = max;
	//	}

	//	/// <summary>
	//	/// 构造函数
	//	/// </summary>
	//	public RuntimeState() { }
	//	public RuntimeState(int sType, int stateId, int turns = 0) {
	//		this.sType = sType; this.stateId = stateId; this.turns = turns;
	//	}
	//	public RuntimeState(BaseState state, int turns = 0)
	//		: this(ItemService.Get().getTypeEnum(
	//			state.GetType()), state.id, turns) { }

	//}

	/// <summary>
	/// 运行时状态
	/// </summary>
	public partial class RuntimeState<S, T> : RuntimeData 
		where S : BaseState<T> where T : TraitData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int stateId { get; protected set; } // 状态ID
		[AutoConvert]
		public int turns { get; protected set; } // 状态回合

		/// <summary>
		/// 状态实例
		/// </summary>
		protected CacheAttr<S> state_ = null;
		protected S _state_() {
			return DataService.Get().get<S>(stateId);
		}
		public S state() {
			return state_?.value();
		}

		/// <summary>
		/// 特性
		/// </summary>
		/// <returns></returns>
		public T[] traits => state().traits;

		/// <summary>
		/// 是否为负面状态
		/// </summary>
		/// <returns></returns>
		public bool isNega() {
			return state().isNega;
		}

		/// <summary>
		/// 状态是否过期
		/// </summary>
		/// <returns></returns>
		public bool isOutOfDate() {
			return turns <= 0;
		}

		/// <summary>
		/// 回合结束回调
		/// </summary>
		public void onRoundEnd() {
			if (turns > 0) turns--;
		}

		/// <summary>
		/// 移除状态回合
		/// </summary>
		/// <param name="turns">回合数</param>
		public void remove(int turns) {
			this.turns -= turns;
		}

		/// <summary>
		/// 增加状态回合
		/// </summary>
		/// <param name="turns">回合数</param>
		public void add(int turns) {
			this.turns += turns;
			var max = state().maxTurns;
			if (max > 0 && this.turns > max)
				this.turns = max;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeState() { }
		public RuntimeState(int stateId, int turns = 0) {
			this.stateId = stateId; this.turns = turns;
		}
		public RuntimeState(S state, int turns = 0)
			: this(state.id, turns) { }

	}

}
