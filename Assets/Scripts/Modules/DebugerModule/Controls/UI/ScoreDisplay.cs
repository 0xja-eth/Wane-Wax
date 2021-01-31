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
	public class ScoreDisplay : ParamDisplay<int> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text score;

		/// <summary>
		/// 绘制值
		/// </summary>
		/// <param name="data"></param>
		protected override void drawExactlyValue(int data) {
			base.drawExactlyValue(data);
			score.text = data.ToString();
		}
	}
}
