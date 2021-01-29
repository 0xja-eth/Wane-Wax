using System;
using System.Collections.Generic;

using Core.Data;

namespace BattleModule.Data {

	/// <summary>
	/// 运行时BUFF
	/// </summary>
	public class RuntimeBuff : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int paramId { get; protected set; } // 属性ID
		[AutoConvert]
		public double value { get; protected set; } // 改变数值
		[AutoConvert]
		public double rate { get; protected set; } // 改变比率
		[AutoConvert]
		public int turns { get; protected set; } // BUFF回合

		/// <summary>
		/// 是否为Debuff
		/// </summary>
		/// <returns></returns>
		public bool isDebuff() {
			return value < 0 || rate < 1;
		}

		/// <summary>
		/// BUFF是否过期
		/// </summary>
		/// <returns></returns>
		public bool isOutOfDate() {
			return turns == 0;
		}

		/// <summary>
		/// 回合结束回调
		/// </summary>
		public void onRoundEnd() {
			if (turns > 0) turns--;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeBuff() { }
		public RuntimeBuff(int paramId,
			double value = 0, double rate = 1, int turns = 0) {
			this.paramId = paramId; this.value = value;
			this.rate = rate; this.turns = turns;
		}

	}
}
