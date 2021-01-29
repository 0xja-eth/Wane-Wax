using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using Core.Services;

namespace AssetModule.Services {
	
	/// <summary>
	/// 资源设置
	/// </summary>
	public class AssetSetting {

		/// <summary>
		/// 属性
		/// </summary>
		//public AssetConfig.Type name; // 资源类型名
		public string path; // 资源路径
		public string format; // 资源文件名格式

		/// <summary>
		/// 构造函数
		/// </summary>
		public AssetSetting(string path, string format) {
			this.path = path; this.format = format;
		}
		public AssetSetting(params string[] paths) {
			var last = paths.Length - 1;
			format = paths[last];

			paths = paths.Take(last).ToArray();
			path = Path.Combine(paths);
		}
	}

	/// <summary>
	/// 缓存池对象
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class AssetCachePool<T> where T : class {

		/// <summary>
		/// 缓存池
		/// </summary>
		static Dictionary<string, T> cache = new Dictionary<string, T>();

		/// <summary>
		/// 获取
		/// </summary>
		/// <param name="key">路径</param>
		/// <returns></returns>
		public static T get(string key) {
			if (cache.ContainsKey(key)) return cache[key];
			return null;
		}

		/// <summary>
		/// 设置
		/// </summary>
		/// <param name="key">路径</param>
		/// <param name="obj">资源</param>
		/// <returns></returns>
		public static T set(string key, T obj) {
			return cache[key] = obj;
		}

		/// <summary>
		/// 是否包含
		/// </summary>
		/// <param name="key">路径</param>
		/// <returns></returns>
		public static bool contains(string key) {
			return cache.ContainsKey(key);
		}
	}

	/// <summary>
	/// 资源服务
	/// </summary>
	public partial class AssetService : BaseService<AssetService> {
		
		/// <summary>
		/// 预设资源路径/名称
		/// </summary>
		public const string SystemPath = "System/";

		#region 加载资源封装

		/// <summary>
		/// 读取资源
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns></returns>
		public T loadAsset<T>(string path, string fileName) where T : Object {
			var key = path + fileName;
			Debug.Log("LoadAsset<" + typeof(T) + "> from " + key);

			if (!AssetCachePool<T>.contains(key)) {
				var obj = Resources.Load<T>(key);
				return AssetCachePool<T>.set(key, obj);
			}
			return AssetCachePool<T>.get(key);
		}

		/// <summary>
		/// 读取资源（多个，组合资源）
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns></returns>
		public T[] loadAssets<T>(string path, string fileName) where T : Object {
			var key = path + fileName;
			Debug.Log("LoadAssets<" + typeof(T) + "> from " + key);

			if (!AssetCachePool<T[]>.contains(key)) {
				var obj = Resources.LoadAll<T>(key);
				return AssetCachePool<T[]>.set(key, obj);
			}
			return AssetCachePool<T[]>.get(key);
		}

		/// <summary>
		/// 读取2D纹理
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>2D纹理</returns>
		public Texture2D loadTexture2D(string path, string fileName) {
			return loadAsset<Texture2D>(path, fileName);
		}
		/// <param name="name">文件主体名称</param>
		/// <param name="id">序号</param>
		Texture2D loadTexture2D(string path, string name, int id) {
			return loadTexture2D(path, name + "_" + id);
		}

		/// <summary>
		/// 读取音频
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>音频文件</returns>
		public AudioClip loadAudio(string path, string fileName) {
			return loadAsset<AudioClip>(path, fileName);
		}
		/// <param name="id">文件id</param>
		/// <returns>音频文件</returns>
		public AudioClip loadAudio(string path, string name, int id) {
			return loadAudio(path, name + "_" + id);
		}

		/// <summary>
		/// 读取动画
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="fileName">文件名</param>
		/// <returns>音频文件</returns>
		public AnimationClip loadAnimation(string path, string fileName) {
			return loadAsset<AnimationClip>(path, fileName);
		}
		/// <param name="id">文件id</param>
		/// <returns>音频文件</returns>
		public AnimationClip loadAnimation(string path, string name, int id) {
			return loadAnimation(path, name + "_" + id);
		}
		public AnimationClip loadAnimation(int id) {
			return loadAsset<AnimationClip>(AssetConfig.Animation, id);
		}

		#region 纹理资源处理

		/// <summary>
		/// 根据尺寸获取纹理截取区域 
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="xSize">单个图标宽度</param>
		/// <param name="ySize">单个图标高度</param>
		/// <param name="xIndex">图标X索引</param>
		/// <param name="yIndex">图标Y索引</param>
		/// <returns>返回要截取的区域</returns>
		public Rect getRectBySize(Texture2D texture,
			int xSize, int ySize, int xIndex, int yIndex) {
			int h = texture.height;
			int x = xIndex * xSize, y = h - (yIndex + 1) * ySize;

			return new Rect(x, y, xSize, ySize);
		}
		/// <param name="index">图标索引</param>
		public Rect getRectBySize(Texture2D texture,
			int xSize, int ySize, int index) {
			int w = texture.width; int cols = w / xSize;
			int xIndex = index % cols, yIndex = index / cols;

			return getRectBySize(texture, xSize, ySize, xIndex, yIndex);
		}

