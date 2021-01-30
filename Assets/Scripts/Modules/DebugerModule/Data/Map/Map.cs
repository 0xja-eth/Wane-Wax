using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 地图
	/// </summary>
	public class Map : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int mapX { get; protected set; }
		[AutoConvert]
		public int mapY { get; protected set; }
		[AutoConvert]
		public Grid[] grids { get; protected set; }
		[AutoConvert]
		public RuntimeActor actor { get; protected set; }
		[AutoConvert]
		public List<RuntimeEnemy> enemies { get; protected set; } = new List<RuntimeEnemy>();

		/// <summary>
		/// 战斗者
		/// </summary>
		public List<RuntimeBattler> battlers {
			get {
				var res = new List<RuntimeBattler>(enemies.Count + 1);
				res.Add(actor); foreach (var enemy in enemies) res.Add(enemy);
				return res;
			}
		}

		/// <summary>
		/// 刷新请求
		/// </summary>
		bool _refreshRequest;
		public bool refreshRequest {
			get {
				var res = _refreshRequest;
				_refreshRequest = false;
				return res;
			}
		}

		#region 方块控制

		/// <summary>
		/// 获取方块
		/// </summary>
		public Grid getGrid(int x, int y) {
			if (!isValidCoord(x, y)) return null;
			return grids[index(x, y)];
		}
		public Grid getGridIgnoreY(int x, int y) {
			y = Mathf.Clamp(y, 0, mapY - 1);
			return getGrid(x, y);
		}
		public Grid getGridIgnoreX(int x, int y) {
			x = Mathf.Clamp(x, 0, mapX - 1);
			return getGrid(x, y);
		}
		public Grid getGridLoopX(int x, int y) {
			x = (mapX + x) % mapX;
			y = Mathf.Clamp(y, 0, mapY - 1);
			return getGrid(x, y);
		}
		public Grid setGrid(int x, int y, Grid grid = null) {
			if (!isValidCoord(x, y)) return null;
			return grids[index(x, y)] = grid ?? new Grid(x, y, this);
		}

		/// <summary>
		/// 改变方块
		/// </summary>
		public void changeGrid(int x, int y, Grid.Belong belong) {
			getGrid(x, y)?.changeBelong(belong);
		}
		public void changeGrid(int x, int y, Grid.Type type) {
			getGrid(x, y)?.changeType(type);
		}

		/// <summary>
		/// 计算索引
		/// </summary>
		int index(int x, int y) { return y * mapX + x; }

		/// <summary>
		/// 坐标是否有效
		/// </summary>
		public bool isValidCoord(int x, int y) {
			return x >= 0 && x < mapX && y >= 0 && y < mapY;
		}
		
		#region 格子放置

		/// <summary>
		/// 预览位置
		/// </summary>
		List<Vector2> previewPos = new List<Vector2>();

		/// <summary>
		/// 放置格子
		/// </summary>
		/// <param name="grids">格子组</param>
		/// <param name="x">位置</param>
		/// <param name="belong">性质</param>
		public void placeGrids(RuntimeGrids grids, int x, bool preview = false) {
			var _pp = getPlacePoint(grids, x);
			if (_pp == null) return;
			var pp = (Vector2)_pp;

			var rGrid = grids.rootGrid;
			var offsetX = (int)pp.x - rGrid.x;
			var offsetY = (int)pp.y - rGrid.y;

			// 遍历每个方块
			foreach (var grid in grids.realGrids) {
				var rx = grid.x + offsetX; // 实际地图坐标
				var ry = grid.y + offsetY;

				if (preview)
					previewPos.Add(new Vector2(rx, ry));
				else {
					changeGrid(rx, ry, grids.belong);
					changeGrid(rx, ry, newGridType());
					_refreshRequest = true;
				}
			}
		}

		/// <summary>
		/// 计算新格子的类型
		/// </summary>
		/// <param name="preview"></param>
		/// <returns></returns>
		Grid.Type newGridType() { return Grid.Type.Normal; }

		/// <summary>
		/// 获取放置点
		/// </summary>
		/// <param name="grids"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		Vector2? getPlacePoint(RuntimeGrids grids, int x) {
			Vector2? res = null;
			switch (grids.operer) {
				case Grid.Belong.Player:
					for (int y = mapY - 1; y >= 0; --y) {
						if (!isPlacePointValid(grids, x, y)) break;
						res = new Vector2(x, y);
					}
					break;
				case Grid.Belong.Enemy:
					for (int y = 0; y < mapY; ++y) {
						if (!isPlacePointValid(grids, x, y)) break;
						res = new Vector2(x, y);
					}
					break;
			}
			if (res == null && x > 0) // 找不到放置点
				return getPlacePoint(grids, x - 1);

			return res;
		}

		/// <summary>
		/// 判断放置点是否合法
		/// </summary>
		/// <param name="grids"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool isPlacePointValid(RuntimeGrids grids, int x, int y) {
			var rGrid = grids.rootGrid;
			var offsetX = x - rGrid.x;
			var offsetY = y - rGrid.y;

			// 遍历每个方块
			foreach (var grid in grids.realGrids) {
				var rx = grid.x + offsetX; // 实际地图坐标
				var ry = grid.y + offsetY;

				if (!isPointValid(rx, ry, grids.belong)) return false;
			}
			return true;
		}

		/// <summary>
		/// 判断某点的归属
		/// </summary>
		bool isPointValid(int x, int y, Grid.Belong belong) {
			var grid = getGridIgnoreY(x, y);
			if (grid == null) return false;
			return grid.belong != belong;
		}

		///// <summary>
		///// 判断某点的归属
		///// </summary>
		//bool judgePointBelong(int x, int y, Grid.Belong belong) {
		//	var grid = getGrid(x, y);
		//	if (grid == null) return false;
		//	return grid.belong == belong;
		//}

		#endregion

		#endregion

		#region 地图控制

		/// <summary>
		/// 创建地图
		/// </summary>
		public void createMap(int mapX, int mapY) {
			this.mapX = mapX; this.mapY = mapY;
			grids = new Grid[mapX * mapY];

			for (int y = 0; y < mapY; ++y)
				for (int x = 0; x < mapX; ++x) setGrid(x, y);
		}

		/// <summary>
		/// 获取坠落高度
		/// </summary>
		/// <returns></returns>
		public int getFallingY(int x, int y, Grid.Belong belong) {
			var grid = getGridLoopX(x, y);

			if (grid.belong == belong) {
				// 向下试探
				do {
					if (belong == Grid.Belong.Player
						&& --y < 0) return -mapY;
					if (belong == Grid.Belong.Enemy
						&& ++y >= mapY) return 2 * mapY;
					grid = getGridLoopX(x, y);
				} while (grid.belong == belong);

				return belong == Grid.Belong.Player ? y + 1 : y - 1;
			}
			return getUpperY(x, y, belong);
		}

		/// <summary>
		/// 获取提升高度（被方块覆盖时）
		/// </summary>
		/// <returns></returns>
		public int getUpperY(int x, int y, Grid.Belong belong) {
			var grid = getGridLoopX(x, y);

			if (grid.belong == belong) return y;
			// 向上试探
			do {
				if (belong == Grid.Belong.Player
					&& ++y >= mapY) return 2 * mapY;
				if (belong == Grid.Belong.Enemy
					&& --y < 0) return -mapY;
				grid = getGridLoopX(x, y);
			} while (grid.belong != belong);

			return y;
		}

		/// <summary>
		/// 判断前方墙的高度
		/// </summary>
		public int judgeWall(int x, int y, Grid.Belong belong) {
			switch(belong) {
				case Grid.Belong.Player:
					return judgePlayerWall(x, y);
				case Grid.Belong.Enemy:
					return judgeEnemyWall(x, y);
			}
			return 2;
		}
		int judgePlayerWall(int x, int y) {
			var wy = y;
			var belong = Grid.Belong.Player;
			var grid = getGridLoopX(x, y);
			// 空气
			if (grid.belong == belong) {
				// 向上试探一格（防止上面还有）
				grid = getGridLoopX(x, y + 1);
				if (grid.belong != belong) return 2;

				// 向下试探
				do {
					if (--wy < 0) return -mapY;
					grid = getGridLoopX(x, wy);
				} while (grid.belong == belong);

				return wy - y + 1;
			} else {
				// 查看头顶
				grid = getGridLoopX(x - 1, y + 2);
				if (grid.belong != belong) return 2;

				// 向上试探
				grid = getGridLoopX(x, y + 1);
				if (grid.belong != belong) return 2;

				// 再向上试探一格（防止上面还有）
				grid = getGridLoopX(x, y + 2);
				if (grid.belong != belong) return 2;

				return 1;
			}
		}
		int judgeEnemyWall(int x, int y) {
			var wy = y;
			var belong = Grid.Belong.Enemy;
			var grid = getGridLoopX(x, y);
			// 空气
			if (grid.belong == belong) {
				// 向上试探一格（防止上面还有）
				grid = getGridLoopX(x, y - 1);
				if (grid.belong != belong) return 2;

				// 向下试探
				do {
					if (++wy >= mapY) return -mapY;
					grid = getGridLoopX(x, wy);
				} while (grid.belong == belong);

				return y - wy + 1;
			} else {
				// 查看头顶
				grid = getGridLoopX(x - 1, y - 2);
				if (grid.belong != belong) return 2;

				// 向上试探
				grid = getGridLoopX(x, y - 1);
				if (grid.belong != belong) return 2;
				
				// 再向上试探
				grid = getGridLoopX(x, y - 2);
				if (grid.belong != belong) return 2;

				return 1;
			}
		}

		/// <summary>
		/// 消除行方块
		/// </summary>
		/// <param name="y"></param>
		public void clearLines(int y) {
			int yy = y;
			var belong = (y >= mapY >> 1) ? 
				Grid.Belong.Player : Grid.Belong.Enemy;
			switch (belong) {
				case Grid.Belong.Player:
					for (; yy < mapY - 1; ++yy)
						for (int x = 0; x < mapX; ++x) {
							var grid = getGrid(x, yy);
							var uGrid = getGrid(x, yy + 1);

							grid.changeBelong(uGrid.belong);
							grid.changeType(uGrid.type);
						}
					break;

				case Grid.Belong.Enemy:
					for (; yy > 0; --yy)
						for (int x = 0; x < mapX; ++x) {
							var grid = getGrid(x, yy);
							var uGrid = getGrid(x, yy - 1);

							grid.changeBelong(uGrid.belong);
							grid.changeType(uGrid.type);
						}
					break;
			}

			for (int x = 0; x < mapX; ++x) {
				var grid = getGrid(x, yy);

				grid?.changeBelong(belong);
				grid?.changeType(Grid.Type.Normal);
			}

			_refreshRequest = true;
		}

		#endregion

		#region 成员控制

		/// <summary>
		/// 添加战斗者
		/// </summary>
		/// <param name="battler"></param>
		public void addActor(int x, int y) {
			actor = new RuntimeActor(this, x, y);
		}
		public void addActor() {
			actor = new RuntimeActor(this);
		}
		public void addEnemy(int x, int y) {
			enemies.Add(new RuntimeEnemy(this, x, y));
		}
		public void addEnemy() {
			enemies.Add(new RuntimeEnemy(this));
		}

		#endregion

		/// <summary>
		/// 更新
		/// </summary>
		public void update() {
			updateGrids();
			updateBattlers();
			updatePlacingGrids();
		}

		/// <summary>
		/// 更新格子状态
		/// </summary>
		void updateGrids() {
			foreach (var grid in grids) grid.preview = false;
		}

		/// <summary>
		/// 更新战斗者状态
		/// </summary>
		void updateBattlers() {
			foreach (var battler in battlers) battler?.update();
		}

		/// <summary>
		/// 更新放置
		/// </summary>
		void updatePlacingGrids() {
			foreach(var pos in previewPos) {
				var grid = getGrid((int)pos.x, (int)pos.y);
				if (grid != null) grid.preview = true;
			}
			previewPos.Clear();
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Map() { }
		public Map(int mapX, int mapY) { createMap(mapX, mapY); }
	}
}
