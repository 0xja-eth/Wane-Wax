using UnityEngine;
using UnityEngine.Events;

using Core;

using Core.Data;
using Core.Services;
using Core.Systems;

using Core.Components.Utils;

using PlayerModule.Services;

/// <summary>
/// 基本系统
/// </summary>
namespace GameModule.Services {

	using Data;

	/// <summary>
	/// Exermon控制类
	/// </summary>
	/// <remarks>
	/// 控制整个游戏进程（游戏内流程）
	/// </remarks>
	public class GameService : BaseService<GameService> {

		/// <summary>
		/// 缓存文件路径
		/// </summary>
		public const string ConfigDataFilename = ".config";

		/// <summary>
		/// 游戏配置（设置）
		/// </summary>
		[CacheItem(ConfigDataFilename)]
		public GameSettings settings { get; protected set; } = new GameSettings();

		/// <summary>
		/// 回调设置
		/// </summary>
		public UnityEvent onNewGame { get; protected set; } = new UnityEvent();
		public UnityEvent onLoadGame { get; protected set; } = new UnityEvent();
		public UnityEvent onReconnect { get; protected set; } = new UnityEvent();

		/// <summary>
		/// 外部系统
		/// </summary>
		SceneSystem sceneSys;
        StorageSystem storageSys;

		PlayerService playerSer;

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOthers() {
			base.initializeOthers();
			gameSys.onReconnected.AddListener(onReconnected);
			playerSer.onLoginSuccess.AddListener(onPlayerLoginSuccess);
		}

		#region 流程控制

		/// <summary>
		/// 开始游戏（根据创建角色的first属性判断是新游戏还是继续游戏）
		/// </summary>
		public void startGame() {
            storageSys.save();
			playerSer.login.invoke();
        }

		/// <summary>
		/// 新游戏（未有角色的玩家）
		/// </summary>
		void newGame() {
			// TODO: 新游戏
			onNewGame.Invoke();
        }

        /// <summary>
        /// 继续游戏（已经有角色的玩家）
        /// </summary>
        void loadGame() {
			// TODO: 读取游戏
			onLoadGame.Invoke();
		}

        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void backToMenu() {
			playerSer.logout.invoke();
			sceneSys.gotoScene(SceneConfig.Type.TitleScene);
        }

		/// <summary>
		/// 结束游戏
		/// </summary>
		public void exitGame() {
			playerSer.logout.invoke();
			gameSys.terminate();
		}

		/// <summary>
		/// 结束游戏
		/// </summary>
		public void saveGame() {
			playerSer.save.invoke();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 登陆成功回调
		/// </summary>
		void onPlayerLoginSuccess() {
			if (playerSer.isFirst()) newGame();
			else loadGame();
		}

		/// <summary>
		/// 重连回调
		/// </summary>
		void onReconnected() {
			onReconnect.Invoke();
			//Debug.Log("onReconnected: " + playerSer.isPlaying());
			//if (playerSer.isLogined()) playerSer.reconnect();
		}

        #endregion
    }

}