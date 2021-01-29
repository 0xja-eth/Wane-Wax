using System;
using System.Collections.Generic;
using System.Reflection;

namespace Core.Utils {

	/// <summary>
	/// 枚举类型
	/// </summary>
	public abstract class EnumExtend { }

	/// <summary>
	/// 枚举/类型处理类
	/// </summary>
	public abstract class EnumTypeProcessor {

		/// <summary>
		/// 绑定的类型
		/// </summary>
		protected Type bindingType { get; set; }

		/// <summary>
		/// 配置类型
		/// </summary>
		/// <param name="type"></param>
		protected void setupType(Type type, bool bind = false) {
			if (bind) bindingType = type;

			if (type == null) return;
			if (type.IsEnum) setupEnumType(type);
			else if(type.IsSubclassOf(typeof(EnumExtend)))
				setupEnumExtendType(type);
		}

		/// <summary>
		/// 配置普通类型
		/// </summary>
		/// <param name="type"></param>
		void setupEnumExtendType(Type type) {
			ReflectionUtils.processMember<FieldInfo>(type,
				typeof(string), f => {
					var val = f.GetValue(null) as string;
					if (val == null) f.SetValue(null, val = f.Name);

					processValue(val);
				}, flags: ReflectionUtils.DefaultStaticFlags);
		}

		/// <summary>
		/// 配置普通类型
		/// </summary>
		/// <param name="type"></param>
		void setupEnumType(Type type) {
			foreach (var name in Enum.GetNames(type))
				processValue(name);
		}

		/// <summary>
		/// 处理值
		/// </summary>
		/// <param name="name"></param>
		protected abstract void processValue(string val);

		/// <summary>
		/// 构造函数
		/// </summary>
		public EnumTypeProcessor() { }
		public EnumTypeProcessor(Type type) {
			setupType(type, true);
		}

	}
}
