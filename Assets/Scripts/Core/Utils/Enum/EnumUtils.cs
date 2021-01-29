using System;

namespace Core.Utils {

	/// <summary>
	/// 枚举工具类
	/// </summary>
	public static class EnumUtils {

		/// <summary>
		/// 获取枚举值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T getEnum<T>(int value) where T : Enum {
			if (Enum.IsDefined(typeof(T), value))
				return (T)Enum.ToObject(typeof(T), value);
			return default;
		}

	}
}
