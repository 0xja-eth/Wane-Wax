
using System;
using System.Collections.Generic;

using Core.Data;

namespace GameModule.Data {

	using Utils;

	/// <summary>
	/// 游戏系统配置数据
	/// </summary>
	public partial class GameConfigure {

		/// <summary>
		/// 配置项
		/// </summary>
		[AutoConvert]
		public TupleList targetTypes { get; protected set; }

	}

}