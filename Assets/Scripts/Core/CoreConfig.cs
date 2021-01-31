using System;
using System.Collections.Generic;

using Core.Systems;

/// <summary>
/// 配置
/// </summary>
namespace Core {

	/// <summary>
	/// 核心配置
	/// </summary>
	public static class CoreConfig {

		/// <summary>
		/// 能否联网
		/// </summary>
		public const bool NetworkEnable = true;

		/// <summary>
		/// 是否自动重连
		/// </summary>
		public const bool AutoReconnect = true;

	}

	/// <summary>
	/// 场景相关
	/// </summary>
	public static class SceneConfig {

		/// <summary>
		/// 游戏场景枚举
		/// </summary>
		public enum Type {

			NoneScene = -1,

			TitleScene = 0,
			GameScene,
		}

		/// <summary>
		/// 游戏是否已开始的标志
		/// </summary>
		/// <returns></returns>
		public static bool isStarted(Type scene) {
			// 默认开始标志为连接状态
			return true; // NetworkSystem.Get().isConnected();
		}

	}
}