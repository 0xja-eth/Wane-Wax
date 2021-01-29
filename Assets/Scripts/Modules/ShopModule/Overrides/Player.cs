
using System;
using System.Collections.Generic;

using UnityEngine;

using ShopModule.Data;

/// <summary>
/// 信息模块数据
/// </summary>
namespace PlayerModule.Data {

	/// <summary>
	/// 玩家数据（一个存档的数据，包含角色数据、存档数据、关卡状态等）
	/// </summary>
	public partial class Player {

		/// <summary>
		/// 金钱
		/// </summary>
		public Money money { get; protected set; }

		/// <summary>
		/// 获得金钱
		/// </summary>
		/// <param name="money"></param>
		public bool gainMoney(Money money) {
			var zero = new Money();
			var newMoney = this.money + money;

			var res = newMoney >= zero;
			if (res) this.money = newMoney;

			return res;
		}

		/// <summary>
		/// 失去金钱
		/// </summary>
		/// <param name="money"></param>
		public bool lostMoney(Money money) {
			return gainMoney(-money);
		}
	}
}