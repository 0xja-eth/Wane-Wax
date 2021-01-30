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

		/// <summary>
		/// 获取方块
		/// </summary>
		public Grid getGrid(int x, int y) {
			if (!isValidCoord(x, y)) return null;
			return grids[index(x, y)];
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

		/// <summary>
		/// 创建地图
		/// </summary>
		public void createMap(int mapX, int mapY) {
			this.mapX = mapX; this.mapY = mapY;
			grids = new Grid[mapX * mapY];

			for (int y = 0; y < mapY; ++y)
				for (int x = 0; x < mapX; ++x) setGrid(x, y);
		}

		#region 格子放置

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

				changeGrid(rx, ry, grids.belong);
				changeGrid(rx, ry, newGridType(preview));
			}
		}

		/// <summary>
		/// 计算新格子的类型
		/// </summary>
		/// <param name="preview"></param>
		/// <returns></returns>
		Grid.Type newGridType(bool preview = false) {
			if (preview) return Grid.Type.Preview;
			return Grid.Type.Normal;
		}

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
			foreach(var grid in grids.realGrids) {
				var rx = grid.x + offsetX; // 实际地图坐标
				var ry = grid.y + offsetY;

				if (judgePointBelong(rx, ry, grids.belong)) return false;
			}
			return true;
		}

		/// <summary>
		/// 判断某点的归属
		/// </summary>
		bool judgePointBelong(int x, int y, Grid.Belong belong) {
			var grid = getGrid(x, y);
			if (grid == null) return false;
			return grid.belong == belong; 
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		public Map() { }
		public Map(int mapX, int mapY) { createMap(mapX, mapY); }
	}
}
