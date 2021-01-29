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
    public partial class PlayerService : BaseService<PlayerService> {

		/// <summary>
		/// 回调函数
		/// </summary>
		public UnityEvent onLoginSuccess { get; protected set; } = new UnityEvent();
		public UnityEvent onLogoutSuccess { get; protected set; } = new UnityEvent();
		public UnityEvent onSaveSuccess { get; protected set; } = new UnityEvent();

		/// <summary>
		/// 存档名
		/// </summary>
		const string PlayerSaveFilename = ".player";

		/// <summary>
		/// 玩家
		/// </summary>
		Player _player = null;
		public new Player player {
			get => _player;
			protected set { _player = value; }
		}

		/// <summary>
		/// 外部系统设置
		/// </summary>
		NetworkSystem networkSys;
		SceneSystem sceneSys;

		//#region 服务器操作

		///// <summary>
		///// 服务器登陆
		///// </summary>
		//void loginServer() {
		//	NetworkSystem.RequestObject.SuccessAction _onSuccess =
		//		(data) => {
		//			player = DataLoader.load<Player>(data, "player");
		//			invokeOnLogoutSuccess();
		//		};

		//	loginServer(_onSuccess);
		//}
		//void loginServer(
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

		//	JsonData data = new JsonData();
		//	data["uid"] = DataLoader.convert(gameSys.uuid);

		//	sendRequest(PlayerConfig.Login, data, onSuccess, onError);
		//}

		///// <summary>
		///// 服务器登出
		///// </summary>
		//void logoutServer() {
		//	NetworkSystem.RequestObject.SuccessAction _onSuccess =
		//		(_) => invokeOnLogoutSuccess();

		//	logoutServer(_onSuccess);
		//}
		//void logoutServer(
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

		//	JsonData data = new JsonData();
		//	data["uid"] = DataLoader.convert(gameSys.uuid);

		//	sendRequest(PlayerConfig.Logout, data, onSuccess, onError);
		//}

		///// <summary>
		///// 服务器保存
		///// </summary>
		//void saveServer() {
		//	NetworkSystem.RequestObject.SuccessAction _onSuccess =
		//		(_) => invokeOnSaveSuccess();

		//	saveServer(_onSuccess);
		//}
		//void saveServer(
		//	NetworkSystem.RequestObject.SuccessAction onSuccess, UnityAction onError = null) {

		//	JsonData data = new JsonData();
		//	data["player"] = DataLoader.convert(player);

		//	sendRequest(PlayerConfig.Save, data, onSuccess, onError);
		//}

		//#endregion

		#region 本地操作

		///// <summary>
		///// 本地登陆
		///// </summary>
		//void loginLocal() {
		//	if (hasPlayerSave()) loadPlayer();
		//	else createPlayer();
		//	invokeOnLoginSuccess();
		//}

		///// <summary>
		///// 本地登出
		///// </summary>
		//void logoutLocal() {
		//	clearPlayer();
		//	invokeOnLogoutSuccess();
		//}

		///// <summary>
		///// 本地保存
		///// </summary>
		//void saveLocal() {
		//	savePlayer();
		//	invokeOnSaveSuccess();
		//}

		/// <summary>
		/// 是否存在存档
		/// </summary>
		/// <returns></returns>
		public bool hasPlayerSave() {
			return StorageSystem.hasFile(PlayerSaveFilename);
		}

		/// <summary>
		/// 创建角色
		/// </summary>
		/// <param name="name"></param>
		public void createPlayer() {
			player = new Player(true);
			savePlayer();
		}

		/// <summary>
		/// 读取角色
		/// </summary>
		/// <param name="fileName"></param>
		public void loadPlayer() {
			StorageSystem.loadObjectFromFile(ref _player, PlayerSaveFilename);
		}

		/// <summary>
		/// 保存角色
		/// </summary>
		/// <param name="fileName"></param>
		public void savePlayer() {
			StorageSystem.saveObjectIntoFile(player, PlayerSaveFilename);
		}

		/// <summary>
		/// 清除角色
		/// </summary>
		public void clearPlayer() {
			player = null;
		}

		#endregion

		#region 数据判断

		/// <summary>
		/// 是否存在玩家
		/// </summary>
		/// <returns></returns>
		public bool isPlaying() {
			return player != null;
		}

		/// <summary>
		/// 是否首次游戏
		/// </summary>
		/// <returns></returns>
		public bool isFirst() {
			return isPlaying() && player.first;
		}
		
		#endregion

		#region 回调函数

		/// <summary>
		/// 登陆回调
		/// </summary>
		void invokeOnLoginSuccess() {
			onLoginSuccess.Invoke();
		}

		/// <summary>
		/// 登出回调
		/// </summary>
		void invokeOnLogoutSuccess() {
			onLogoutSuccess.Invoke();
		}

		/// <summary>
		/// 保存回调
		/// </summary>
		void invokeOnSaveSuccess() {
			onSaveSuccess.Invoke();
		}

		#endregion
	}

}