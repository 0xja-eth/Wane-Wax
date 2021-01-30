using System;
using System.Collections.Generic;

using UnityEngine;

using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Controls {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	[RequireComponent(typeof(MapDisplay))]
	public class GridContainer : ContainerDisplay<Grid> {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		[HideInInspector]
		public MapDisplay mapDisplay;

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public int mapX => mapDisplay.mapX;
		public int mapY => mapDisplay.mapY;

		#region 数据

		/// <summary>
		/// 获取实际位置
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Vector2 getPosition(int x, int y) {
			return new Vector2(x - mapX / 2f + 0.5f, y - mapY / 2f + 0.5f);
		}

		#endregion

		#region 绘制

		/// <summary>
		/// Sub创建
		/// </summary>
		protected override void onSubViewCreated(ItemDisplay<Grid> sub, int index) {
			base.onSubViewCreated(sub, index);
			var display = sub as GridDisplay;
			if (display == null) return;

			var grid = items[index];
			var pos = getPosition(grid.x, grid.y);

			display.mapDisplay = mapDisplay;
			display.transform.localPosition = pos;
		}

		#endregion
	}
}
