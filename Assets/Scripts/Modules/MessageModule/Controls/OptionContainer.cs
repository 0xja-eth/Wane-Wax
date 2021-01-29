using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using ExerComps.Controls.ItemDisplays;

/// <summary>
/// 消息模块控件
/// </summary>
namespace MessageModule.Controls {

	using Data;
	using Windows;

	/// <summary>
	/// 选项容器
	/// </summary>
	public class OptionContainer : SelectableContainerDisplay<DialogOption> {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[HideInInspector]
		public MessageDisplay messageDisplay;

		/// <summary>
		/// 对话框窗口
		/// </summary>
		MessageWindow messageWindow => messageDisplay.messageWindow;

		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="index"></param>
		public override void onClick(int index) {
			base.onClick(index);
			messageWindow.deactivate();
		}
	}
}
