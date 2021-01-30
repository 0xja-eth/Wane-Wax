
using Core;

using Core.Systems;
using Core.Components;

using DebugerModule;
using DebugerModule.Controls;
using DebugerModule.Data;

namespace Scenes.GameScene {

	/// <summary>
	/// 游戏场景
	/// </summary>
	public class GameScene : BaseScene {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public MapDisplay mapDisplay;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		Map currentMap;

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			var mapX = DebugerConfig.MapX;
			var mapY = DebugerConfig.MapY;

			currentMap = new Map(mapX, mapY);
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();

			mapDisplay.setItem(currentMap);
		}
	}
}
