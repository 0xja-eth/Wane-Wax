
using System;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

using Core.Utils;

/// <summary>
/// 物品模块数据
/// </summary>
namespace BattleModule.Data {

	/// <summary>
	/// 使用效果数据
	/// </summary>
	public abstract class CodeParamsData : BaseData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int code { get; protected set; }
		[AutoConvert("params")]
		public JsonData params_ { get; protected set; } // 参数（数组）
		[AutoConvert]
		public string description { get; protected set; }
		[AutoConvert]
		public string shortDescription { get; protected set; }

		/// <summary>
		/// 获取指定下标下的数据
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="index">下标</param>
		/// <param name="default_">默认值</param>
		/// <returns></returns>
		public T get<T>(int index, T default_ = default) {
			if (params_ == null || !params_.IsArray) return default_;
			if (index < params_.Count) return DataLoader.load<T>(params_[index]);
			return default_;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public CodeParamsData() { }
		public CodeParamsData(int code, JsonData params_,
			string description, string shortDescription) {
			this.code = code;
			this.params_ = params_;
			this.description = description;
			this.shortDescription = shortDescription;
		}
	}
	
}
