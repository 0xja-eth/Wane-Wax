
using UnityEngine;

using LitJson;

using Core.Data;

using GameModule.Services;
using AssetModule.Services;

/// <summary>
/// 物品模块数据
/// </summary>
namespace ItemModule.Data {

	/// <summary>
	/// 有限物品数据
	/// </summary>
	public partial class BaseItem {
		
		/// <summary>
		/// 能否购买
		/// </summary>
		/// <returns></returns>
		public virtual bool isBuyable => true;
	}

}
