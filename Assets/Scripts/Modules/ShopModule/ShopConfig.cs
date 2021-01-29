using System;
using System.Collections.Generic;

using Core.Data;

namespace ShopModule {

	/// <summary>
	/// 配置类
	/// </summary>
	public static class ShopConfig {
		

	}

	/// <summary>
	/// 物品类型
	/// </summary>
	public partial class ShopType {
		public static int
			Unset = 0  // 未设置
		;
	}

	/// <summary>
	/// 物品类型
	/// </summary>
	public partial class ShopItemType {
		public static int
			Unset = 0  // 未设置
		;
	}

}

/// <summary>
/// 玩家模块数据
/// </summary>
namespace ShopModule.Data {

	/// <summary>
	/// 玩家数据（在这里添加自定义属性）
	/// </summary>
	public partial class Money {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int gold { get; set; }

	}
}