
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
		public ScoreDisplay scoreDisplay, winScore, loseScore;
		public LifeDisplay playerLife, enemyLife;

		public GameObject pauseLayer;

		public Animation winResult, loseResult;
		public Animation cameraAni;

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
			if (!debugSer.pause && !debugSer.isResult)
				debugSer.update();

			updateResult(); updateUI();
		}

		/// <summary>
		/// 更新UI
		/// </summary>
		void updateUI() {
			if (currentMap == null) return;

			scoreDisplay.score.text = debugSer.score.ToString();

			playerLife.setValue(currentMap.actor);
			enemyLife.setValue(currentMap.enemies[0]);
		}

		/// <summary>
		/// 更新结果
		/// </summary>
		void updateResult() {
			if (debugSer.isWin) updateWin();
			if (debugSer.isLose) updateLose();
		}

		string resultAni = "ResultAni";
		string rotateAni = "RotateAni";

		bool rotateFlag = false; // 静态
		bool resultFlag = false; // 静态

		/// <summary>
		/// 胜利
		/// </summary>
		void updateWin() {
			if (resultFlag) return;

			winScore.score.text = debugSer.score.ToString();

			winResult.gameObject.SetActive(true);
			winResult.Play(resultAni);

			resultFlag = true;
		}

		/// <summary>
		/// 失败
		/// </summary>
		void updateLose() {
			if (resultFlag) return;

			if (cameraAni.IsPlaying(rotateAni)) return;
			if (rotateFlag) {
				loseScore.score.text = debugSer.score.ToString();

				loseResult.gameObject.SetActive(true);
				loseResult.Play(resultAni);

				resultFlag = true;
			} else {
				rotateFlag = true; cameraAni.Play(rotateAni);
			}
		}

		/// <summary>
		/// 暂停/继续
		/// </summary>
		public void togglePause() {
			debugSer.pause = !debugSer.pause;
			pauseLayer.SetActive(debugSer.pause);
		}
	}
}
