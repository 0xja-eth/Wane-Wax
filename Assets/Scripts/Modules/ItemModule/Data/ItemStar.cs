
using UnityEngine;

using Core.Data;

using GameModule.Data;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 物品星级数据
	/// </summary>
	[DatabaseData]
	public class ItemStar : TypeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public Color color { get; protected set; }
	}

}