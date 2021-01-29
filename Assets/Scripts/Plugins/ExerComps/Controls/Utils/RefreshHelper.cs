using System.Collections.Generic;

using Core.Components;

namespace ExerComps.Controls.Utils {

    /// <summary>
    /// 批量刷新组件
    /// </summary>
    public class RefreshHelper : GeneralComponent {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public List<BaseComponent> components;
        
        #region 界面控制

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            foreach (var comp in components)
                refreshView(comp);
        }

        /// <summary>
        /// 刷新单个视窗
        /// </summary>
        /// <param name="comp">组件</param>
        protected virtual void refreshView(BaseComponent comp) {
            comp.requestRefresh(true);
        }

        #endregion
    }
}