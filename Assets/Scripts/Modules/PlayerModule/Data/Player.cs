
using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using LitJson;

using Core.Data;
using Core.Systems;

using InfoModule.Data;

using ExerComps.Controls.ParamDisplays;

/// <summary>
/// 信息模块数据
/// </summary>
namespace PlayerModule.Data {

	using Services;

	/// <summary>
	/// 玩家数据（一个存档的数据，包含角色数据、存档数据、关卡状态等）
	/// </summary>
	public partial class Player : BaseData,
		ParamDisplay.IDisplayDataConvertable {

		/// <summary>
		/// 获取实例
		/// </summary>
		public static Player Get => PlayerService.Get().player;

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string uid { get; protected set; }

        [AutoConvert]
        public bool first { get; set; } = true;

		// 请在 Config 中添加自定义属性

		/// <summary>
		/// 转化为显示数据
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public JsonData convertToDisplayData(string type = "") {
			return toJson();
		}

		/// <summary>
		/// 生成随机UID
		/// </summary>
		string generateUid() {
			return GameSystem.Get().uuid;
			//return Random.Range(0, 99999).ToString();
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name"></param>
		public Player() { }
		public Player(bool create = false) {
			if (create) uid = generateUid();
		}
	}
}