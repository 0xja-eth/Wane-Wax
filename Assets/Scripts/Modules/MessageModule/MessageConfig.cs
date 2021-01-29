
using UnityEngine;

using AssetModule.Services;

/// <summary>
/// 消息模块
/// </summary>
namespace MessageModule {

	/// <summary>
	/// 配置
	/// </summary>
	public static class MessageConfig {

		/// <summary>
		/// 动画资源
		/// </summary>
		public static readonly AssetSetting Bust =
			new AssetSetting("Bust", "Bust_{0}");

		/// <summary>
		/// 下一句话按键判定
		/// </summary>
		public static readonly KeyCode[] nextKeys = new KeyCode[] {
			KeyCode.Return, KeyCode.Space, KeyCode.Mouse0};

		/// <summary>
		/// 获取当前角色BustID
		/// </summary>
		/// <returns></returns>
		public static int currentBustId() {
			// 默认为0
			return 0;
		}
	}

}