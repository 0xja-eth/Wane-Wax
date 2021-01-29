using System;
using System.Collections.Generic;

using UnityEngine;

using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Components {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	[RequireComponent(typeof(GridContainer))]
	[RequireComponent(typeof(LineRenderer))]
	public class MapDisplay : ItemDisplay<Map> {

		/// <summary>
		/// 线段
		/// </summary>
		public struct Line {

			/// <summary>
			/// 两点
			/// </summary>
			public Vector2 p1, p2;

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

			/// <summary>
			/// 构造函数
			/// </summary>
			public Line(Vector2 p1, Vector2 p2) {
				this.p1 = p1; this.p2 = p2;
			}
		}

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected GridContainer gridContainer;
		[RequireTarget]
		protected LineRenderer lineRenderer;

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public int mapX => item?.mapX ?? 0;
		public int mapY => item?.mapY ?? 0;

		/// <summary>
		/// 线段数据
		/// </summary>
		List<Line> rawLines = new List<Line>();
		List<Line> drawingLines = new List<Line>();

		#region 数据

		/// <summary>
		/// 地图改变
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			gridContainer.setItems(item.grids);

			generateLines();
		}

		/// <summary>
		/// 计算线段
		/// </summary>
		void generateLines() {
			clearLines();

			var lastGrid = new Grid(Grid.Belong.Enemy);

			for (int y = 0; y < mapY; ++y)
				for (int x = 0; x < mapX; ++x) {
					var grid = item.getGrid(x, y);
					if (lastGrid.belong != grid.belong)
						; // TODO: 获取边界线
				}
		}

		/// <summary>
		/// 清空线段
		/// </summary>
		void clearLines() {
			rawLines.Clear();
			drawingLines.Clear();
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

		/// <summary>
		/// 绘制线段
		/// </summary>
		void drawLines() {

		}

		#endregion
	}
}
