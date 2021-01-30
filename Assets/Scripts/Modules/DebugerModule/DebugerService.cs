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

		#region 初始化

		/// <summary>
		/// 状态类型
		/// </summary>
		protected override Type stateType => typeof(State); 

		#endregion

		#region 游戏控制

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
			base.updateOthers();
			updateMap(); updateEnemy();
		}

		/// <summary>
		/// 更新地图
		/// </summary>
		void updateMap() {
			currentMap.update();
		}

		// TODO: 封装 AI 模块
		float placeTime = 0;
		public float speed { get; protected set; } = 2; // 放置速度 s/个

		/// <summary>
		/// 更新敌人
		/// </summary>
		void updateEnemy() {
			if (enemyGrids == null) enemyGrids = generateGrids(Grid.Belong.Enemy);

			placeGrids(enemyGrids, currentMap.actor.x, true);

			placeTime += Time.deltaTime;
			if (placeTime >= speed) {
				placeGrids(enemyGrids, currentMap.actor.x);
				enemyGrids = null;
				placeTime = 0;
			}
		}

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInputting() {
			placeGrids(currentGrids, (int)mousePos.x, 
				!Input.GetKeyDown(KeyCode.Space));
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
