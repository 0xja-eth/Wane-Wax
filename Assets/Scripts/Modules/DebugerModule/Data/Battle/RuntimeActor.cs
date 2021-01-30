using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using BattleModule;
using BattleModule.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 玩家
	/// </summary>
	public class RuntimeActor : RuntimeBattler {

		/// <summary>
		/// 角色
		/// </summary>
		public override bool isActor => true;

		/// <summary>
		/// 初始位置
		/// </summary>
		public override Vector2 initPos => new Vector2(mapX / 2, mapY / 2);
		public override bool initDir => true;
		
		/// <summary>
		/// 初始化
		/// </summary>
		public RuntimeActor() { }
		public RuntimeActor(Map map) : base(map) { }
		public RuntimeActor(Map map, int x, int y) : base(map, x, y) { }

	}
}
