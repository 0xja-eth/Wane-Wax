using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;
using System.Reflection;

using Core.Data.Loaders;

using GameModule.Data;
using GameModule.Services;

namespace Core.Data {

	using Utils;

	/// <summary>
	/// 游戏数据父类
	/// </summary>
	public class DictionaryData<E, T> : BaseData where E : Enum {
		
		/// <summary>
		/// 基本术语
		/// </summary>
		Dictionary<E, T> data = new Dictionary<E, T>();

		/// <summary>
		/// 默认值
		/// </summary>
		T default_ = default;

		/// <summary>
		/// 构造函数
		/// </summary>
		public DictionaryData() : this(default) { }
		public DictionaryData(T default_ ) {
			this.default_ = default_;
			setupData();
		}

		/// <summary>
		/// 配置开关字典
		/// </summary>
		void setupData() {
			data.Clear();
			foreach (var s in Enum.GetValues(typeof(E)))
				data.Add((E)s, default_);
		}

		#region 数据获取

		/// <summary>
		/// 快速操作
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public T this[E type] {
			get => getData(type);
			set => setData(type, value);
		}

		/// <summary>
		/// 获取变量值
		/// </summary>
		public T getData(E type) {
			if (!data.ContainsKey(type)) return default_;
			return data[type];
		}

		/// <summary>
		/// 获取开关值
		/// </summary>
		public T setData(E type, T val) {
			return data[type] = val;
		}

		#endregion

		#region 数据读取

		/// <summary>
		/// 读取自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			this.data.Clear();
			var data = DataLoader.load(json, "data");

			if (data != null) {
				data.SetJsonType(JsonType.Object);
				// 取出键值对（键：枚举值，值：T类型值）
				foreach (KeyValuePair<string, JsonData> pair in data) {
					var key = (E)Enum.ToObject(typeof(E), int.Parse(pair.Key));
					var value = DataLoader.load<T>(pair.Value);

					this.data.Add(key, value);
				}
			}
		}

		/// <summary>
		/// 转化自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);

			var data = new JsonData();
			data.SetJsonType(JsonType.Object);

			foreach (var pair in this.data)
				data[pair.Key.GetHashCode().ToString()] = DataLoader.convert(pair.Value);

			json["data"] = data;
		}

		#endregion

	}
}