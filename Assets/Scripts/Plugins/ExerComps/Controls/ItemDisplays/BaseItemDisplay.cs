using System;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.Events;

using ItemModule.Data;
using ItemModule.Services;

namespace ExerComps.Controls.ItemDisplays {

    /// <summary>
    /// 基本物品显示
    /// </summary>
    public class BaseItemDisplay : ItemDisplay<BaseItem> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name;
        public Image icon; // 图片

        /// <summary>
        /// 绘制函数
        /// </summary>
        Dictionary<int, UnityAction<BaseItem>> drawFuncs = 
            new Dictionary<int, UnityAction<BaseItem>>();

		/// <summary>
		/// 外部系统定义
		/// </summary>
		protected ItemService itemSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            initializeDrawFuncs();
        }

        /// <summary>
        /// 初始化绘制函数
        /// </summary>
        protected virtual void initializeDrawFuncs() {}

        #endregion

        #region 绘制函数控制

        /// <summary>
        /// 注册物品类型
        /// </summary>
        /// <typeparam name="T">物品类型</typeparam>
        /// <param name="func">绘制函数</param>
        public void registerItemType<T>(UnityAction<T> func) where T : BaseItem{
            UnityAction<BaseItem> func_ = (item) => func?.Invoke((T)item);
			var type = itemSer.getTypeEnum<T>();
			drawFuncs.Add(type, func_);
        }

        #endregion
        
        #region 数据控制

        /// <summary>
        /// 是否为空物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool isNullItem(BaseItem item) {
            return base.isNullItem(item) || !drawFuncs.ContainsKey(item.type);
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">物品</param>
        protected override void drawExactlyItem(BaseItem item) {
            base.drawExactlyItem(item);
            drawFuncs[item.type]?.Invoke(item);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            if (icon) icon.gameObject.SetActive(false);
            if (name) name.text = "";
        }

        #endregion
    }
}