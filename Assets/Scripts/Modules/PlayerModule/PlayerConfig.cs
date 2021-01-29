
using UnityEngine;

using Core.Data;
using Core.Services;

/// <summary>
/// 玩家模块
/// </summary>
namespace PlayerModule {

	using Services;
	using Data;

	/// <summary>
	/// 配置
	/// </summary>
	public static class PlayerConfig {

		/// <summary>
		/// 操作
		/// </summary>
		public static Interface Login = new Interface("登陆", "player/player/login");
		public static Interface Logout = new Interface("登出", "player/player/logout");
		public static Interface Save = new Interface("保存", "player/player/save");

		/// <summary>
		/// 存档数量
		/// </summary>
		public const int MaxSaveCount = 1;

	}
}

/// <summary>
/// 玩家模块数据
/// </summary>
namespace PlayerModule.Data {

	using InfoModule.Data;

	/// <summary>
	/// 玩家数据（在这里添加自定义属性）
	/// </summary>
	public partial class Player {

		/// <summary>
		/// InfoModule
		/// </summary>
		[AutoConvert]
		public Info info { get; protected set; } = new Info();
		
	}
}
