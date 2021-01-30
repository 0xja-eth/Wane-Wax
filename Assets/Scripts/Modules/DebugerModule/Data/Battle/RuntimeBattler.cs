﻿using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using BattleModule;
using BattleModule.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 战斗者
	/// </summary>
	public class RuntimeBattler : BaseRuntimeBattler {

		/// <summary>
		/// 状态
		/// </summary>
		public enum State {
			Idle, Panic, Running, Jumping, Falling, Hitting, Dead
		}

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public int x { get; protected set; }
		[AutoConvert]
		public int y { get; protected set; }

		[AutoConvert]
		public bool direction { get; protected set; } = true; // 朝向（true 为右，false 为左

		[AutoConvert]
		public float frequency { get; protected set; } = 1f; // 移动频率 s/次
		[AutoConvert]
		public float speed { get; protected set; } = 1f; // 移动速度 m/s

		/// <summary>
		/// 下一个坐标
		/// </summary>
		public int nextX => (map.mapX + (direction ? x + 1 : x - 1)) % map.mapX;
		public int nextY => y + nextWall; // wall 大于 2 会修改direction

		/// <summary>
		/// 下一个坐标墙高度
		/// </summary>
		public int nextWall => map.judgeWall(nextX, y, belong);

		/// <summary>
		/// 归属
		/// </summary>
		public Grid.Belong belong => isActor ?
			Grid.Belong.Player : Grid.Belong.Enemy;
 
		/// <summary>
		/// 实际速度
		/// </summary>
		public float velocity { get; protected set; } = 0; 

		/// <summary>
		/// 所在地图
		/// </summary>
		public Map map { get; set; }

		/// <summary>
		/// 计时器
		/// </summary>
		float idleTime = 0, runTime = 0;

		#region 配置

		/// <summary>
		/// 状态类型
		/// </summary>
		public override Type stateType => typeof(State);

		#endregion

		#region 流程

		/// <summary>
		/// 向前移动
		/// </summary>
		public void move() {
			var state = getMoveState();

			if (state == State.Panic) return;

			velocity = speed;
			runTime = 1 / speed;

			changeState(state);
		}

		/// <summary>
		/// 计算移动状态
		/// </summary>
		State getMoveState(bool backWall = false) {
			var wall = nextWall;
			if (wall < 0) return State.Falling;
			else if (wall == 0) return State.Running;
			else if (wall == 1) return State.Jumping;
			// wall >= 2
			else if (backWall) return State.Panic;
			else { 
				direction = !direction;
				return getMoveState(true);
			}
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void stop() {
			y = nextY; x = nextX;

			idleTime = velocity = 0;
			changeState(State.Idle);
		}

		#endregion

		#region 更新

		/// <summary>
		/// 更新待命
		/// </summary>
		protected override void _updateIdle() {
			idleTime += Time.deltaTime;
			if (idleTime >= frequency) move();
		}

		/// <summary>
		/// 更新跑步
		/// </summary>
		protected virtual void _updatePanic() {
			move();
		}

		/// <summary>
		/// 更新跑步
		/// </summary>
		protected virtual void _updateRunning() {
			updateMoving();
		}

		/// <summary>
		/// 更新爬升
		/// </summary>
		protected virtual void _updateJumping() {
			updateMoving();
		}

		/// <summary>
		/// 更新下落
		/// </summary>
		protected virtual void _updateFalling() {
			updateMoving();
		}

		/// <summary>
		/// 更新下落
		/// </summary>
		protected virtual void updateMoving() {
			runTime -= Time.deltaTime;
			if (runTime <= 0) stop();
		}

		/// <summary>
		/// 更新受击
		/// </summary>
		protected override void _updateHitting() {

		}

		/// <summary>
		/// 更新死亡
		/// </summary>
		protected virtual void _updateDead() {

		}

		#endregion

		/// <summary>
		/// 初始化
		/// </summary>
		void initialize() {
			changeState(State.Idle);
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public RuntimeBattler() { }
	}
}
