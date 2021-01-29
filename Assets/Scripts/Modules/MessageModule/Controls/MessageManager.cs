using System;
using System.Collections;
using System.Collections.Generic;

using Core.Components;

/// <summary>
/// 消息模块控件
/// </summary>
namespace MessageModule.Controls {

	using Services;
	using Data;

	using Windows;

	/// <summary>
	/// 消息显示基类
	/// </summary>
	public class MessageManager : GeneralComponent {

		/// <summary>
		/// 消息类型-窗口映射
		/// </summary>
		[Serializable]
		public class MessageTypeWindow {

			/// <summary>
			/// 属性
			/// </summary>
			public DialogMessage.Type type; // 类型
			public MessageWindow window; // 对应窗口
		}

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public List<MessageTypeWindow> windows;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected MessageService messageSer;

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateDialog();
		}

		/// <summary>
		/// 更新对话框
		/// </summary>
		void updateDialog() {
			if (messageSer.messageCount() > 0 && !isBusy()) {
				var type = messageSer.getMessageType();
				getWindow(type)?.activate();
			}
		}

		#endregion

		#region 状态判断

		/// <summary>
		/// 繁忙
		/// </summary>
		/// <returns></returns>
		public bool isBusy() {
			return !windows.TrueForAll(w => !w.window.shown);
		}

		#endregion

		#region 数据获取

		/// <summary>
		/// 获取对应类型的窗口
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public MessageWindow getWindow(DialogMessage.Type type) {
			foreach (var w in windows)
				if (w.type == type) return w.window;
			return null;
		}

		#endregion
	}
}
