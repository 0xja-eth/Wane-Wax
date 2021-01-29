using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Core.Data;
using Core.Utils;

namespace ShopModule.Data {

	/// <summary>
	/// 货币
	/// </summary>
	public partial class Money : RuntimeData {

		// 在Config中添加货币属性

		#region 反射操作

		/// <summary>
		/// 金钱键
		/// </summary>
		static List<string> _moneyKeys = null;
		static List<string> moneyKeys =>
			fetchPropKeys(ref _moneyKeys, typeof(Money), typeof(int));

		/// <summary>
		/// 金钱项操作
		/// </summary>
		/// <returns></returns>
		Dictionary<string, Func<int>> _getMoneyItems = null;
		Dictionary<string, Func<int>> getMoneyItems => 
			fetchPropGetMethodDict(ref _getMoneyItems);

		Dictionary<string, Action<int>> _setMoneyItems = null;
		Dictionary<string, Action<int>> setMoneyItems =>
			fetchPropSetMethodDict(ref _setMoneyItems);

		/// <summary>
		/// 获取货币值
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int this[string key] {
			get => getMoneyItems[key]?.Invoke() ?? 0;
			set { setMoneyItems[key]?.Invoke(value); }
		}

		#endregion

		#region 运算重载

		/// <summary>
		/// 比较
		/// </summary>
		public static Money operator -(Money m) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = -m[key];
			return res;
		}
		public static Money operator +(Money m1, Money m2) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = m1[key] + m2[key];
			return res;
		}
		public static Money operator -(Money m1, Money m2) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = m1[key] - m2[key];
			return res;
		}
		public static Money operator *(Money m1, Money m2) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = m1[key] * m2[key];
			return res;
		}
		public static Money operator /(Money m1, Money m2) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = m1[key] / m2[key];
			return res;
		}
		public static Money operator *(Money m1, double val) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = (int)Math.Round(m1[key] * val);
			return res;
		}
		public static Money operator /(Money m1, double val) {
			var res = new Money();
			foreach (var key in moneyKeys) res[key] = (int)Math.Round(m1[key] / val);
			return res;
		}
		public static bool operator ==(Money m1, Money m2) {
			return moneyKeys.All(key => m1[key] == m2[key]);
		}
		public static bool operator !=(Money m1, Money m2) {
			return !(m1 == m2);
		}
		public static bool operator >(Money m1, Money m2) {
			return moneyKeys.All(key => m1[key] > m2[key]);
		}
		public static bool operator <(Money m1, Money m2) {
			return moneyKeys.All(key => m1[key] < m2[key]);
		}
		public static bool operator >=(Money m1, Money m2) {
			return moneyKeys.All(key => m1[key] >= m2[key]);
		}
		public static bool operator <=(Money m1, Money m2) {
			return moneyKeys.All(key => m1[key] <= m2[key]);
		}
		
		/// <summary>
		/// 判断相等
		/// </summary>
		/// <param name="obj">对象</param>
		/// <returns>返回是否相等</returns>
		public override bool Equals(object obj) {
			var data = obj as Money;
			return data != null && this == data;
		}

		/// <summary>
		/// 生成哈希码
		/// </summary>
		/// <returns>返回哈希码</returns>
		public override int GetHashCode() {
			var hashCode = 574597825;
			foreach (var key in moneyKeys)
				hashCode = hashCode * -1521134295 + this[key];
			return hashCode;
		}

		#endregion

	}
}
