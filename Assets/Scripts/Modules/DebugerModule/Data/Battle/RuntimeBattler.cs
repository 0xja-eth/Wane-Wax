using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using BattleModule;
using BattleModule.Data;

namespace DebugerModule.Data {

	using Services;

	/// <summary>
	/// 战斗者
	/// </summary>
	public abstract class RuntimeBattler : BaseRuntimeBattler {

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
		public bool direction { get; protected set; } // 朝向（true 为右，false 为左

		[AutoConvert]
		public float frequency { get; protected set; } = 0f; // 移动频率 s/次
		[AutoConvert]
		public float speed { get; protected set; } = 1f; // 移动速度 m/s

		[AutoConvert]
		public double hurtRate { get; protected set; } = 5; // 伤害速度 点/s
		[AutoConvert]
		public double recoveryRate { get; protected set; } = 1; // 恢复速度 点/s

		/// <summary>
		/// 地图尺寸
		/// </summary>
		public int mapX => map.mapX;
		public int mapY => map.mapY;

		/// <summary>
		/// 下一个坐标
		/// </summary>
		public int nextX => (mapX + (direction ? x + 1 : x - 1)) % map.mapX;
		public virtual int nextY => y + nextWall; // wall 大于 2 会修改direction

		public int prevX => (mapX + (direction ? x - 1 : x + 1)) % map.mapX;

		/// <summary>
		/// 下一个坐标墙高度
		/// </summary>
		int _nextWall = 999;
		public int nextWall {
			get {
				if (_nextWall > 2)
					_nextWall = map.judgeWall(nextX, y, belong);
				return _nextWall;
			}
		}

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
		/// 初始位置
		/// </summary>
		public abstract Vector2 initPos { get; }
		public abstract bool initDir { get; }

		/// <summary>
		/// 所在地图
		/// </summary>
		public Map map { get; set; }

		/// <summary>
		/// 服务
		/// </summary>
		protected DebugerService debugSer => DebugerService.Get();

		/// <summary>
		/// 计时器
		/// </summary>
		float idleTime = 0, runTime = 0;

		/// <summary>
		/// 目标
		/// </summary>
		public float realX, realY;
		int targetX, targetY;

		#region 配置

		/// <summary>
		/// 状态类型
		/// </summary>
		public override Type stateType => typeof(State);

		#endregion

		#region 状态

		/// <summary>
		/// 状态判断
		/// </summary>
		public bool isMoving => isState(State.Running) ||
			isState(State.Jumping) || isState(State.Falling);
		public bool isRunning => isState(State.Running);
		public bool isJumping => isState(State.Jumping);
		public bool isFalling => isState(State.Falling);

		public bool isIdle => isState(State.Idle);

		public bool isPanic => isState(State.Panic);

		#endregion

		#region 流程

		/// <summary>
		/// 向前移动
		/// </summary>
		public void move() {
			idleTime = 0;

			var state = getMoveState();

			if (state == State.Panic) {
				targetX = x; targetY = map.getFallingY(x, y, belong);
			} else {
				targetX = nextX; targetY = nextY;
			}

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
			else if (backWall) {
				_nextWall = 999; // 重置 nextWall
				direction = !direction;
				return State.Panic;
			} else {
				_nextWall = 999; // 重置 nextWall
				direction = !direction;
				return getMoveState(true);
			}
		}

		///// <summary>
		///// 同步位置
		///// </summary>
		///// <param name="x"></param>
		///// <param name="y"></param>
		//public void syncPosition(float x, float y) {
		//	Debug.Log(this + ". sync: " + x + ", target: " + targetX);

		//	if (Mathf.Abs(targetX - x) <= 0.2f) this.x = targetX;
		//	if (Mathf.Abs(targetY - y) <= 0.2f) this.y = targetY;

		//	if (this.x == targetX && this.y == targetY) stop();
		//}

		/// <summary>
		/// 停止
		/// </summary>
		public void stop() {
			runTime = 0; _nextWall = 999;

			realX = x = targetX; realY = y = targetY;

			changeState(State.Idle);
		}

		#endregion

