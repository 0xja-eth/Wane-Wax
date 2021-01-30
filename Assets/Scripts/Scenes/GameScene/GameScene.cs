
using Core;

using Core.Systems;
using Core.Components;

using DebugerModule;
using DebugerModule.Controls;
using DebugerModule.Services;
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
		/// 服务
		/// </summary>
		DebugerService debugSer;

		/// <summary>
		/// 当前地图
		/// </summary>
		Map currentMap => debugSer.currentMap;

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
			base.start();

			debugSer.start();
			mapDisplay.setItem(currentMap);
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			debugSer.update();
		}
	}
}
