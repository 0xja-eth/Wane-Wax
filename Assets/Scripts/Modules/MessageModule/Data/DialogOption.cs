
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
	/// 对话框选项
	/// </summary>
	[Serializable]
	public class DialogOption : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		public string text = "";

		/// <summary>
		/// 动作
		/// </summary>
		//public List<UnityAction> actions = new List<UnityAction>();
		public UnityEvent actions = new UnityEvent();

		/// <summary>
		/// 添加动作
		/// </summary>
		/// <param name="action"></param>
		public void addAction(UnityAction action) {
			actions.AddListener(action);
		}

		/// <summary>
		/// 执行
		/// </summary>
		public void invoke() {
			actions?.Invoke();
			//foreach (var action in actions) action?.Invoke();
		}

		/// <summary>
		/// 获取测试数据
		/// </summary>
		/// <returns></returns>
		public static DialogOption testData(int index) {
			var opt = new DialogOption();
			opt.text = "选项" + index;
			opt.addAction(() => Debug.Log("You selected: " + index));

			return opt;
		}
	}

}
