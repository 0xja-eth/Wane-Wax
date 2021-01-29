using System;

using Core.Services;

/// <summary>
/// 计算模块服务
/// </summary>
namespace Core.Calculations {

	/// <summary>
	/// 公用计算函数
	/// </summary>
	public static class CommonCalc {

		/// <summary>
		/// sigmoid函数
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static double sigmoid(double x) {
			return 1 / (1 + Math.Exp(-x));
		}
	}

}
