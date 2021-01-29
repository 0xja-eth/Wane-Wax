
using Core.Data;

namespace GameModule.Data {

	using Services;

	/// <summary>
	/// 游戏系统配置数据
	/// </summary>
	public partial class GameConfigure : BaseData { // DictionaryData<Configuration, string> {
		
		/// <summary>
		/// 获取实例
		/// </summary>
		public static GameConfigure Get => DataService.Get().configure;

		/// <summary>
		/// /ID是否可用
		/// </summary>
		/// <returns></returns>
		protected override bool idEnable() {
			return false;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		// public GameConfigure() : base("") { }
	}

}