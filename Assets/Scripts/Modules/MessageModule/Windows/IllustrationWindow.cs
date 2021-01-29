using Core.Components;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.Systems;
using Core.Components.Utils;

using GameModule.Services;

namespace MessageModule.Windows {

	using Data;

	/// <summary>
	/// 对话窗口层
	/// </summary>
    public class IllustrationWindow : MessageWindow {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public GameObject foreground; // 前景，用于最后窗口隐藏后覆盖在最上面的背景

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public UnityEvent onExit; // 退出回调

		public AudioClip illustrationBgm; // BGM

		public List<DialogMessage> illustrationMessages; // 消息内容
		
        /// <summary>
        /// 外部系统设置
        /// </summary>
		protected SceneSystem sceneSys;

		#region 流程控制

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();

			if (illustrationBgm) {
				SceneUtils.audioSource.clip = illustrationBgm;
				SceneUtils.audioSource.Play();
			}
		}

		/// <summary>
		/// 开始
		/// </summary>
		protected override void start() {
            base.start();
			requestRefresh();
        }
		
		/// <summary>
		/// 窗口完全开启回调
		/// </summary>
		protected override void onWindowShown() {
			base.onWindowShown();

			if (foreground) foreground.SetActive(true);
		}

		/// <summary>
		/// 窗口完全隐藏回调
		/// </summary>
		protected override void onWindowHidden() {
			base.onWindowHidden();

			if (hasMessages()) activate(); 
			else processExit();
		}

		/// <summary>
		/// 处理退出
		/// </summary>
		void processExit() {
			onExit?.Invoke();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 是否还有消息
		/// </summary>
		/// <returns></returns>
		public override bool hasMessages() {
			return illustrationMessages.Count > 0;
		}

		/// <summary>
		/// 是否还有消息
		/// </summary>
		/// <returns></returns>
		public override DialogMessage getMessage() {
			var msg = illustrationMessages[0];
			illustrationMessages.RemoveAt(0);
			return msg;
		}

		#endregion

	}
}
