using System;

using Core.Data;
using Core.Data.Loaders;

/// <summary>
/// 游戏模块数据
/// </summary>
namespace GameModule.Data {

	/// <summary>
	/// 游戏版本数据
	/// </summary>
	public class GameVersionData : BaseData {

		/// <summary>
		/// 更新日志格式
		/// </summary>
		public const string UpdateNoteFormat = "版本号：{1}.{2}\n更新日期：{0}\n更新内容：\n{3}\n";

		/// <summary>
		/// 属性
		/// </summary>
		[AutoConvert]
		public string mainVersion { get; protected set; }
		[AutoConvert]
		public string subVersion { get; protected set; }
		[AutoConvert]
		public string updateNote { get; protected set; }
		[AutoConvert]
		public DateTime updateTime { get; protected set; }
		[AutoConvert]
		public string description { get; protected set; }

		/// <summary>
		/// 生成单个版本的更新日志
		/// </summary>
		/// <returns>更新日志文本</returns>
		public string generateUpdateNote() {
			string time = updateTime.ToString(DataLoader.SystemDateFormat);
			return string.Format(UpdateNoteFormat, mainVersion,
				subVersion, time, updateNote, description);
		}

	}


}