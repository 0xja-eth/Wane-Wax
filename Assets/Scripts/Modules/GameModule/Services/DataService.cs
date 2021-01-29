using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using LitJson;

using Core.Data;
using Core.Data.Loaders;
using Core.Systems;
using Core.Services;
using Core.Utils;

using GameModule.Data;

namespace GameModule.Services {

	using Utils;

    /// <summary>
    /// 系统数据控制类
    /// </summary>
    /// <remarks>
    /// 控制游戏系统数据的读取、刷新等
    /// </remarks>
    public partial class DataService : BaseService<DataService> {

		/// <summary>
		/// 缓存文件路径
		/// </summary>
		public const string StaticDataFilename = ".static";
		public const string DynamicDataFilename = ".dynamic";

		/// <summary>
		/// 状态
		/// </summary>
		public enum State {
			Unload,
			LoadingStatic,
			LoadingDynamic,
			Loaded,
		}
		public bool isLoaded() {
			return isState(State.Loaded);
		}

		/// <summary>
		/// 游戏数据
		/// </summary>
		[CacheItem(StaticDataFilename)]
		public GameStaticData staticData { get; protected set; } = new GameStaticData();
		[CacheItem(DynamicDataFilename)]
		public GameDynamicData dynamicData { get; protected set; } = new GameDynamicData();

		/// <summary>
		/// 快捷访问
		/// </summary>
		public GameConfigure configure => staticData.configure;
		public GameDatabase database => staticData.database;

		/// <summary>
		/// 外部系统
		/// </summary>
		StorageSystem storageSys;

		#region 初始化

		/// <summary>
		/// 状态类型
		/// </summary>
		protected override Type stateType => typeof(State);
		
		/// <summary>
		/// 其他初始化工作
		/// </summary>
		protected override void initializeOthers() {
			base.initializeOthers();
			changeState(State.Unload);
		}

		#endregion

		#region 操作控制

		/// <summary>
		/// 读取数据
		/// </summary>
		/// <param name="onError">失败函数</param>
		public void load(UnityAction onError = null) {
			if (isState(State.Unload, State.LoadingStatic))
				getStatic.invoke(onError: onError);
			else if (isState(State.LoadingDynamic))
				getDynamic.invoke(onError: onError);
		}

		/// <summary>
		/// 本地静态数据是否已有缓存
		/// </summary>
		bool isStaticDataCached() {
			return staticData.isLoaded();
		}

		///// <summary>
		///// 读取静态数据
		///// </summary>
		//void loadStaticData() {
		//    var cached = isStaticDataCached();
		//    changeState(State.LoadingStatic);

		//    JsonData data = new JsonData();
		//    data["main_version"] = GameStaticData.LocalMainVersion;
		//    data["sub_version"] = GameStaticData.LocalSubVersion;
		//    data["cached"] = cached; // 是否有缓存

		//    sendRequest(GetStatic, data, onStaticDataLoaded,
		//        unacceptFunc, failFormat: FailTextFormat);
		//}

		///// <summary>
		///// 读取动态数据（读取静态数据之后）
		///// </summary>
		///// <param name="data">静态数据</param>
		//void loadDynamicData() {
		//    changeState(State.LoadingDynamic);
		//    sendRequest(GetDynamic, null, onDynamicDataLoaded,
		//        unacceptFunc, failFormat: FailTextFormat);
		//}

		#endregion

		#region 获取数据

		/// <summary>
		/// 获取
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public T get<T>(int id) where T : BaseData {
			return DataGetter<T>.get(id);
		}
		public BaseData get(Type type, int id) {
			return DataGetter.get(type, id);
		}

		/// <summary>
		/// 获取
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public List<T> getCollection<T>() where T : BaseData {
			return DataGetter<T>.getCollection();
		}
		public List<BaseData> getCollection(Type type, int id) {
			return DataGetter.getCollection(type);
		}

		/// <summary>
		/// 获取下标
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		public int indexOf<T>(int id) where T : BaseData {
			return DataGetter<T>.indexOf(id);
		}
		public int indexOf(Type type, int id) {
			return DataGetter.indexOf(type, id);
		}

		/// <summary>
		/// TypeData类型转化为Tuple
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Tuple<int, string>[] typeDataToTuples(TypeData[] data) {
            var len = data.Length;
            var res = new Tuple<int, string>[len];
            for (int i = 0; i < len; ++i) 
                res[i] = new Tuple<int, string>(
                    data[i].id, data[i].name);
            return res;
        }
		
        #endregion

        #region 回调控制

        ///// <summary>
        ///// 静态数据读取回调
        ///// </summary>
        ///// <param name="data">静态数据</param>
        //void onStaticDataLoaded(JsonData data) {
        //    staticData.load(DataLoader.load(data, "data"));
        //    storageSys.save();
        //    loadDynamicData();
        //}

        ///// <summary>
        ///// 初始化成功回调
        ///// </summary>
        ///// <param name="data">数据</param>
        //void onDynamicDataLoaded(JsonData data) {
        //    dynamicData.load(DataLoader.load(data, "data"));
        //    changeState(State.Loaded);
        //}

        #endregion

    }
}