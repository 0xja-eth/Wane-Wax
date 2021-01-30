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
			Normal, Preview
		}

		/// <summary>
		/// 方块归属
		/// </summary>
		public enum Belong {
			None, Player, Enemy
		}

		/// <summary>
		/// 反向
		/// </summary>
		public static Belong opposite(Belong belong) {
			if (belong == Belong.Player) return Belong.Enemy;
			if (belong == Belong.Enemy) return Belong.Player;
			return belong;
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
			= Belong.None; // belong 是指当前方块的可通行方（空气）

		/// <summary>
		/// 地图
		/// </summary>
		public Map map { get; protected set; }

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

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public int mapX => map.mapX;
		public int mapY => map.mapY;

		/// <summary>
		/// 位置领域
		/// </summary>
		public Belong posBelong => map == null ? Belong.None :
			(y >= mapY >> 1 ? Belong.Player : Belong.Enemy);

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
		/// 改变方块类型
		/// </summary>
		/// <param name="type"></param>
		public void changeType(Type type) {
			this.type = type;
			_refreshRequest = true;
		}

		/// <summary>
		/// 改变方块归属
		/// </summary>
		/// <param name="type"></param>
		public void changeBelong(Belong belong) {
			this.belong = belong;
			_refreshRequest = true;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Grid() { }
		public Grid(Belong belong) { setup(belong); }
		public Grid(int x, int y, Map map = null) {
			this.map = map; setup(x, y);
		}
	}
}
