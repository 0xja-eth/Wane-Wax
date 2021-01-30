using System;
using System.Collections.Generic;

using UnityEngine;

using Core.Data;

using BattleModule;
using BattleModule.Data;

namespace DebugerModule.Data {

	/// <summary>
	/// 敌人
	/// </summary>
	public class RuntimeEnemy : RuntimeBattler {

		/// <summary>
		/// 敌人
		/// </summary>
		public override bool isEnemy => true;

		/// <summary>
		/// 初始位置
		/// </summary>
		public override Vector2 initPos => new Vector2(mapX / 2, mapY / 2 - 1);
		public override bool initDir => false;

		/// <summary>
		/// 初始化
		/// </summary>
		public RuntimeEnemy() { }
		public RuntimeEnemy(Map map) : base(map) { }
		public RuntimeEnemy(Map map, int x, int y) : base(map, x, y) { }

	}
}
