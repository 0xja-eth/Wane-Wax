using System;
using System.Collections.Generic;

using AssetModule.Services;

namespace DebugerModule {

	/// <summary>
	/// 配置
	/// </summary>
	public class DebugerConfig {

		/// <summary>
		/// 战斗者资源
		/// </summary>
		public static readonly AssetSetting Battler =
			new AssetSetting("Battler", "Battler_{0}");

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public const int MapX = 28;
		public const int MapY = 16;
	}
}
