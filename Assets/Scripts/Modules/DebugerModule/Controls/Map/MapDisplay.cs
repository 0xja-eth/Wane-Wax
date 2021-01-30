using System;
using System.Collections.Generic;

using UnityEngine;

using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Controls {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	[RequireComponent(typeof(GridContainer))]
	[RequireComponent(typeof(MapLineContainer))]
	[RequireComponent(typeof(BattleGround))]
	public class MapDisplay : ItemDisplay<Map> {

		/// <summary>
		/// 线段
		/// </summary>
		public class Line {

			/// <summary>
			/// 两点
			/// </summary>
			public Vector2 p1, p2;
			public bool flag = false;

			/// <summary>
			/// 另一个点
			/// </summary>
			/// <param name="p"></param>
			/// <returns></returns>
			public Vector2? other(Vector2 p) {
				if (p1 == p) return p2;
				if (p2 == p) return p1;
				return null;
			}

			/// <summary>
			/// 相交点
			/// </summary>
			/// <param name="l"></param>
			/// <returns></returns>
			public Vector2? interact(Line l) {
				if (p1 == l.p1) return p1;
				if (p1 == l.p2) return p1;
				if (p2 == l.p1) return p2;
				if (p2 == l.p2) return p2;
				return null;
			}
			public Vector2? interact(Vector2 p) {
				if (p1 == p) return p1;
				if (p2 == p) return p2;
				return null;
			}

			/// <summary>
			/// 构造函数
			/// </summary>
			public Line(Vector2 p1, Vector2 p2) {
				this.p1 = p1; this.p2 = p2;
			}

			/// <summary>
			/// ToString
			/// </summary>
			/// <returns></returns>
			public override string ToString() {
				return "(" + p1 + ", " + p2 + ")";
			}
		}

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		[HideInInspector]
		public GridContainer gridContainer;
		[RequireTarget]
		[HideInInspector]
		public MapLineContainer linesContainer;
		[RequireTarget]
		[HideInInspector]
		public BattleGround battleground;

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public int mapX => item?.mapX ?? 0;
		public int mapY => item?.mapY ?? 0;

		/// <summary>
		/// 线段数据
		/// </summary>
		List<Line> rawLines = new List<Line>();
		List<List<Line>> paintableLines = new List<List<Line>>();

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (item == null) return;
			if (item.refreshRequest)
				setupLines();
		}

		#endregion

		#region 数据

		/// <summary>
		/// 获取格子实际位置
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Vector2 getGridPosition(float x, float y) {
			return new Vector2(x - mapX / 2f + 0.5f, y - mapY / 2f + 0.5f);
		}
		public Vector2 getGridPosition(Vector2 pos) {
			return getGridPosition(pos.x, pos.y);
		}
		public Vector2 getGridCoord(float x, float y) {
			return new Vector2(x - 0.5f + mapX / 2f, y - 0.5f + mapY / 2f);
		}
		public Vector2 getGridCoord(Vector2 pos) {
			return getGridCoord(pos.x, pos.y);
		}

		/// <summary>
		/// 获取战斗者实际位置
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Vector2 getBattlerPosition(float x, float y) {
			return new Vector2(x - mapX / 2f + 0.5f, y - mapY / 2f);
		}
		public Vector2 getBattlerPosition(Vector2 pos) {
			return getBattlerPosition(pos.x, pos.y);
		}
		public Vector2 getBattlerCoord(float x, float y) {
			return new Vector2(x - 0.5f + mapX / 2f, y + mapY / 2f);
		}
		public Vector2 getBattlerCoord(Vector2 pos) {
			return getBattlerCoord(pos.x, pos.y);
		}

		/// <summary>
		/// 地图改变
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();

			setupLines();

			gridContainer.setItems(item?.grids);
			battleground.setItems(item.battlers);
		}

		/// <summary>
		/// 计算线段
		/// </summary>
		void setupLines() {
			generateLines();

			linesContainer.setItems(paintableLines);
		}

		/// <summary>
		/// 计算线段
		/// </summary>
		void generateLines() {
			clearLines();
			generateRawLines();
			makePaintableLines();
		}

		/// <summary>
		/// 生成原始分离线段
		/// </summary>
		void generateRawLines() {
			Grid lastGrid;

			// 由下至上扫描
			for (int x = 0; x < mapX; ++x) {
				lastGrid = new Grid(Grid.Belong.Enemy);
				for (int y = 0; y < mapY; ++y) {
					var grid = item.getGrid(x, y);
					if (grid.belong != lastGrid.belong) {
						var p1 = grid.point(Grid.Direction.LD);
						var p2 = grid.point(Grid.Direction.RD);
						rawLines.Add(new Line(p1, p2));
					}
					lastGrid = grid;
				}
			}

			// 由左至右扫描
			for (int y = 0; y < mapY; ++y) {
				lastGrid = new Grid(0, y, item);
				for (int x = 0; x < mapX; ++x) {
					var grid = item.getGrid(x, y);
					if (grid.belong != lastGrid.belong) {
						var p1 = grid.point(Grid.Direction.LD);
						var p2 = grid.point(Grid.Direction.LU);
						rawLines.Add(new Line(p1, p2));
					}
					lastGrid = grid;
				}
			}
		}

		/// <summary>
		/// 生成可绘制的连续线段
		/// </summary>
		void makePaintableLines() {
			// 从中点扫描
			makePaintableLines(new Vector2(-0.5f, (mapY >> 1) - 0.5f));

			// 扫描剩余线条
			foreach (var line in rawLines) {
				if (line.flag) continue; // 遍历过

				makePaintableLines(line);
			}
		}
		void makePaintableLines(Vector2 point) {
			var lines = new List<Line>();

			makePaintableLines(lines, point);

			paintableLines.Add(lines);
		}
		void makePaintableLines(Line line) {
			var lines = new List<Line>();
			line.flag = true; lines.Add(line);

			makePaintableLines(lines, line.p2);

			paintableLines.Add(lines);
		}
		void makePaintableLines(List<Line> lines, Vector2 point) {
			if (point.x >= mapX - 1) return;

			Vector2 point2 = point;
			foreach (var line in rawLines) {
				if (line.flag) continue; // 遍历过

				var op = line.other(point);
				if (op == null) continue;
				point2 = (Vector2)op;

				line.flag = true;
				lines.Add(new Line(point, point2));
				break;
			}
			if (point2 == point) return;

			makePaintableLines(lines, point2);
		}

		/// <summary>
		/// 清空线段
		/// </summary>
		void clearLines() {
			rawLines.Clear();
			paintableLines.Clear();
		}

		#endregion

		#region 绘制

		/// <summary>
		/// 绘制地图
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(Map item) {
			base.drawExactlyItem(item);
		}
		
		#endregion
	}
}
