using System;
using System.Collections.Generic;

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
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Grid getGrid(int x, int y) {
			if (!isValidCoord(x, y)) return null;
			return grids[index(x, y)];
		}
		public Grid setGrid(int x, int y, Grid grid = null) {
			if (!isValidCoord(x, y)) return null;
			return grids[index(x, y)] = grid ?? new Grid(x, y);
		}

		/// <summary>
		/// 计算索引
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 构造函数
		/// </summary>
		public Map() { }
		public Map(int mapX, int mapY) { createMap(mapX, mapY); }
	}
}
