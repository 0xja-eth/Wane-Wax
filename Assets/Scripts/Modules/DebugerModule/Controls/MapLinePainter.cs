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

		#region 绘制

		/// <summary>
		/// 绘制线条
		/// </summary>
		/// <param name="lines"></param>
		protected override void drawExactlyItem(List<MapDisplay.Line> lines) {
			base.drawExactlyItem(lines);

			var points = new List<Vector2>();

			foreach(var line in lines) {
				points.Add(line.p1); points.Add(line.p2);
			}

			lineRenderer.positionCount = points.Count;

			for (var i = 0; i < points.Count; ++i)
				lineRenderer.SetPosition(i, points[i]);
		}

		#endregion
	}
}
