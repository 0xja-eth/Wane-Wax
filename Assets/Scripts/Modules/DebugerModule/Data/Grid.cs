using System;
using System.Collections.Generic;

using Core.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 格子
	/// </summary>
	public class Grid : RuntimeData {

		/// <summary>
		/// 方块类型
		/// </summary>
		public enum Type {
			Normal
		}

		/// <summary>
		/// 方块归属
		/// </summary>
		public enum Belong {
			Player, Enemy
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int x { get; protected set; }
		[AutoConvert]
		public int y { get; protected set; }
		[AutoConvert]
		public Type type { get; protected set; } = Type.Normal;
		[AutoConvert]
		public Belong belong { get; protected set; }

		/// <summary>
		/// 地图
		/// </summary>
		public Map map { get; protected set; }

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public int mapX => map.mapX;
		public int mapY => map.mapY;

		/// <summary>
		/// 位置领域
		/// </summary>
		public Belong posBelong => y >= mapY >> 1 ? Belong.Player : Belong.Enemy;

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void setup(int x, int y) {
			this.x = x; this.y = y;
			belong = posBelong;
		}
		void setup(Belong belong) {
			this.belong = belong;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Grid() { }
		public Grid(Belong belong) { setup(belong); }
		public Grid(int x, int y) { setup(x, y); }
	}
}
