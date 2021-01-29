using System;
using System.Reflection;

using UnityEngine;

namespace Core.Utils {

	/// <summary>
	/// 反射工具类
	/// </summary>
	public static class InputUtils {

		/// <summary>
		/// GetKey
		/// </summary>
		public static bool getKey(params KeyCode[] keys) {
			foreach (var key in keys)
				if (Input.GetKey(key)) return true;
			return false;
		}

		/// <summary>
		/// GetKeyDown
		/// </summary>
		public static bool getKeyDown(params KeyCode[] keys) {
			foreach (var key in keys)
				if (Input.GetKeyDown(key)) return true;
			return false;
		}

		/// <summary>
		/// GetKeyUp
		/// </summary>
		public static bool getKeyUp(params KeyCode[] keys) {
			foreach (var key in keys)
				if (Input.GetKeyUp(key)) return true;
			return false;
		}

	}
}
