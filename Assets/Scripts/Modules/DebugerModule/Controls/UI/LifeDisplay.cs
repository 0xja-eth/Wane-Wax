using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using BattleModule;

using ExerComps.Controls.ParamDisplays;
using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Controls {

	using Data;

	/// <summary>
	/// 生命值显示
	/// </summary>
	public class LifeDisplay : ParamDisplay<RuntimeBattler> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Image[] lifes;

		/// <summary>
		/// 属性
		/// </summary>
		int maxLife => lifes.Length; // 显示的最大生命值

		/// <summary>
		/// 绘制值
		/// </summary>
		/// <param name="data"></param>
		protected override void drawExactlyValue(RuntimeBattler data) {
			base.drawExactlyValue(data);
			var rate = data.mhp / data.hp;
			var index = (int)(rate * maxLife);
			var alpha = (float)(rate - (index / maxLife)) * maxLife;

			for (int i = 0; i < maxLife; ++i) {
				var a = 1f;
				if (i == index) a = alpha;
				if (i > index) a = 0;

				lifes[i].color = new Color(1, 1, 1, a);
			}
		}
	}
}
