using System;
using System.Collections.Generic;

using UnityEngine;

using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Components {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BoxCollider2D))]
	public class GridDisplay : ItemDisplay<Grid> {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public int PlayerLayer = 8;
		public int EnemyLayer = 9;

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public GameObject previewObject;

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected new MeshRenderer renderer;

		public MapDisplay mapDisplay { get; set; }

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (item == null) return;
			if (item.refreshRequest) requestRefresh(true);
		}

		#endregion

		#region 数据

		#endregion

		#region 绘制

		/// <summary>
		/// 绘制地图
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(Grid item) {
			base.drawExactlyItem(item);
			drawInterface(item);
			drawBelong(item);
		}

		/// <summary>
		/// 绘制外观
		/// </summary>
		/// <param name="item"></param>
		void drawInterface(Grid item) {
			previewObject.SetActive(item.type == Grid.Type.Preview);
		}

		/// <summary>
		/// 绘制归属效果
		/// </summary>
		/// <param name="item"></param>
		void drawBelong(Grid item) {
			if (item.isPlayer) {
				gameObject.layer = PlayerLayer;
				renderer.enabled = false;
			}
			if (item.isEnemy) {
				gameObject.layer = EnemyLayer;
				renderer.enabled = true;
			}
		}

		#endregion
	}
}
