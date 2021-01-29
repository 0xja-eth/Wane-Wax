
using System;

using UnityEngine;

using Core.Data;
using LitJson;

namespace GameModule.Data {

	using Services;

	/// <summary>
	/// 游戏资料数据标记
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DatabaseData : AutoListFieldSetting {

		/// <summary>
		/// ALF数据
		/// </summary>
		public override Type alfDataType => typeof(GameDatabase);
	}

	/// <summary>
	/// 游戏资料数据
	/// </summary>
	public class GameDatabase : AutoListFieldsData<DatabaseData> {

		// TODO: 声明自定义资料数据，或者使用[DatabaseData]标记

		/// <summary>
		/// 获取实例
		/// </summary>
		public static GameDatabase Get => DataService.Get().database;

	}

}