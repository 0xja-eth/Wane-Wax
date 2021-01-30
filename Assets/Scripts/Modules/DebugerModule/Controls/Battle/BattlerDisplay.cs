using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations;

using ExerComps.Controls.AnimationExtensions;
using ExerComps.Controls.ItemDisplays;

namespace DebugerModule.Controls {

	using Data;

	/// <summary>
	/// 地图显示
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(AnimatorExtend))]
	public class BattlerDisplay : ItemDisplay<RuntimeBattler> {

		/// <summary>
		/// 外部变量设置
		/// </summary>
		public const int PlayerLayer = 8;
		public const int EnemyLayer = 9;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float jumpForce = 20; // 跳跃力量

		public RuntimeAnimatorController playerAnimator, enemyAnimator;

		/// <summary>
		/// 外部组件设置
		/// </summary>

		/// <summary>
		/// 内部组件设置
		/// </summary>
		[RequireTarget]
		protected SpriteRenderer sprite;
		[RequireTarget]
		protected new Rigidbody2D rigidbody;
		[RequireTarget]
		protected AnimatorExtend animator;

		public MapDisplay mapDisplay { get; set; }

		/// <summary>
		///  属性定义
		/// </summary>
		public bool isActor => item?.isActor ?? false;
		public bool isEnemy => item?.isEnemy ?? false;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		RuntimeBattler lastItem;

		#region 更新

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (item == null) return;

			updateState();
			updateDirection();
			updateVelocity();
			updatePosition();

			updateGravity();
		}

		/// <summary>
		/// 更新状态
		/// </summary>
		void updateState() {
			animator.setVar(item.state);
		}

		/// <summary>
		/// 更新速度
		/// </summary>
		void updateVelocity() {
			var velocity = rigidbody.velocity;
			velocity.x = item.velocity;

			rigidbody.velocity = velocity;
		}

		/// <summary>
		/// 更新朝向
		/// </summary>
		void updateDirection() {
			drawDirection(item);
		}

		/// <summary>
		/// 跳跃
		/// </summary>
		void _enterJumping() {
			var force = isEnemy ? -jumpForce : jumpForce;
			rigidbody.AddForce(new Vector2(0, force));
		}

		/// <summary>
		/// 更新位置
		/// </summary>
		void updatePosition() {
			var pos = transform.position;
			item.syncPosition(pos.x, pos.y);
		}

		/// <summary>
		/// 重力
		/// </summary>
		void updateGravity() {
			if (!isEnemy) return;
			rigidbody.AddForce(new Vector2(0, 9.8f*2));
		}

		#endregion

		#region 数据

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			lastItem?.stateMachine.removeObject(this);
			item?.stateMachine.registerObject(this);

			lastItem = item;
		}

		#endregion

		#region 绘制

		/// <summary>
		/// 绘制地图
		/// </summary>
		/// <param name="item"></param>
		protected override void drawExactlyItem(RuntimeBattler item) {
			base.drawExactlyItem(item);
			drawBelong(item);
			drawDirection(item);
		}

		/// <summary>
		/// 绘制外观
		/// </summary>
		/// <param name="item"></param>
		void drawBelong(RuntimeBattler item) {
			if (item.isActor) {
				gameObject.layer = PlayerLayer;
				transform.localEulerAngles = new Vector3(0, 0, 0);
				animator.animator.runtimeAnimatorController = playerAnimator;
			}
			if (item.isEnemy) {
				gameObject.layer = EnemyLayer;
				transform.localEulerAngles = new Vector3(0, 0, 180);
				animator.animator.runtimeAnimatorController = enemyAnimator;
			}
		}

		/// <summary>
		/// 绘制外观
		/// </summary>
		/// <param name="item"></param>
		void drawDirection(RuntimeBattler item) {
			sprite.flipX = item.direction;
		}

		#endregion
	}
}
