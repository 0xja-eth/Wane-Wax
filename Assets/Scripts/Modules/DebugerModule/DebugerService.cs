using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

using Core.Services;

namespace DebugerModule.Services {

	using Data;

	/// <summary>
	/// 游戏服务
	/// </summary>
	public class DebugerService : BaseService<DebugerService> {

		/// <summary>
		/// 地图
		/// </summary>
		public Map currentMap;

		/// <summary>
		/// 方块
		/// </summary>
		public RuntimeGrids currentGrids;
		public RuntimeGrids nextGrids;

		/// <summary>
		/// 状态
		/// </summary>
		public enum State {
			None, // 默认
			Start, // 游戏开始
			Placing, // 放置方块中
			Placed, // 方块放置完毕
			Result // 结果
		}

		#region 初始化

		/// <summary>
		/// 状态类型
		/// </summary>
		protected override Type stateType => typeof(State); 

		#endregion

		#region 游戏控制

		/// <summary>
		/// 游戏开始
		/// </summary>
		public void start() {
			changeState(State.Start);

			var mapX = DebugerConfig.MapX;
			var mapY = DebugerConfig.MapY;

			currentMap = new Map(mapX, mapY);
		}

		/// <summary>
		/// 生成下一个格子
		/// </summary>
		void generateNextGrids() {
			currentGrids = nextGrids ?? generateGrids();
			nextGrids = generateGrids();

			changeState(State.Placing);
		}

		/// <summary>
		/// 生成格子
		/// </summary>
		/// <returns></returns>
		public RuntimeGrids generateGrids(Grid.Belong operer = Grid.Belong.Player) {
			var gridsSet = Grids.GridsSet;
			var grids = gridsSet[Random.Range(0, gridsSet.Length)];

			return new RuntimeGrids(grids, operer);
		}

		/// <summary>
		/// 放置方块
		/// </summary>
		/// <param name="operer"></param>
		public void placeGrids(RuntimeGrids grids, int x, bool preview = false) {
			currentMap.placeGrids(grids, x, preview);

			if (grids.isPlayer && !preview) changeState(State.Placed);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void updateOthers() {
			base.updateOthers();
			updateBattlers();
		}

		/// <summary>
		/// 更新角色
		/// </summary>
		void updateBattlers() {

		}

		/// <summary>
		/// 更新输入
		/// </summary>
		void updateInputting() {
			var mPos = Input.mousePosition;

			Debug.Log("Mouse: " + mPos);

			placeGrids(currentGrids, (int)mPos.x, 
				!Input.GetKeyDown(KeyCode.Space));
		}

		/// <summary>
		/// 更新游戏开始
		/// </summary>
		void _updateStart() {
			generateNextGrids();
		}

		/// <summary>
		/// 更新放置状态
		/// </summary>
		void _updatePlacing() {
			updateInputting();
		}

		/// <summary>
		/// 更新放置后状态
		/// </summary>
		void _updatePlaced() {
			generateNextGrids();
		}

		#endregion
	}
}
