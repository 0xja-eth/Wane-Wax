using System;

using Core.Data;
using Core.Data.Loaders;

namespace GameModule.Utils {
	using LitJson;
	using Services;

	/// <summary>
	/// 数据获取类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TupleList : BaseData {

		/// <summary>
		/// 数据
		/// </summary>
		Tuple<int, string>[] data;

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string this[int id] {
			get => get(id);
		}
		public string this[Enum id] {
			get => get(id.GetHashCode());
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string get(int id) {
			return DataGetter.collectionGet(data, id).Item2;
		}

		/// <summary>
		/// 通过下标获取
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string getByIndex(int index) {
			return data[index].Item2;
		}

		/// <summary>
		/// 能否使用ID
		/// </summary>
		/// <returns></returns>
		protected override bool idEnable() { return false; }

		#region 数据转化

		/// <summary>
		/// 加载数据
		/// </summary>
		/// <param name="json"></param>
		protected override void loadCustomAttributes(JsonData json) {
			data = DataLoader.load<Tuple<int, string>[]>(json);
		}

		/// <summary>
		/// 转化数据
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			json = DataLoader.convert(data);
		}

		#endregion
	}
}
