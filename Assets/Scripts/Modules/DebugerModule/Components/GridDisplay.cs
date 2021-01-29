using System;
using System.Collections.Generic;

using UnityEngine;

using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Components {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	public class GridDisplay : ItemDisplay<Grid> {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		public MapDisplay mapDisplay { get; set; }
		
		#region 绘制

		/// <summary>
		/// 绘制地图
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(Grid item) {
			base.drawExactlyItem(item);
		}

		#endregion
	}
}
