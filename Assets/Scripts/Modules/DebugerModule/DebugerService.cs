using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using Core.Services;

namespace DebugerModule.Services {

	using Data;

	/// <summary>
	/// 游戏服务
	/// </summary>
	public class DebugerService : BaseService<DebugerService> {

		/// <summary>
		/// 地图
		/// </summary>
		public Map currentMap;

		/// <summary>
		/// 方块
		/// </summary>
		public RuntimeGrids currentGrids;
		public RuntimeGrids nextGrids;

		public RuntimeGrids enemyGrids;

		/// <summary>
		/// 状态
		/// </summary>
		public enum State {
			None, // 默认
			Start, // 游戏开始
			Placing, // 放置方块中
			Placed, // 方块放置完毕
			Result // 结果
		}

		/// <summary>
		/// 结果
		/// </summary>
		public enum Result {
			None, Win, Lose
		}

		/// <summary>
		/// 结果
		/// </summary>
		public Result result = Result.None;

		#region 初始化

		/// <summary>
		/// 状态类型
		/// </summary>
		protected override Type stateType => typeof(State);

		#endregion

		#region 游戏控制

		/// <summary>
		/// 暂停游戏
		/// </summary>
		public bool pause { get; set; }

		/// <summary>
		/// 分数
		/// </summary>
		public int score { get; set; }

		/// <summary>
		/// 有结果
		/// </summary>
		public bool isResult => result != Result.None;
		public bool isWin => result != Result.Win;
		public bool isLose => result != Result.Lose;

		/// <summary>
		/// 游戏开始
		/// </summary>
		public void start() {
			changeState(State.Start);

			var mapX = DebugerConfig.MapX;
			var mapY = DebugerConfig.MapY;

			currentMap = new Map(mapX, mapY);
			currentMap.addActor();
			currentMap.addEnemy();
		}

		/// <summary>
		/// 生成下一个格子
		/// </summary>
		void generateNextGrids() {
			currentGrids = nextGrids ?? generateGrids();
			nextGrids = generateGrids();

			changeState(State.Placing);
		}

		/// <summary>
		/// 生成格子
		/// </summary>
		/// <returns></returns>
		public RuntimeGrids generateGrids(Grid.Belong operer = Grid.Belong.Player) {
			var gridsSet = Grids.GridsSet;
			var grids = gridsSet[Random.Range(0, gridsSet.Length)];

			return new RuntimeGrids(grids, operer);
		}

		/// <summary>
		/// 放置方块
		/// </summary>
		/// <param name="operer"></param>
		public void placeGrids(RuntimeGrids grids, int x, bool preview = false) {
			currentMap.placeGrids(grids, x, preview);

			if (grids.isPlayer && !preview) changeState(State.Placed);
		}

		#endregion

		#region 输入

		/// <summary>
		/// 鼠标位置（在地图上的位置）
		/// </summary>
		public Vector2 mousePos {
			get {
				var mPos = Input.mousePosition;
				mPos = Camera.main.ScreenToWorldPoint(mPos);

				mPos.x += currentMap.mapX / 2f;
				mPos.y += currentMap.mapY / 2f;

				return mPos;
			}
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void updateOthers() {
			if (pause || isResult) return;

			base.updateOthers();

			updateMap();
			updateEnemy();
			updateClearLines();

			updateSpeed();
			updateScore();
		}

		/// <summary>
		/// 添加速度
		/// </summary>
		void updateSpeed() {
			if (aiSpeed > 1) aiSpeed -= 0.001f;
			if (clearSpeed > 3) clearSpeed -= 0.001f;
		}

		/// <summary>
		/// 添加分数
		/// </summary>
		void updateScore() {
			score += (int)Mathf.Round(1 / aiSpeed * 3);
		}

		/// <summary>
		/// 更新结果
		/// </summary>
		void updateResult() {
			if (currentMap.isActorLost) result = Result.Lose;
			else if (currentMap.isEnemiesLost) result = Result.Win;
		}

		/// <summary>
		/// 更新地图
		/// </summary>
		void updateMap() {
			currentMap.update();
		}

		// TODO: 封装 AI 模块
		float placeTime = 0;
		public float aiSpeed { get; protected set; } = 3; // 放置速度 s/个

		float clearTime = 0;
		public float clearSpeed { get; protected set; } = 7; // 清除速度 s/个

		/// <summary>
		/// 更新敌人
		/// </summary>
		void updateEnemy() {
			if (enemyGrids == null) enemyGrids = generateGrids(Grid.Belong.Enemy);

			placeGrids(enemyGrids, currentMap.actor.x, true);

			placeTime += Time.deltaTime;
			if (placeTime >= aiSpeed) {
				placeGrids(enemyGrids, currentMap.actor.x);
				enemyGrids = null;
				placeTime = 0;
			}
		}

		/// <summary>
		/// 更新敌人
		/// </summary>
		void updateClearLines() {
			clearTime += Time.deltaTime;
			if (clearTime >= clearSpeed) {
				var rand = Random.Range(0f, 1f);
				if (rand > 0.5f)
					currentMap.clearLines(currentMap.mapY >> 1);
				else
					currentMap.clearLines((currentMap.mapY >> 1) - 1);
				clearTime = 0;
			}
		}

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInputting() {
			if (Input.GetKeyDown(KeyCode.Mouse1)) currentGrids.rotate();

			placeGrids(currentGrids, (int)mousePos.x, !Input.GetKeyDown(KeyCode.Mouse0));
		}

		/// <summary>
		/// 更新游戏开始
		/// </summary>
		void _updateStart() {
			generateNextGrids();
		}

		/// <summary>
		/// 更新放置状态
		/// </summary>
		void _updatePlacing() {
			updateInputting();
		}

		/// <summary>
		/// 更新放置后状态
		/// </summary>
		void _updatePlaced() {
			generateNextGrids();
		}

		#endregion
	}
}
