using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using Core.Services;
using Core.Utils;

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
		public bool isWin => result == Result.Win;
		public bool isLose => result == Result.Lose;

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

			fallingY = currentMap.mapY - 1;

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

			if (!preview) _isPlaced = true;

			if (grids.isPlayer && !preview) changeState(State.Placed);
			if (grids.isEnemy && !preview) enemyGrids = null;
		}
		public void placeGrids(RuntimeGrids grids, int x, int y, bool preview = false) {
			currentMap.placeGrids(grids, x, y, preview);

			if (!preview) _isPlaced = true;

			if (grids.isPlayer && !preview) changeState(State.Placed);
			if (grids.isEnemy && !preview) enemyGrids = null;
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
		/// 放置方块
		/// </summary>
		bool _isPlaced = false;
		public bool isPlaced {
			get {
				var res = _isPlaced;
				_isPlaced = false;
				return res;
			}
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected override void updateOthers() {
			if (pause || isResult) return;

			base.updateOthers();

			updateMap();
			updateEnemy();
			updateClearLines();

			updateResult();

			updateSpeed();
			updateScore();
		}

		/// <summary>
		/// 添加速度
		/// </summary>
		void updateSpeed() {
			if (aiSpeed > 0.1f) aiSpeed -= 0.001f;
			if (clearSpeed > 3) clearSpeed -= 0.002f;
			if (fallSpeed > 0.5f) fallSpeed -= 0.002f;
		}

		/// <summary>
		/// 添加分数
		/// </summary>
		void updateScore() {
			score += (int)Mathf.Round(1 / aiSpeed * 3);
			score = Mathf.Max(score, 0);
		}

		/// <summary>
		/// 更新结果
		/// </summary>
		void updateResult() {
			if (currentMap.isActorLost) result = Result.Lose;
			else if (currentMap.isEnemiesLost) result = Result.Win;

			if (isResult) {
				currentMap.clearAll();
				changeState(State.Result);
			}
		}

		/// <summary>
		/// 更新地图
		/// </summary>
		void updateMap() {
			currentMap.update();
		}

		// TODO: 封装 AI 模块
		float placeTime = 0;
		public float aiSpeed { get; protected set; } = 1; // 放置速度 s/个

		float clearTime = 0;
		public float clearSpeed { get; protected set; } = 7; // 清除速度 s/个

		/// <summary>
		/// 更新敌人
		/// </summary>
		void updateEnemy() {
			if (enemyGrids == null) {
				enemyGrids = generateGrids(Grid.Belong.Enemy);
				fallingY2 = 0;
			}

			placeTime += Time.deltaTime;
			if (placeTime >= aiSpeed) {
				var targetX = currentMap.actor.nextX;
				var dist = targetX - fallingX2;

				if (dist > 0) fallingX2++;
				else fallingX2--;

				var w = currentMap.mapX;
				fallingX2 = (w + fallingX2) % w;

				placeTime = 0;

				if (fallingX2 == targetX)
					placeGrids(enemyGrids, fallingX2);
			}

			if (enemyGrids == null) return;
			//var down = InputUtils.getKeyDown(KeyCode.DownArrow, KeyCode.S);
			//if (down) placeGrids(enemyGrids, fallingX2);
			if (!currentMap.isPlacePointValid(enemyGrids, fallingX2, fallingY2 + 1))
				placeGrids(enemyGrids, fallingX2, fallingY2);
			else
				placeGrids(enemyGrids, fallingX2, fallingY2, true);
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

		int fallingX, fallingY;
		int fallingX2, fallingY2;

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInputting() {
			var hor = Input.GetAxisRaw("Horizontal");
			//var ver = Input.GetAxisRaw("Vertical");
			var down = InputUtils.getKeyDown(KeyCode.DownArrow, KeyCode.S);
			var rotate = InputUtils.getKeyDown(KeyCode.UpArrow, KeyCode.W, KeyCode.Space);

			if (hor > 0) fallingX++; if (hor < 0) fallingX--;

			var w = currentMap.mapX; fallingX = (w + fallingX) % w;

			if (rotate) currentGrids.rotate();
			if (down) placeGrids(currentGrids, fallingX);
			else if (!currentMap.isPlacePointValid(currentGrids, fallingX, fallingY - 1))
				placeGrids(currentGrids, fallingX, fallingY);
			else
				placeGrids(currentGrids, fallingX, fallingY, true);
		}

		/// <summary>
		/// 降落
		/// </summary>
		float fallTime = 0;
		public float fallSpeed { get; protected set; } = 1.5f; // 放置速度 s/格

		/// <summary>
		/// 更新降落
		/// </summary>
		void updateFalling() {
			fallTime += Time.deltaTime;
			if (fallTime >= fallSpeed) {
				fallTime = 0; fallingY--; fallingY2++;
			}
		}

		/// <summary>
		/// 更新游戏开始
		/// </summary>
		void _updateStart() {
			fallingX = fallingX2 = currentMap.actor.x;

			generateNextGrids();
		}

		/// <summary>
		/// 更新放置状态
		/// </summary>
		void _updatePlacing() {
			updateInputting();
			updateFalling();
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
