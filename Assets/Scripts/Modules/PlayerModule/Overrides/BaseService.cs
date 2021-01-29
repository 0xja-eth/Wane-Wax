using PlayerModule.Data;
using PlayerModule.Services;

/// <summary>
/// 核心服务
/// </summary>
namespace Core.Services {

	/// <summary>
	/// 业务控制类（父类）（单例模式）
	/// </summary>
	public partial class BaseService<T> {

		/// <summary>
		/// 玩家实例
		/// </summary>
		public Player player => PlayerService.Get().player;
	}
}
