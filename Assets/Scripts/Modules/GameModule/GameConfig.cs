
using UnityEngine;

using Core.Data;
using Core.Services;

/// <summary>
/// 游戏模块
/// </summary>
namespace GameModule {

	/// <summary>
	/// 模块配置类
	/// </summary>
	public class GameConfig {

	}

	/// <summary>
	/// 模块配置类
	/// </summary>
	public class DataConfig {

		/// <summary>
		/// 业务操作
		/// </summary>
		public static Interface GetStatic = new Interface("初始化数据", "game/load/static");
		public static Interface GetDynamic = new Interface("初始化数据", "game/load/dynamic");
		//public static Interface Refresh = new Interface("刷新数据", "game/load/dynamic");

	}

}

namespace GameModule.Data {

	/// <summary>
	/// 游戏系统配置数据（在这里添加自定义属性）
	/// </summary>
	public partial class GameConfigure {

		/// <summary>
		/// 配置项
		/// </summary>

	}

	/// <summary>
	/// 游戏设定数据（在这里添加自定义属性）
	/// </summary>
	public partial class GameSettings {

		/// <summary>
		/// 设置项
		/// </summary>
		[AutoConvert]
		public string rememberPassword { get; set; } = null; // 记住密码
		[AutoConvert]
		public string rememberUsername { get; set; } = null; // 记住账号
		[AutoConvert]
		public bool autoLogin { get; set; } = false; // 自动登录
	}

}