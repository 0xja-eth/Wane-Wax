
using System;
using System.Collections.Generic;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

using ExerComps.Controls.ParamDisplays;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 可装备物品数据
	/// </summary>
	public abstract partial class EquipableItem : LimitedItem { 
		//ParamDisplay.IDisplayDataArrayConvertable {

		/// <summary>
		/// 属性类型
		/// </summary>
		//public enum ParamType {
		//    NoType = 0, // 无类型
		//    Attack = 1, // 攻击型
		//    Defense = 2 // 防御型
		//}

		/// <summary>
		/// 属性
		/// </summary>
		//[AutoConvert]
		//public ParamRateData[] levelParams { get; protected set; }
		//[AutoConvert]
		//public ParamData[] baseParams { get; protected set; }
		[AutoConvert]
		public int minLevel { get; protected set; }

		#region 数据转换

		/// <summary>
		/// 转化为属性信息集
		/// </summary>
		/// <returns>属性信息集</returns>
		public override JsonData convertToDisplayData(string type = "") {
			var res = base.convertToDisplayData(type);

			res["min_level"] = minLevel;
			//res["params"] = DataLoader.convert(
			//	convertToDisplayDataArray(type));

			return res;
		}

		///// <summary>
		///// 转化为属性信息集
		///// </summary>
		///// <returns>属性信息集</returns>
		//public JsonData[] convertToDisplayDataArray(string type = "") {
		//	var params_ = DataService.get().staticData.configure.baseParams;
		//	var count = params_.Length;
		//	var data = new JsonData[count];
		//	for (int i = 0; i < count; ++i) {
		//		var json = new JsonData();
		//		var paramId = params_[i].id;

		//		var levelParam = getLevelParam(paramId).value;
		//		var baseParam = getBaseParam(paramId).value;

		//		json["level_param"] = levelParam;
		//		json["equip_param"] = baseParam;

		//		data[i] = json;
		//	}
		//	return data;
		//}

		#endregion

		///// <summary>
		///// 获取装备的属性
		///// </summary>
		///// <param name="paramId">属性ID</param>
		///// <returns>属性数据</returns>
		//public ParamRateData getLevelParam(int paramId) {
		//	foreach (var param in levelParams)
		//		if (param.paramId == paramId) return param;
		//	return new ParamRateData(paramId);
		//}

		///// <summary>
		///// 获取装备的属性
		///// </summary>
		///// <param name="paramId">属性ID</param>
		///// <returns>属性数据</returns>
		//public ParamData getBaseParam(int paramId) {
		//	foreach (var param in baseParams)
		//		if (param.paramId == paramId) return param;
		//	return new ParamData(paramId);
		//}
	}
}
