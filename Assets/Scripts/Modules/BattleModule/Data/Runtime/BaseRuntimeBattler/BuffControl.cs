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
	public abstract partial class BaseRuntimeBattler : RuntimeData {

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public List<RuntimeBuff> buffs { get; protected set; } 
			= new List<RuntimeBuff>();

		#region Buff控制

		/// <summary>
		/// 状态是否改变
		/// </summary>
		List<RuntimeBuff> _addedBuffs = new List<RuntimeBuff>();
		public List<RuntimeBuff> addedBuffs {
			get {
				var res = _addedBuffs;
				_addedBuffs.Clear(); return res;
			}
		}

		#region Buff变更

		/// <summary>
		/// 添加Buff
		/// </summary>
		/// <param name="paramId">属性ID</param>
		/// <param name="value">变化值</param>
		/// <param name="rate">变化率</param>
		/// <param name="turns">持续回合</param>
		/// <returns>返回添加的Buff</returns>
		public RuntimeBuff addBuff(int paramId,
			double value = 0, double rate = 1, int turns = 0) {
			return addBuff(new RuntimeBuff(paramId, value, rate, turns));
		}
		public RuntimeBuff addBuff(RuntimeBuff buff) {
			buffs.Add(buff);
			on(BaseBattlerCallback.BuffAdded, buff);
			_addedBuffs.Add(buff);

			return buff;
		}

		/// <summary>
		/// 移除Buff
		/// </summary>
		/// <param name="index">Buff索引</param>
		public void removeBuff(int index, bool force = false) {
			var buff = buffs[index];
			buffs.RemoveAt(index);
			_addedBuffs.Remove(buff);

			on(BaseBattlerCallback.BuffRemoved, buff, force);
		}
		/// <param name="buff">Buff对象</param>
		public void removeBuff(RuntimeBuff buff, bool force = false) {
			buffs.Remove(buff);
			_addedBuffs.Remove(buff);

			on(BaseBattlerCallback.BuffRemoved, buff, force);
		}

		/// <summary>
		/// 移除多个满足条件的Buff
		/// </summary>
		/// <param name="p">条件</param>
		public void removeBuffs(Predicate<RuntimeBuff> p, bool force = true) {
			for (int i = buffs.Count - 1; i >= 0; --i)
				if (p(buffs[i])) removeBuff(i, force);
		}

		/// <summary>
		/// 清除所有Debuff
		/// </summary>
		public void removeDebuffs(bool force = true) {
			removeBuffs(buff => buff.isDebuff(), force);
		}

		/// <summary>
		/// 清除所有Buff
		/// </summary>
		public void clearBuffs(bool force = true) {
			for (int i = buffs.Count - 1; i >= 0; --i)
				removeBuff(i, force);
		}

		/// <summary>
		/// 是否处于指定条件的Buff
		/// </summary>
		public bool containsBuff(int paramId) {
			return buffs.Exists(buff => buff.paramId == paramId);
		}
		public bool containsBuff(Predicate<RuntimeBuff> p) {
			return buffs.Exists(p);
		}

		/// <summary>
		/// 重置BUFF
		/// </summary>
		partial void resetBuffs() { clearBuffs(); }

		#endregion

		#region Buff判断

		/// <summary>
		/// 是否处于指定条件的Debuff
		/// </summary>
		public bool containsDebuff(int paramId) {
			return buffs.Exists(buff => buff.isDebuff() && buff.paramId == paramId);
		}

		/// <summary>
		/// 是否存在Debuff
		/// </summary>
		public bool anyDebuff() {
			return buffs.Exists(buff => buff.isDebuff());
		}

		/// <summary>
		/// 获取指定条件的Buff
		/// </summary>
		public RuntimeBuff getBuff(int paramId) {
			return buffs.Find(buff => buff.paramId == paramId);
		}
		public RuntimeBuff getBuff(Predicate<RuntimeBuff> p) {
			return buffs.Find(p);
		}

		/// <summary>
		/// 获取指定条件的Buff（多个）
		/// </summary>
		public List<RuntimeBuff> getBuffs(Predicate<RuntimeBuff> p) {
			return buffs.FindAll(p);
		}

		#endregion

		#endregion

		#region 回调

		/// <summary>
		/// 处理状态回合结束
		/// </summary>
		partial void processBuffsRoundEnd() {
			for (int i = buffs.Count - 1; i >= 0; --i) {
				var buff = buffs[i]; buff.onRoundEnd();
				if (buff.isOutOfDate()) removeBuff(i);
			}
		}

		/// <summary>
		/// BUFF添加回调
		/// </summary>
		protected virtual void _onBuffAdded(RuntimeBuff buff) { }

		/// <summary>
		/// BUFF移除回调
		/// </summary>
		protected virtual void _onBuffRemoved(RuntimeBuff buff, bool force = false) { }

		#endregion

	}
}
