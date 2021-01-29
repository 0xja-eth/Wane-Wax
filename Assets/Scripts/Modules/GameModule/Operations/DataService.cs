using System;

using UnityEngine;
using UnityEngine.Events;

using LitJson;

using Core;

using Core.Data;
using Core.Data.Loaders;

/// <summary>
/// 基本系统
/// </summary>
namespace GameModule.Services {

	using Data;

    /// <summary>
    /// 玩家服务类
    /// </summary>
    public partial class DataService {

		/// <summary>
		/// 操作文本设定
		/// </summary>
		public new const string FailTextFormat = "{0}发生错误，错误信息：\n{{0}}"; // \n选择“重试”进行重试，选择“取消”退出游戏。";

		/// <summary>
		/// 静态数据
		/// </summary>
		public class GetStatic : Operation {

			/// <summary>
			/// 请求参数
			/// </summary>
			[AutoConvert]
			public string mainVersion => GameStaticData.LocalMainVersion;
			[AutoConvert]
			public string subVersion => GameStaticData.LocalSubVersion;
			[AutoConvert]
			public bool cached => service.isStaticDataCached();

			/// <summary>
			/// 配置
			/// </summary>
			protected override string failFormat => FailTextFormat;

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override void preprocess() {
				service.changeState(State.LoadingStatic);
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				service.staticData.load(DataLoader.load(data, "data"));
				service.storageSys.save();
			}

			/// <summary>
			/// 执行成功回调
			/// </summary>
			protected override void invokeOnSuccess() {
				service.getDynamic.invoke();
				base.invokeOnSuccess();
			}

			/// <summary>
			/// 本地
			/// </summary>
			protected override void processLocal() {
				if (!cached) throw new Exception("请联网"); // TODO: 异常处理
			}
		}
		public GetStatic getStatic => new GetStatic();

		/// <summary>
		/// 动态数据
		/// </summary>
		public class GetDynamic : Operation {

			/// <summary>
			/// 配置
			/// </summary>
			protected override string failFormat => FailTextFormat;

			/// <summary>
			/// 是否有效
			/// </summary>
			/// <returns></returns>
			protected override void preprocess() {
				service.changeState(State.LoadingDynamic);
			}

			/// <summary>
			/// 成功回调
			/// </summary>
			/// <param name="data"></param>
			protected override void processSuccess(JsonData data) {
				service.dynamicData.load(DataLoader.load(data, "data"));
				service.storageSys.save();
			}

			/// <summary>
			/// 执行成功回调
			/// </summary>
			protected override void invokeOnSuccess() {
				service.changeState(State.Loaded);
				base.invokeOnSuccess();
			}
		}
		public GetDynamic getDynamic => new GetDynamic();
		
	}

}