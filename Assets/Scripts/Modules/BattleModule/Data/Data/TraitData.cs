
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
	/// 特征数据
	/// </summary>
	public abstract class TraitData : CodeParamsData {

		/// <summary>
		/// 构造函数
		/// </summary>
		public TraitData() { }
		public TraitData(int code, JsonData params_,
			string description, string shortDescription) :
			base(code, params_, description, shortDescription) { }
	}

	/// <summary>
	/// 特征数据
	/// </summary>
	public abstract class TraitData<E> : TraitData where E : Enum {

		/// <summary>
		/// 枚举值
		/// </summary>
		public E codeEnum => EnumUtils.getEnum<E>(code);

		/// <summary>
		/// 构造函数
		/// </summary>
		public TraitData() { }
		public TraitData(int code, JsonData params_,
			string description = "", string shortDescription = "") : 
			base(code, params_, description, shortDescription) { }
		public TraitData(E code, JsonData params_,
			string description = "", string shortDescription = "") : 
			base(code.GetHashCode(), params_, 
				description, shortDescription) { }
	}

}
