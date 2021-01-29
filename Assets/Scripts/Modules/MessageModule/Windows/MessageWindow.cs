using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Components;
using Core.Utils;

using GameModule.Services;

namespace MessageModule.Windows {

	using Controls;

	using Services;
	using Data;

	/// <summary>
	/// 对话窗口层
	/// </summary>
	public class MessageWindow : BaseWindow {

		/// <summary>
		/// 内部组件设置
		/// </summary>
		public MessageDisplay display;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected MessageService messageSer;
		protected GameService gameSer;

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			if (!display) display = get<MessageDisplay>();
			display.messageWindow = this;
		}

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
            base.update();
			updateInput();
        }

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInput() {
			if (InputUtils.getKeyDown(MessageConfig.nextKeys))
				nextOrRevealAll();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否还有消息
		/// </summary>
		/// <returns></returns>
		public virtual bool hasMessages() {
			return messageSer.messageCount() > 0;
		}

		/// <summary>
		/// 是否还有消息
		/// </summary>
		/// <returns></returns>
		public virtual DialogMessage getMessage() {
			return messageSer.getMessage();
		}

		#endregion

		#region 内容绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			display.setItem(getMessage());
		}

		#endregion

		#region 消息事件

		/// <summary>
		/// 下一条消息或者快速展开文字
		/// </summary>
		protected void nextOrRevealAll() {
            if (display.printing) display.stopPrint();
            else if (display.optionCount() <= 0) deactivate();
        }

        #endregion
		
    }
}
