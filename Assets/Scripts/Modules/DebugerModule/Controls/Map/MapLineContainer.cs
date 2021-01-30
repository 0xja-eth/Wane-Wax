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
	public class MapLineContainer : 
		ContainerDisplay<List<MapDisplay.Line>> {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		[HideInInspector]
		public MapDisplay mapDisplay;

		/// <summary>
		/// Sub创建
		/// </summary>
		protected override void onSubViewCreated(
			ItemDisplay<List<MapDisplay.Line>> sub, int index) {
			base.onSubViewCreated(sub, index);
			var painter = sub as MapLinePainter;
			if (painter == null) return;

			painter.mapDisplay = mapDisplay;
		}
	}
}