		/// <summary>
		/// 根据个数获取纹理截取区域 
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="xCnt">X坐标图标数</param>
		/// <param name="yCnt">Y坐标图标数</param>
		/// <param name="xIndex">图标X索引</param>
		/// <param name="yIndex">图标Y索引</param>
		/// <returns></returns>
		public Rect getRectByCnt(Texture2D texture,
			int xCnt, int yCnt, int xIndex, int yIndex) {
			int w = texture.width, h = texture.height;
			float xSize = w * 1f / xCnt, ySize = h * 1f / yCnt;
			float x = xIndex * xSize, y = h - (yIndex + 1) * ySize;

			return new Rect(x, y, xSize, ySize);
		}
		/// <param name="index">图标索引</param>
		public Rect getRectByCnt(Texture2D texture,
			int xCnt, int yCnt, int index) {
			int xIndex = index % xCnt, yIndex = index / xCnt;

			return getRectBySize(texture, xCnt, yCnt, xIndex, yIndex);
		}

		/// <summary>
		/// 生成精灵
		/// </summary>
		/// <param name="texture">纹理</param>
		/// <param name="rect">截取矩形</param>
		/// <returns></returns>
		public Sprite genSprite(Texture2D texture, Rect rect = default) {
			if (rect == default)
				rect = new Rect(0, 0, texture.width, texture.height);
			return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
		}
		public Sprite genSpriteBySize(Texture2D texture,
			int xSize, int ySize, int xIndex, int yIndex) {

			var rect = getRectBySize(texture, xSize, ySize, xIndex, yIndex);
			return genSprite(texture, rect);
		}
		public Sprite genSpriteBySize(Texture2D texture,
			int xSize, int ySize, int index) {

			var rect = getRectBySize(texture, xSize, ySize, index);
			return genSprite(texture, rect);
		}
		public Sprite genSpriteByCnt(Texture2D texture,
			int xCnt, int yCnt, int xIndex, int yIndex) {

			var rect = getRectByCnt(texture, xCnt, yCnt, xIndex, yIndex);
			return genSprite(texture, rect);
		}
		public Sprite genSpriteByCnt(Texture2D texture,
			int xCnt, int yCnt, int index) {

			var rect = getRectByCnt(texture, xCnt, yCnt, index);
			return genSprite(texture, rect);
		}

		#endregion

		#endregion

		#region 加载单个资源

		/// <summary>
		/// 读取资源
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="setting">资源设置</param>
		/// <param name="id">编号</param>
		/// <returns></returns>
		public T loadAsset<T>(AssetSetting setting, int id = 0) where T : Object {

			var fileName = string.Format(setting.format, id);
			return loadAsset<T>(setting.path, fileName);
		}

		/// <summary>
		/// 读取资源（多个，组合资源）
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="setting">资源设置</param>
		/// <param name="id">编号</param>
		/// <returns></returns>
		public T[] loadAssets<T>(AssetSetting setting, int id = 0) where T : Object {

			var fileName = string.Format(setting.format, id);
			return loadAssets<T>(setting.path, fileName);
		}

		/// <summary>
		/// 从资源组读取资源
		/// </summary>
		/// <typeparam name="T">资源类型</typeparam>
		/// <param name="setting">资源设置</param>
		/// <param name="index">下标</param>
		/// <param name="id">编号</param>
		/// <returns></returns>
		public T loadAssetFromGroup<T>(AssetSetting setting, int index, int id = 0) where T : Object {
			var assets = loadAssets<T>(setting, id);
			if (index < 0 || assets.Length >= index) return null;

			return assets[index];
		}

		#endregion

		#region 加载组合资源

		/// <summary>
		/// 获取组合资源所占的区域（图标类资源）
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <param name="path">路径</param>
		/// <param name="name">文件名</param>
		/// <param name="xSize">X尺寸</param>
		/// <param name="ySize">Y尺寸</param>
		/// <returns>返回对应图标索引的矩形区域</returns>
		public Rect getGroupAssetsRect(int index,
			string path, string name, int xSize, int ySize) {
			return getRectBySize(loadTexture2D(path, name), xSize, ySize, index);
		}

		/// <summary>
		/// 读取组合资源精灵（图标类资源）
		/// </summary>
		/// <param name="index">图标索引</param>
		/// <param name="path">路径</param>
		/// <param name="name">文件名</param>
		/// <param name="xSize">X尺寸</param>
		/// <param name="ySize">Y尺寸</param>
		/// <returns>返回对应图标索引的精灵</returns>
		public Sprite getGroupAssetsSprite(int index,
			string path, string name, int xSize, int ySize) {
			var texture = loadTexture2D(path, name);
			var rect = getRectBySize(texture, xSize, ySize, index);
			return genSprite(texture, rect);
		}
		
		#endregion

		#region 自定义读取函数

		#endregion

	}
}
