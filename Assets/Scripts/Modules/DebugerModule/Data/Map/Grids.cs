using System;
using System.Collections.Generic;

using Core.Data;

using GameModule.Data;

using ItemModule.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 格子组合（控制掉落）
	/// </summary>
	[DatabaseData]
	public class Grids : BaseItem {

		/// <summary>
		/// 格子组合数据库
		/// </summary>
		public static readonly Grids[] GridsSet = new Grids[] {
			// 长条形
			new Grids("1"), new Grids("11"), //new Grids("111"),

			// 7字形
			new Grids("01\n11"), // new Grids("010\n111"), new Grids("001\n111"),

			// 矩形
			new Grids("11\n11")
		};

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string code { get; protected set; }

		/// <summary>
		/// 解析数据
		/// </summary>
		public int width { get; protected set; }
		public int height { get; protected set; }

		public List<Grid> grids { get; protected set; }

		/// <summary>
		/// 解析
		/// </summary>
		void parse() {
			grids = new List<Grid>();

			var lines = code.Split('\n');
			width = lines[0].Length;
			height = lines.Length;

			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					if (lines[y][x] == '1')
						grids.Add(new Grid(x, height-y));
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public Grids() { }
		public Grids(string code) {
			this.code = code;
			parse();
		}
	}
}
