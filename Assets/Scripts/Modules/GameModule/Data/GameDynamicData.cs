
using System;

using UnityEngine;

using Core.Data;

namespace GameModule.Data {

	using Services;

	/// <summary>
	/// 游戏资料数据标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DynamicData : AutoListFieldSetting {

		/// <summary>
		/// ALF数据
		/// </summary>
		public override Type alfDataType => typeof(GameDynamicData);
	}

	/// <summary>
	/// 游戏动态数据
	/// </summary>
	public class GameDynamicData : AutoListFieldsData<DynamicData> {

		// TODO: 声明自定义动态数据，或者使用[DynamicData]标记

		/// <summary>
		/// 获取实例
		/// </summary>
		public static GameDynamicData Get => DataService.Get().dynamicData;

	}

}