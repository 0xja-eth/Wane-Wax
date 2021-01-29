
using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

/// <summary>
/// 信息模块数据
/// </summary>
namespace InfoModule.Data {

	/// <summary>
	/// 游戏数据变量
	/// </summary>
	public class Info : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public DictionaryData<Switches, bool> switches { get; protected set; }
			= new DictionaryData<Switches, bool>(false);
		[AutoConvert]
		public DictionaryData<Variables, float> variables { get; protected set; }
			= new DictionaryData<Variables, float>(0);
		
		/// <summary>
		/// 获取开关值
		/// </summary>
		public bool getSwitch(Switches type) {
			return switches[type];
		}

		/// <summary>
		/// 设置开关值
		/// </summary>
		public bool setSwitch(Switches type, bool val) {
			return switches[type] = val;
		}

		/// <summary>
		/// 获取变量值
		/// </summary>
		public float getVariable(Variables type) {
			return variables[type];
		}

		/// <summary>
		/// 获取开关值
		/// </summary>
		public float setVariable(Variables type, float val) {
			return variables[type] = val;
		}
	}

}