using System;
using System.Collections.Generic;

namespace BattleModule.Data {

	/// <summary>
	/// 特性对象接口
	/// </summary>
	public partial interface ITraitsObject {

		/// <summary>
		/// 效果数组
		/// </summary>
		TraitData[] baseTraits { get; }
	}

	/// <summary>
	/// 特性对象接口
	/// </summary>
	public partial interface ITraitsObject<T> :
		ITraitsObject where T : TraitData {

		/// <summary>
		/// 特性数组
		/// </summary>
		T[] traits { get; }
	}
}
