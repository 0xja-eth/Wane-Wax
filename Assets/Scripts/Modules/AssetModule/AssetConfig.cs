
using UnityEngine;

using Core.Data;

/// <summary>
/// 游戏模块
/// </summary>
namespace AssetModule {

	using Services;

	/// <summary>
	/// 资源配置类
	/// </summary>
	public static class AssetConfig {

		/// <summary>
		/// 动画资源
		/// </summary>
		public static readonly AssetSetting Animation = 
			new AssetSetting(AssetService.SystemPath, 
				"Animation", "Animation_{0}");

	}

}