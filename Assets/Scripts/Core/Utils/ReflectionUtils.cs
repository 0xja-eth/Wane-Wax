using System;
using System.Linq;
using System.Reflection;

namespace Core.Utils {

	/// <summary>
	/// 反射工具类 V2.3
	/// </summary>
	public static class ReflectionUtils {

		/// <summary>
		/// 默认绑定标志
		/// </summary>
		public const BindingFlags DefaultFlags =
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		public const BindingFlags DefaultStaticFlags =
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Static;

		#region 获取实例

		/// <summary>
		/// 获取字段
		/// </summary>
		public static T getField<T>(object obj, string name, BindingFlags flags = DefaultFlags) {
			if (obj == null) return default;
			var info = obj.GetType().GetField(name, flags);
			return (T)info?.GetValue(obj);
		}

		/// <summary>
		/// 获取属性
		/// </summary>
		public static T getProperty<T>(object obj, string name, BindingFlags flags = DefaultFlags) {
			if (obj == null) return default;
			var info = obj.GetType().GetProperty(name, flags);
			return (T)info?.GetValue(obj);
		}

		/// <summary>
		/// 快速处理成员
		/// </summary>
		/// <typeparam name="M">MemberInfo类型</typeparam>
		/// <param name="type">所属类</param>
		/// <param name="processFunc">处理函数</param>
		public static void processMember<M>(Type type, Action<M> processFunc, 
			int processCnt = 0, BindingFlags flags = DefaultFlags) where M : MemberInfo {

			var memberInfos = getMemberInfos<M>(type, flags);
			if (memberInfos == null) return;

			var cnt = 0;
			foreach (var m in memberInfos) {
				if (processCnt > 0 && cnt++ >= processCnt) return;

				var member = m as M;
				if (member == null) continue;

				processFunc(member);
			}
		}
		/// <typeparam name="T">成员类型（父类）</typeparam>
		public static void processMember<M, T>(Type type, Action<M> processFunc, 
			int processCnt = 0, BindingFlags flags = DefaultFlags) where M : MemberInfo {

			var tType = typeof(T); var cnt = 0;
			processMember<M>(type, m => {
				if (processCnt > 0 && cnt++ >= processCnt) return;

				var mType = getMemberType<M>(m);
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m);
			}, flags: flags);
		}
		/// <param name="tType">成员类型（父类）</param>
		public static void processMember<M>(Type type, Type tType, Action<M> processFunc, 
			int processCnt = 0, BindingFlags flags = DefaultFlags) where M : MemberInfo {

			var cnt = 0;
			processMember<M>(type, m => {
				if (processCnt > 0 && cnt++ >= processCnt) return;

				var mType = getMemberType<M>(m);
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m);
			}, flags: flags);
		}

		#endregion

		#region 快速处理

		/// <summary>
		/// 快速处理特性
		/// </summary>
		/// <typeparam name="M">MemberInfo类型</typeparam>
		/// <typeparam name="A">特性类型</typeparam>
		/// <param name="type">所属类</param>
		/// <param name="processFunc">处理函数</param>
		public static void processAttribute<M, A>(Type type, Action<M, A> processFunc, 
			BindingFlags flags = DefaultFlags) where M : MemberInfo where A : Attribute {

			processMember<M>(type, m => {
				foreach (Attribute a in m.GetCustomAttributes(false)) {
					var attr = a as A;
					if (attr == null) continue;

					processFunc(m, attr);
				}
			}, flags: flags);
		}

		/// <summary>
		/// 处理类特性
		/// </summary>
		/// <typeparam name="A">特性类型</typeparam>
		/// <param name="type">类型</param>
		/// <param name="processFunc">处理函数</param>
		public static void processClassAttribute<A>(
			Type type, Action<A> processFunc) where A : Attribute {
			var member = type as MemberInfo;
			if (member == null) return;

			foreach (Attribute a in member.GetCustomAttributes(true)) {
				var attr = a as A;
				if (attr == null) continue;

				processFunc(attr);
			}
		}

		/// <summary>
		/// 处理字段
		/// </summary>
		/// <typeparam name="T">字段类型</typeparam>
		/// <param name="self">实例</param>
		/// <param name="processFunc">处理函数</param>
		public static void processField<T>(object self, Action<T> processFunc, 
			BindingFlags flags = DefaultFlags) {
			var tType = typeof(T);
			var sType = self.GetType();
			processMember<FieldInfo>(sType, m => {
				var mType = m.FieldType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc((T)m.GetValue(self));
			}, flags: flags);
		}
		public static void processField(object self, Type tType, 
			Action<object> processFunc, BindingFlags flags = DefaultFlags) {
			var sType = self.GetType();

			processMember<FieldInfo>(sType, m => {
				var mType = m.FieldType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m.GetValue(self));
			}, flags: flags);
		}

		/// <summary>
		/// 处理属性
		/// </summary>
		/// <typeparam name="T">属性类型</typeparam>
		/// <param name="self">实例</param>
		/// <param name="processFunc">处理函数</param>
		public static void processProperty<T>(object self, Action<T> processFunc, 
			BindingFlags flags = DefaultFlags) {
			var tType = typeof(T);
			var sType = self.GetType();
			processMember<PropertyInfo>(sType, m => {
				var mType = m.PropertyType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc((T)m.GetValue(self));
			}, flags: flags);
		}
		public static void processProperty(object self, Type tType, 
			Action<object> processFunc, BindingFlags flags = DefaultFlags) {
			var sType = self.GetType();

			processMember<PropertyInfo>(sType, m => {
				var mType = m.PropertyType;
				if (mType == tType || mType.IsSubclassOf(tType))
					processFunc(m.GetValue(self));
			}, flags: flags);
		}

		#endregion

		#region 获取Info

		/// <summary>
		/// 获取成员信息数组
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static MemberInfo[] getMemberInfos(Type type, 
			MemberTypes memberType, BindingFlags flags = DefaultFlags) {

			switch (memberType) {
				case MemberTypes.All:
					return type.GetMembers(flags);
				case MemberTypes.Field:
					return type.GetFields(flags);
				case MemberTypes.Property:
					return type.GetProperties(flags);
				case MemberTypes.Event:
					return type.GetEvents(flags);
				case MemberTypes.Method:
					return type.GetMethods(flags);
				default: return null;
			}
		}
		public static MemberInfo[] getMemberInfos<M>(Type type, 
			BindingFlags flags = DefaultFlags) where M : MemberInfo {
			var memberType = typeof(M);

			if (memberType == typeof(MemberInfo))
				return getMemberInfos(type, MemberTypes.All, flags);
			if (memberType == typeof(FieldInfo))
				return getMemberInfos(type, MemberTypes.Field, flags);
			if (memberType == typeof(PropertyInfo))
				return getMemberInfos(type, MemberTypes.Property, flags);
			if (memberType == typeof(EventInfo))
				return getMemberInfos(type, MemberTypes.Event, flags);
			if (memberType == typeof(MethodInfo))
				return getMemberInfos(type, MemberTypes.Method, flags);

			return null;
		}

		/// <summary>
		/// 获取成员类型（字段类型/属性类型）
		/// </summary>
		/// <typeparam name="M"></typeparam>
		/// <param name="info"></param>
		/// <returns></returns>
		public static Type getMemberType<M>(MemberInfo info) where M : MemberInfo {
			var memberType = typeof(M);

			if (memberType == typeof(FieldInfo))
				return (info as FieldInfo).FieldType;
			if (memberType == typeof(PropertyInfo))
				return (info as PropertyInfo).PropertyType;
			if (memberType == typeof(EventInfo))
				return (info as EventInfo).EventHandlerType;

			return null;
		}

		#endregion

		#region 命名空间相关计算

		/// <summary>
		/// 获取命名空间下的所有类型
		/// </summary>
		/// <param name="namespace_">命名空间全名称</param>
		/// <returns></returns>
		public static Type[] getNamespaceTypes(
			string namespace_ = null, Type parent = null, Type attrType = null) {
			return getNamespaceTypes(Assembly.GetExecutingAssembly(), 
				namespace_, parent, attrType);
		}
		/// <param name="assembly">程序集</param>
		public static Type[] getNamespaceTypes(Assembly assembly, 
			string namespace_ = null, Type parent = null, Type attrType = null) {
			return assembly.GetTypes().Where(
				t => isTypeSatisfied(t, namespace_, parent, attrType)).ToArray();
		}

		/// <summary>
		/// 类型是否满足
		/// </summary>
		static bool isTypeSatisfied(Type type, 
			string namespace_, Type parent, Type attrType) {
			bool res = (type != null && !type.IsAbstract);

			if (parent != null)
				if (parent.IsInterface) // 接口
					res = res && type.GetInterfaces().Contains(parent);
				else if (parent.IsClass) // 类
					res = res && type.IsSubclassOf(parent);

			if (attrType != null)
				res = res && type.GetCustomAttribute(attrType) != null;

			if (namespace_ != null)
				res = res && type.Namespace == namespace_;

			return res;
		}

		#endregion

		#region 类继承关系相关计算

		/// <summary>
		/// 寻找拥有泛型的父类
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type findGenericParentType(Type type) {
			if (type == null) return null;
			if (type.IsGenericType) return type;
			return findGenericParentType(type.BaseType);
		}

		#endregion
	}
}
