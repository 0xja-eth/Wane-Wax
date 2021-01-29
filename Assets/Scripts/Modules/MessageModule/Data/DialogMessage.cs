
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Core.Data;

using AssetModule.Services;
using AssetModule;

/// <summary>
/// ITU模块数据
/// </summary>
namespace MessageModule.Data {

	/// <summary>
	/// 对话框信息
	/// </summary>
	[Serializable]
	public class DialogMessage : BaseData {

		/// <summary>
		/// 对话类型
		/// </summary>
		public enum Type {
			Normal, Log
		}

		/// <summary>
		/// 属性
		/// </summary>
		[TextArea(0, 100)]
		public string message = ""; // 消息
		public string name = ""; // 名称

		public Type type = Type.Normal; // 类型

		/// <summary>
		/// 立绘
		/// </summary>
		public int bustId = 0; // 立绘ID
		[SerializeField]
		Sprite _bust = null; // 立绘（Editor赋值）

		/// <summary>
		/// 选项
		/// </summary>
		[SerializeField]
		public List<DialogOption> options = new List<DialogOption>();

		/// <summary>
		/// 立绘
		/// </summary>
		//[AutoConvert]
		//public int bustId { get; protected set; } // 立绘ID

		/// <summary>
		/// 获取立绘实例
		/// </summary>
		/// <returns></returns>
		protected CacheAttr<Sprite> bust_ = null;
		protected Sprite _bust_() {
			return AssetService.Get().loadAsset<Sprite>(
				MessageConfig.Bust, bustId);
		}
		public Sprite bust() {
			return _bust ?? bust_?.value();
		}

		/// <summary>
		/// 获取测试数据
		/// </summary>
		/// <returns></returns>
		public static DialogMessage testData(
			string message, string name = "", int bustId = 0) {
			var msg = new DialogMessage();
			msg.message = message;
			msg.name = name;

			msg.bustId = bustId;

			for (int i = 0; i < UnityEngine.Random.Range(0, 4); ++i)
				msg.options.Add(DialogOption.testData(i));

			return msg;
		}
	}

}
