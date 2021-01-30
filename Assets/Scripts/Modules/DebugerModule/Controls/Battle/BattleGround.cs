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
	public class BattleGround : ContainerDisplay<RuntimeBattler> {

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

		#region 绘制
		
		/// <summary>
		/// 获取实际位置
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public Vector2 getPosition(int x, int y) {
			return mapDisplay.getBattlerPosition(x, y);
		}

		/// <summary>
		/// Sub创建
		/// </summary>
		protected override void onSubViewCreated(
			ItemDisplay<RuntimeBattler> sub, int index) {
			base.onSubViewCreated(sub, index);
			var display = sub as BattlerDisplay;
			if (display == null) return;

			// 因为他是创建之后再对里面的物品进行赋值
			var battler = items[index]; 
			var pos = getPosition(battler.x, battler.y);

			display.mapDisplay = mapDisplay;
			display.transform.localPosition = pos;
		}

		#endregion
	}
}
