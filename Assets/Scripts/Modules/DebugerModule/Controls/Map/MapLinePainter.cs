using System;
using System.Collections.Generic;

using UnityEngine;

using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Controls {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class MapLinePainter : ItemDisplay<List<MapDisplay.Line>> {

		/// <summary>
		/// 内部组件定义
		/// </summary>
		[RequireTarget]
		protected LineRenderer lineRenderer;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		public MapDisplay mapDisplay { get; set; }

		#region 绘制

		/// <summary>
		/// 获取实际座标
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Vector2 getPosition(Vector2 pos) {
			return mapDisplay.getGridPosition(pos);
		}

		/// <summary>
		/// 绘制线条
		/// </summary>
		/// <param name="lines"></param>
		protected override void drawExactlyItem(List<MapDisplay.Line> lines) {
			base.drawExactlyItem(lines);

			var points = new List<Vector2>();

			if (lines.Count > 0) points.Add(lines[0].p1);
			foreach(var line in lines) points.Add(line.p2);

			lineRenderer.positionCount = points.Count;

			for (var i = 0; i < points.Count; ++i) {
				var pos = getPosition(points[i]);
				lineRenderer.SetPosition(i, pos);
			}
		}

		#endregion
	}
}
