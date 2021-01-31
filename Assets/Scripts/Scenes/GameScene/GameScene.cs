
using UnityEngine;

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
		public ScoreDisplay scoreDisplay;
		public LifeDisplay playerLife, enemyLife;

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
			//if (!debugSer.pause || debugSer.isResult)
			debugSer.update();

			updateResult(); updateUI();
		}

		/// <summary>
		/// 更新UI
		/// </summary>
		void updateUI() {
			if (currentMap == null) return;

			scoreDisplay.setValue(debugSer.score);

			playerLife.setValue(currentMap.actor);
			enemyLife.setValue(currentMap.enemies[0]);
		}

		/// <summary>
		/// 更新结果
		/// </summary>
		void updateResult() {
			if (debugSer.isWin) onWin();
			if (debugSer.isLose) onLose();
		}

		/// <summary>
		/// 胜利
		/// </summary>
		void onWin() {
			Debug.Log("Win");
		}

		/// <summary>
		/// 失败
		/// </summary>
		void onLose() {
			Debug.Log("Lost");
		}

		/// <summary>
		/// 暂停/继续
		/// </summary>
		public void togglePause() {
			debugSer.pause = !debugSer.pause;
		}
	}
}
