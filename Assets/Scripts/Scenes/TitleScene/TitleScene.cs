using UnityEngine;

using Core;

using Core.Systems;
using Core.Components;

namespace Scenes.TitleScene {

	/// <summary>
	/// 标题场景
	/// </summary>
	public class TitleScene : BaseScene {

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			if (Input.anyKeyDown) sceneSys.gotoScene(SceneConfig.Type.GameScene);
		}
	}
}
