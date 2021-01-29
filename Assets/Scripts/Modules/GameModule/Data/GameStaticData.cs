using System;
using System.Collections.Generic;

using UnityEngine;

using LitJson;

using Core;

using Core.Data;
using Core.Data.Loaders;

namespace GameModule.Data {
	
	using Services;

	/// <summary>
	/// 游戏静态配置数据
	/// </summary>
	public class GameStaticData : BaseData {

		/// <summary>
		/// 本地版本
		/// </summary>
		//public string localVersion = PlayerSettings.bundleVersion;
		public const string LocalMainVersion = DeployConfig.LocalMainVersion; // "0.3.2";
		public const string LocalSubVersion = DeployConfig.LocalSubVersion; // "20200527";

		/// <summary>
		/// 后台版本
		/// </summary>
		[AutoConvert]
		public GameVersionData curVersion { get; protected set; }

		/// <summary>
		/// 历史版本
		/// </summary>
		[AutoConvert]
		public List<GameVersionData> lastVersions { get; protected set; }

		/// <summary>
		/// 游戏配置（用语、配置项）
		/// </summary>
		public GameConfigure configure { get; protected set; }

		/// <summary>
		/// 游戏资料（数据库）
		/// </summary>
		public GameDatabase database { get; protected set; }

		/// <summary>
		/// 获取实例
		/// </summary>
		public static GameStaticData Get => DataService.Get().staticData;

		/// <summary>
		/// 读取标志
		/// </summary>
		bool loaded = false;
		public bool isLoaded() { return loaded; }

		/// <summary>
		/// 是否需要ID
		/// </summary>
		protected override bool idEnable() { return false; }

		/// <summary>
		/// 生成更新日志
		/// </summary>
		/// <returns>更新日志文本</returns>
		public string generateUpdateNote() {
			string updateNote = "当前版本：\n" + curVersion.generateUpdateNote();
			updateNote += "\n历史版本：\n";
			foreach (var ver in lastVersions)
				updateNote += ver.generateUpdateNote();
			return updateNote;
		}

		/// <summary>
		/// 数据加载
		/// </summary>
		/// <param name="json">数据</param>
		protected override void loadCustomAttributes(JsonData json) {
			base.loadCustomAttributes(json);

			if (curVersion == null) return;
			Debug.Log("curVersion: " + curVersion.generateUpdateNote());

			// 如果没有版本变更且数据已读取（本地缓存），则直接返回
			if (curVersion.mainVersion == LocalMainVersion &&
				curVersion.subVersion == LocalSubVersion && loaded) return;

			configure = DataLoader.load(configure, json, "configure");
			database = DataLoader.load(database, json, "data");

			loaded = true;
		}

		/// <summary>
		/// 转化自定义属性
		/// </summary>
		/// <param name="json"></param>
		protected override void convertCustomAttributes(ref JsonData json) {
			base.convertCustomAttributes(ref json);
			json["configure"] = DataLoader.convert(configure);
			json["data"] = DataLoader.convert(database);
		}
	}
}