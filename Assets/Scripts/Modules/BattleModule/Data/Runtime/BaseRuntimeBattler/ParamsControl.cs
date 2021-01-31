using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

namespace BattleModule.Data {

	/// <summary>
	/// 战斗者
	/// </summary>
	public partial class BaseRuntimeBattler {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public double hp { get; protected set; }
		[AutoConvert]
		public double[] baseParams { get; protected set; }
		[AutoConvert]
		public double[] maxParams { get; protected set; }
		[AutoConvert]
		public double[] minParams { get; protected set; }

		#region 属性控制

		#region 属性快捷定义

		/// <summary>
		/// 最大生命值
		/// </summary>
		/// <returns></returns>
		public double mhp => param(BattleConfig.MHPParamId);
		
		#endregion

		#region HP控制

		#region HP变化显示

		/// <summary>
		/// HP该变量
		/// </summary>
		public class DeltaHP {

			public double value = 0; // 值

			public bool critical = false; // 是否暴击
			public bool miss = false; // 是否闪避

			/// <summary>
			/// 构造函数
			/// </summary>
			/// <param name="value"></param>
			public DeltaHP(double value = 0, bool critical = false, bool miss = false) {
				this.value = value; this.critical = critical; this.miss = miss;
			}
		}

		/// <summary>
		/// HP变化量
		/// </summary>
		DeltaHP _deltaHP = null;
		public DeltaHP deltaHP {
			get {
				var res = _deltaHP;
				_deltaHP = null; return res;
			}
		}

		/// <summary>
		/// 设置闪避标志
		/// </summary>
		public void setMissFlag() {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.miss = true;
		}

		/// <summary>
		/// 设置暴击标志
		/// </summary>
		public void setCriticalFlag() {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.critical = true;
		}

		/// <summary>
		/// 设置值变化
		/// </summary>
		/// <param name="value"></param>
		public void setHPChange(double value) {
			_deltaHP = _deltaHP ?? new DeltaHP();
			_deltaHP.value += value;

			Debug.Log("setHPChange: " + value + ", sum: " + _deltaHP.value);
		}

		#endregion

		/// <summary>
		/// 改变HP
		/// </summary>
		/// <param name="value">目标值</param>
		/// <param name="show">是否显示</param>
		public void changeHP(double value, bool show = true) {
			var oriHp = hp;
			hp = Math.Min(Math.Max(value, 0), mhp);
			if (show) setHPChange(hp - oriHp);

			if (isDying()) on(BaseBattlerCallback.Die);
			if (isReviving()) on(BaseBattlerCallback.Revive);

		}

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="value">增加值</param>
		/// <param name="show">是否显示</param>
		public void addHPByValue(double value, bool show = true) {
			changeHP(hp + value, show);
		}

		/// <summary>
		/// 增加HP
		/// </summary>
		/// <param name="rate">增加率</param>
		/// <param name="show">是否显示</param>
		public void addHPByRate(double rate, bool show = true) {
			changeHP((int)Math.Round(hp + mhp * rate), show);
		}

		/// <summary>
		/// 回复所有HP
		/// </summary>
		/// <param name="show">是否显示</param>
		public void recoverAll(bool show = true) {
			changeHP(mhp, show);
		}

		/// <summary>
		/// 即将死亡
		/// </summary>
		/// <returns></returns>
		public bool isDying() {
			return !isState(BaseBattlerState.Dead) && isDead();
		}

		/// <summary>
		/// 是否死亡
		/// </summary>
		/// <returns></returns>
		public bool isReviving() {
			return isState(BaseBattlerState.Dead) && !isDead();
		}

		/// <summary>
		/// 是否死亡
		/// </summary>
		/// <returns></returns>
		public bool isDead() {
			return hp <= 0;
		}

		/// <summary>
		/// 是否失败
		/// </summary>
		/// <returns></returns>
		public bool isLost() {
			return isDead() || isEscaped;
		}

		#endregion

		#region 属性统一接口

		/// <summary>
		/// 默认基本属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public virtual double defaultParam(int paramId) {
			return BattleConfig.DefaultParams[paramId];
		}

		/// <summary>
		/// 默认最小属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public virtual double defaultMinParam(int paramId) {
			return BattleConfig.MinParams[paramId];
		}

		/// <summary>
		/// 默认最大属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public virtual double defaultMaxParam(int paramId) {
			return BattleConfig.MaxParams[paramId];
		}

		/// <summary>
		/// 基本属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public double baseParam(int paramId) {
			if (paramId >= baseParams.Length) return 0;
			return baseParams[paramId];
		}

		/// <summary>
		/// 基本属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public double maxParam(int paramId) {
			if (paramId >= minParams.Length) return -1;
			return maxParams[paramId];
		}

		/// <summary>
		/// 基本属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public double minParam(int paramId) {
			if (paramId >= minParams.Length) return 0;
			return minParams[paramId];
		}

		/// <summary>
		/// Buff附加值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public double buffValue(int paramId) {
			double value = 0;
			foreach (var buff in buffs)
				if (buff.paramId == paramId && !buff.isOutOfDate())
					value += buff.value;
			return value;
		}

		/// <summary>
		/// Buff附加率
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public double buffRate(int paramId) {
			double rate = 1;
			foreach (var buff in buffs)
				if (buff.paramId == paramId && !buff.isOutOfDate())
					rate *= buff.rate;
			return rate;
		}

		/// <summary>
		/// 特性属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual double traitParamVal(int paramId) {
			return 0;
		}

		/// <summary>
		/// 特性属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual double traitParamRate(int paramId) {
			return 1;
		}

		/// <summary>
		/// 额外属性
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns></returns>
		public virtual double extraParam(int paramId) {
			return 0;
		}

		/// <summary>
		/// 属性值
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <returns>属性值</returns>
		public double param(int paramId) {
			var base_ = baseParam(paramId) + traitParamVal(paramId) + buffValue(paramId);
			var rate = buffRate(paramId) * traitParamRate(paramId);
			var extra = extraParam(paramId);
			var max = maxParam(paramId);
			var min = minParam(paramId);

			var val = Math.Round((base_) * rate + extra);

			return Math.Max(Math.Min(val, max), min);
		}

		/// <summary>
		/// 属性相加
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">增加值</param>
		public void addParam(int paramId, double value) {
			addBuff(paramId, value);
		}
		public void addParam(int paramId, double value, double rate) {
			addBuff(paramId, value, rate);
		}

		/// <summary>
		/// 属性相乘
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="rate">比率</param>
		public void multParam(int paramId, double rate) {
			addBuff(paramId, 0, rate);
		}

		/// <summary>
		/// 配置属性
		/// </summary>
		partial void initializeParams() {
			var cnt = BattleConfig.ParamsCount;

			baseParams = new double[cnt];
			minParams = new double[cnt];
			maxParams = new double[cnt];

			for(int i = 0; i < cnt; ++i) {
				baseParams[i] = defaultParam(i);
				maxParams[i] = defaultMaxParam(i);
				minParams[i] = defaultMinParam(i);
			}

			hp = mhp;
		}

		#endregion

		#endregion

		#region 回调

		/// <summary>
		/// 死亡回调
		/// </summary>
		protected virtual void _onDie() {
			changeState(BaseBattlerState.Dead);
		}

		/// <summary>
		/// 复活回调
		/// </summary>
		protected virtual void _onRevive() {
			changeState(BaseBattlerState.Idle);
		}

		#endregion
	}
}
