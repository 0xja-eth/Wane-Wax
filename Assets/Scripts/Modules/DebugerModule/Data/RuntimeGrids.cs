using System;
using System.Collections.Generic;

using Core.Data;

using GameModule.Data;
using GameModule.Services;

using ItemModule.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 格子组合（控制掉落）
	/// </summary>
	[DatabaseData]
	public class RuntimeGrids : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int gridsId { get; protected set; }
		[AutoConvert]
		public int _rotate { get; protected set; } = 0; // 逆时针旋转次数
		[AutoConvert]
		public Grid.Belong operer { get; protected set; } // 操纵者，对应的 Belong 是相反的

		/// <summary>
		/// 方块归属
		/// </summary>
		public Grid.Belong belong => Grid.opposite(operer);

		/// <summary>
		/// 方块组实例
		/// </summary>
		//public Grids grids => DataService.Get().get<Grids>(gridsId);
		public Grids grids => gridsId < Grids.GridsSet.Length ? 
			Grids.GridsSet[gridsId] : null;

		/// <summary>
		/// 归属状态
		/// </summary>
		public bool isPlayer => operer == Grid.Belong.Player;
		public bool isEnemy => operer == Grid.Belong.Enemy;

		/// <summary>
		/// 实际方块
		/// </summary>
		List<Grid> _realGrids;
		public List<Grid> realGrids {
			get {
				if (_realGrids == null) 
					_realGrids = processRotate(_rotate, grids.grids);

				return _realGrids;
			}
		}

		/// <summary>
		/// 根方块（最左方块）
		/// </summary>
		Grid _rootGrid;
		public Grid rootGrid {
			get {
				if (_rootGrid == null) {
					var grids = realGrids;
					_rootGrid = grids[0]; // 默认
					foreach (var grid in grids)
						if (grid.x <= _rootGrid.x) _rootGrid = grid;
				}
				return _rootGrid;
			}
		}

		/// <summary>
		/// 90度旋转 => x → y, y → -x
		/// </summary>
		List<Grid> processRotate(int rotate, List<Grid> grids) {
			var res = new List<Grid>(grids.Count);

			foreach(var grid in grids) { 
				int x = grid.x, y = grid.y;

				switch(rotate) {
					case 1: x = grid.y; y = -grid.x; break;
					case 2: x = -grid.x; y = -grid.y; break;
					case 3: x = -grid.y; y = grid.x; break;
				}

				// 如果是敌人，则镜像
				if (operer == Grid.Belong.Enemy) y = -y;

				res.Add(new Grid(x, y));
			}
			return res;
		}
		
		/// <summary>
		/// 旋转
		/// </summary>
		public void rotate() {
			_rotate = (_rotate + 1) % 4;
			_realGrids = null; _rootGrid = null;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public RuntimeGrids() { }
		public RuntimeGrids(Grids grids, Grid.Belong operer) : 
			this(grids.id, operer) { }
		public RuntimeGrids(int gid, Grid.Belong operer) {
			gridsId = gid; this.operer = operer;
		}
	}
}
