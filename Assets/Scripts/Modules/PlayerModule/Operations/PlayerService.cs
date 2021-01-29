using System;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core;

using Core.Data;
using Core.Data.Loaders;

using Core.Services;
using Core.Systems;

using Core.Components.Utils;

/// <summary>
/// 基本系统
/// </summary>
namespace PlayerModule.Services {

	using Data;

    /// <summary>
    /// 玩家服务类
    /// </summary>
    public partial class PlayerService {

		/// <summary>
		/// 登陆
		/// </summary>
		public class Login : Operation {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public string uid => GameSystem.Get().uuid;

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() {
				return !service.isPlaying();
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				service.player = DataLoader.load<Player>(data, "player");
				service.invokeOnLoginSuccess();
			}

			/// <summary>
			/// 本地请求
			/// </summary>
			protected override void processLocal() {
				if (service.hasPlayerSave()) service.loadPlayer();
				else service.createPlayer();
				service.invokeOnLoginSuccess();
			}
		}
		public Login login => new Login();

		/// <summary>
		/// 登陆
		/// </summary>
		public class Logout : Operation {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public string uid => GameSystem.Get().uuid;

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() {
				return service.isPlaying();
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				base.processSuccess(data);
				service.invokeOnLogoutSuccess();
			}

			/// <summary>
			/// 本地请求
			/// </summary>
			protected override void processLocal() {
				service.clearPlayer();
				service.invokeOnLogoutSuccess();
			}
		}
		public Logout logout => new Logout();

		/// <summary>
		/// 保存
		/// </summary>
		public class Save : Operation {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public Player player => service.player;

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override bool isValid() {
				return service.isPlaying();
			}

			/// <summary>
			/// 预处理
			/// </summary>
			protected override void preprocess() {
				player.first = false;
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				base.processSuccess(data);
				service.invokeOnSaveSuccess();
			}

			/// <summary>
			/// 本地请求
			/// </summary>
			protected override void processLocal() {
				service.savePlayer();
				service.invokeOnSaveSuccess();
			}
		}
		public Save save => new Save();
		
	}

}