		#region 跳跃计算

		/// <summary>
		/// 因数
		/// </summary>
		float moveFT => 1 / speed; // 移动因数 T

		float jumpFA => -35 * speed * speed / 6; // -35/6t^2, t = 1/speed
		float jumpFB => 41 * speed / 6; // 41/6t, t = 1/speed

		float fallFH => -nextWall; // -h/t^2
		float fallFA => -fallFH * speed * speed; // -h/t^2

		protected virtual float jumpY => jumpFA * runTime * runTime + jumpFB * runTime;
		protected virtual float fallY => fallFA * runTime * runTime;

		#endregion

		#region 更新

		/// <summary>
		/// 更新其他
		/// </summary>
		protected override void updateOthers() {
			base.updateOthers(); updateSpeed();
		}

		/// <summary>
		/// 添加速度
		/// </summary>
		void updateSpeed() {
			if (speed <= 4) speed += 0.001f;
		}

		/// <summary>
		/// 受伤
		/// </summary>
		public bool isHurtting => !map.judgePosBelong(x, y, belong);

		/// <summary>
		/// 更新任意状态
		/// </summary>
		protected override void updateAnyState() {
			base.updateAnyState();
			var dt = Time.deltaTime;

			if (!map.isValidCoord(x, y)) addHPByValue(-9999);

			if (map.judgePosBelong(x, y, belong))
				addHPByValue(recoveryRate * dt);
			else
				addHPByValue(-hurtRate * dt);
		}

		/// <summary>
		/// 更新下落
		/// </summary>
		protected virtual void updateMoving() {
			Debug.Log("update" + state + ": " + runTime + "\nreal: " + realX + ", " + realY +
				", target: " + targetX + ", " + targetY + ", coord: " + x + ", " + y);

			if (x == targetX) return;

			runTime += Time.deltaTime;
			var speed = direction ? this.speed : -this.speed;

			if (runTime >= moveFT) stop();
			else realX += speed * Time.deltaTime;

			//Debug.Log(this + ": " + x + ", target: " + targetX);
			//if (x == targetX) return;

			//var dt = Time.deltaTime;
			//var speed = direction ? this.speed : -this.speed;

			//realX += speed * dt;
		}

		/// <summary>
		/// 更新待命
		/// </summary>
		protected override void _updateIdle() {
			Debug.Log("updateIdle: " + idleTime + "\nreal: " + realX + ", " + realY + 
				", target: " + targetX + ", " + targetY + ", coord: " + x + ", " + y);

			realX = x; realY = y;
			idleTime += Time.deltaTime;
			if (idleTime >= frequency) move();
		}

		/// <summary>
		/// 更新跑步
		/// </summary>
		protected virtual void _updatePanic() {
			Debug.Log("updatePanic: " + idleTime + "\nreal: " + realX + ", " + realY +
				", target: " + targetX + ", " + targetY + ", coord: " + x + ", " + y);

			realX = x = targetX; realY = y = targetY; move();
		}

		/// <summary>
		/// 更新跑步
		/// </summary>
		protected virtual void _updateRunning() {
			var upy = map.getUpperY(x, y, belong);
			if (upy != y) realY = y = targetY = upy;
			updateMoving();
		}

		/// <summary>
		/// 更新爬升
		/// </summary>
		protected virtual void _updateJumping() {
			realY = y + jumpY; updateMoving();
		}

		/// <summary>
		/// 更新下落
		/// </summary>
		protected virtual void _updateFalling() {
			realY = y + fallY; updateMoving();
		}

		/// <summary>
		/// 更新受击
		/// </summary>
		protected override void _updateHitting() {

		}

		/// <summary>
		/// 死亡回调
		/// </summary>
		protected override void _onDie() {
			changeState(State.Dead);
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
			x = (int)initPos.x; y = (int)initPos.y;
			direction = initDir;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		public RuntimeBattler() { }
		public RuntimeBattler(Map map) {
			this.map = map;
			initialize();
		}
		public RuntimeBattler(Map map, int x, int y) : this(map) {
			this.x = x; this.y = y;
		}
	}
}
