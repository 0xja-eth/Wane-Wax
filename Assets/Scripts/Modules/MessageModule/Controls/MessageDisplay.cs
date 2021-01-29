using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using PlayerModule.Services;

using AssetModule.Services;
using AssetModule;

using ExerComps.Controls.ItemDisplays;

/// <summary>
/// 消息模块控件
/// </summary>
namespace MessageModule.Controls {
	
	using Data;

	using Windows;

	/// <summary>
	/// 消息显示基类
	/// </summary>
	public class MessageDisplay : ItemDisplay<DialogMessage> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text message;
		public Image image;
		public GameObject imageFrame;

		public new Text name;
		public GameObject nameFrame;

		public OptionContainer optionContainer;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[HideInInspector]
		public MessageWindow messageWindow;

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public bool setNativeSize = true;
		public float printDeltaTime = 0.05f; // 文本打印间隔时间

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool stopPrintReq = false; // 停止打印请求（打印到最后一个）

		/// <summary>
		/// 属性
		/// </summary>
		public bool printing { get; protected set; } = false; // 当前是否打印中
		
		/// <summary>
		/// 内部变量设置
		/// </summary>
		Dictionary<string, int> bustIdDict = new Dictionary<string, int>();

		/// <summary>
		/// 外部系统设置
		/// </summary>
		protected PlayerService playerSer;
		protected AssetService assetSer;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			if (!imageFrame) imageFrame = image?.gameObject;
			if (!nameFrame) nameFrame = name?.gameObject;

			if (optionContainer)
				optionContainer.messageDisplay = this;
		}

		#endregion

		#region 数据操作

		/// <summary>
		/// 选项数目
		/// </summary>
		/// <returns></returns>
		public int optionCount() {
			return item.options.Count;
		}

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			if (printing) stopPrint();
			optionContainer?.setItems(item.options);
		}

		/// <summary>
		/// 物品清除回调
		/// </summary>
		protected override void onItemClear() {
			base.onItemClear();
			if (printing) stopPrint();
			optionContainer?.clearItems();
		}

		/// <summary>
		/// 获取立绘
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Sprite[] busts(string name) {
			if (bustIdDict.ContainsKey(name)) {
				var bid = bustIdDict[name];
				if (bid == 0) bid = MessageConfig.currentBustId();
				return assetSer.loadAssets<Sprite>(MessageConfig.Bust, bid);
			}
			return null;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制物品
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(DialogMessage item) {
			base.drawExactlyItem(item);
			drawMessage(item);
			drawImage(item);
			drawName(item);
		}

		/// <summary>
		/// 绘制信息
		/// </summary>
		/// <param name="item"></param>
		void drawMessage(DialogMessage item) {
			doRoutine(printMessage(item.message));
		}

		/// <summary>
		/// 绘制名称
		/// </summary>
		/// <param name="item"></param>
		void drawName(DialogMessage item) {
			if (!nameFrame || !name) return;

			name.text = item.name;
			nameFrame.SetActive(!string.IsNullOrEmpty(item.name));
		}

		/// <summary>
		/// 获取立绘
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected virtual Sprite getBust(DialogMessage item) {
			var bust = item.bust();
			if (!bust && busts(item.name) != null)
				return busts(item.name)[0];
			return bust;
		}

		/// <summary>
		/// 绘制立绘
		/// </summary>
		/// <param name="item"></param>
		virtual protected void drawImage(DialogMessage item) {
			if (!imageFrame || !image) return;

			var bust = getBust(item);

			image.overrideSprite = bust;
			imageFrame.SetActive(bust != null);

			if (bust && setNativeSize) image.SetNativeSize();
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			message.text = "";
			if (imageFrame && image) {
				//image.overrideSprite = null;
				imageFrame.SetActive(false);
			}
			if (nameFrame && name) {
				//name.text = "";
				nameFrame.SetActive(false);
			}
		}

		/// <summary>
		/// 停止打印
		/// </summary>
		public void stopPrint() {
			stopPrintReq = true;
		}

		/// <summary>
		/// 打印信息
		/// </summary>
		/// <returns></returns>
		IEnumerator printMessage(string message) {
			onPrintStart(message);

			foreach (var c in message) {
				this.message.text += c;
				if (stopPrintReq) break;

				yield return new WaitForSeconds(printDeltaTime);
			}

			onPrintEnd(message);
		}

		/// <summary>
		/// 打印开始回调
		/// </summary>
		virtual protected void onPrintStart(string message) {
			printing = true;
			this.message.text = "";

			if (optionContainer)
				optionContainer.deactivate();
		}

		/// <summary>
		/// 打印结束回调
		/// </summary>
		virtual protected void onPrintEnd(string message) {
			stopPrintReq = printing = false;
			this.message.text = message;

			if (optionContainer)
				optionContainer.activate();
		}

		#endregion

	}
}
