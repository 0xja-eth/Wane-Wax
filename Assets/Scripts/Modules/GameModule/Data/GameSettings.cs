
using UnityEngine;

using Core.Data;

namespace GameModule.Data {

	/// <summary>
	/// 游戏设定数据
	/// </summary>
	public partial class GameSettings : BaseData {
		
		/// <summary>
		/// 是否需要ID
		/// </summary>
		protected override bool idEnable() { return false; }
	}
	
}