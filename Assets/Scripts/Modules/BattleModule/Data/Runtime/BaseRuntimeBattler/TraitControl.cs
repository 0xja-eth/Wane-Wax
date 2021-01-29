using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using LitJson;

using Core.Data;
using Core.Data.Loaders;

namespace BattleModule.Data {

	using Services;
	using Processors;

	/// <summary>
	/// 战斗者
	/// </summary>
	public abstract partial class BaseRuntimeBattler<T> :
		BaseRuntimeBattler where T : TraitData {

		#region 特性控制

		/// <summary>
		/// 特性对象
		/// </summary>
		/// <returns></returns>
		public virtual List<ITraitsObject<T>> traitObjects() {
			return new List<ITraitsObject<T>>();
		}

		/// <summary>
		/// 获取所有特性
		/// </summary>
		/// <returns></returns>
		public List<T> traits() {
			var res = new List<T>();
			var objs = traitObjects();

			foreach (var obj in objs) 
				res.AddRange(obj.traits);

			return res;
		}

		/// <summary>
		/// 获取特定特性
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <returns>返回符合条件的特性</returns>
		public List<T> filterTraits(Enum code) {
			return traits().FindAll(trait => trait.code == code.GetHashCode());
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public List<T> filterTraits<T1>(Enum code, T1 param, int id = 0) {
			return traits().FindAll(trait => trait.code == code.GetHashCode()
				&& Equals(trait.get<T1>(id), param));
		}

		/// <summary>
		/// 特性值求和
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <param name="index">特性参数索引</param>
		/// <param name="base_">求和基础值</param>
		/// <returns></returns>
		public double sumTraits(Enum code, int index = 0, double base_ = 0) {
			return sumTraits(filterTraits(code), index, base_);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public double sumTraits<T1>(Enum code, T1 param, int id = 0, int index = 1, double base_ = 0) {
			return sumTraits(filterTraits(code, param, id), index, base_);
		}
		public double sumTraits(List<T> traits, int index = 0, double base_ = 0) {
			var res = base_;
			foreach (var trait in traits)
				res += trait.get(index, 0);
			return res;
		}

		/// <summary>
		/// 特性值求积
		/// </summary>
		/// <param name="code">特性枚举</param>
		/// <param name="index">特性参数索引</param>
		/// <param name="base_">概率基础值（默认为100，不需要修改）</param>
		/// <returns></returns>
		public double multTraits(Enum code, int index = 1, int base_ = 100) {
			return multTraits(filterTraits(code), index, base_);
		}
		/// <param name="param">参数取值</param>
		/// <param name="id">参数下标</param>
		public double multTraits<T1>(Enum code, T1 param, int id = 0, int index = 2, int base_ = 100) {
			return multTraits(filterTraits(code, param, id), index, base_);
		}
		public double multTraits(List<T> traits, int index = 1, int base_ = 100) {
			var res = 1.0;
			foreach (var trait in traits)
				// 特性的第一项数据为比率的附加值，因此需要加上base_
				res *= (base_ + trait.get(index, 0)) / 100.0;
			return res;
		}

		#endregion

		#region 处理器

		/// <summary>
		/// 获取特性处理器
		/// </summary>
		/// <returns></returns>
		BaseTraitsProcessor<T> _traitProcessor = null;
		BaseTraitsProcessor<T> traitProcessor {
			get {
				if (_traitProcessor == null) {
					var tType = BattleService.Get().traitsProcessorType<T>();
					_traitProcessor = Activator.CreateInstance(
						tType, this) as BaseTraitsProcessor<T>;
				}
				return _traitProcessor;
			}
		}

		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		protected override void initializeCallbacks() {
			base.initializeCallbacks();

			cbManager.registerObject(traitProcessor);
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		protected override void initializeStates() {
			base.initializeStates();

			stateMachine.registerObject(traitProcessor);
		}

	}
}
