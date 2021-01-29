
using System;
using System.Collections.Generic;

using LitJson;

using Core.Data;

using ExerComps.Controls.ParamDisplays;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	using Services;

	/// <summary>
	/// 基本物品数据
	/// </summary>
	public abstract partial class BaseItem : BaseData,
		ParamDisplay.IDisplayDataConvertable {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public virtual string name { get; protected set; }
		[AutoConvert]
		public virtual string description { get; protected set; }
		[AutoConvert]
		public int type { get; protected set; } // 物品类型

		/// <summary>
		/// 关联类型
		/// </summary>
		public virtual Type defaultPackType => null;

		#region 属性显示数据生成

		/// <summary>
		/// 转化为属性信息集
		/// </summary>
		/// <returns>属性信息集</returns>
		public virtual JsonData convertToDisplayData(string type = "") {
			var res = new JsonData();

			res["name"] = name;
			res["description"] = description;

			return res;
		}

		#endregion

		/// <summary>
		/// 最大叠加数量
		/// </summary>
		/// <returns></returns>
		public virtual int maxCount => 1;

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString() {
			return string.Format("[{0}:{1}. {2}]", GetType(), id, name);
		}
	}
}
