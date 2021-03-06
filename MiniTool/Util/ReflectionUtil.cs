using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;

namespace MiniTool.Util
{
    public static class ReflectionUtil
    {

        #pragma warning disable 1591
             public static BindingFlags bf = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        #pragma warning restore 1591


        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="methodName">方法名，区分大小写</param>
        /// <param name="args">方法参数</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T InvokeMethod<T>(this object obj, string methodName, object[] args)
        {
            
            var type = obj.GetType();
            var parameter = Expression.Parameter(type, "e");
            var callExpression = Expression.Call(parameter, type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray()), args.Select(Expression.Constant));
            return (T)Expression.Lambda(callExpression, parameter).Compile().DynamicInvoke(obj);
        }

        /// <summary>
        /// 执行无返回值方法
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数</param>
        public static void InvokeMethod(this object obj, string methodName, object[] args)
        {
            var type = obj.GetType();
            var parameter = Expression.Parameter(type, "e");
            var callExpression = Expression.Call(parameter, type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray()), args.Select(Expression.Constant));
            Expression.Lambda(callExpression, parameter).Compile().DynamicInvoke(obj);
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="methodName">方法名，区分大小写</param>
        /// <param name="args">方法参数</param>
        public static object InvokeMethod(this Type type,string methodName,object[] args=null){
            var parameter=Expression.Parameter(type,"e");
            MethodCallExpression callExpression=null;
            if(args==null)
             callExpression=Expression.Call(parameter,type.GetMethod(methodName));
            else
           callExpression = Expression.Call(parameter, type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray()), args.Select(Expression.Constant));
          return  Expression.Lambda(callExpression,parameter).Compile().DynamicInvoke(args);
        }


        #region 属性字段设置

        /// <summary>
        /// 设置字段
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">字段名</param>
        /// <param name="value">值</param>
        public static void SetField<T>(this T obj, string name, object value) where T : class
        {
            SetProperty(obj, name, value);
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">字段名</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T GetField<T>(this object obj, string name)
        {
            return GetProperty<T>(obj, name);
        }


        /// <summary>
        /// 获取所有的字段信息
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <returns>字段信息</returns>
        public static FieldInfo[] GetFields(this object obj)
        {
            FieldInfo[] fieldInfos = obj.GetType().GetFields(bf);
            return fieldInfos;
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">属性名</param>
        /// <param name="value">值</param>
        public static string SetProperty<T>(this T obj, string name, object value) where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.PropertyOrField(parameter, name);
            var before = Expression.Lambda(property, parameter).Compile().DynamicInvoke(obj);
            if (value.Equals(before))
            {
                return value.ToString();
            }

            if (property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeof(T).GetProperty(name).SetValue(obj, value);
            }
            else
            {
                var assign = Expression.Assign(property, Expression.Constant(value));
                Expression.Lambda(assign, parameter).Compile().DynamicInvoke(obj);
            }

            return before.ToString();
        }
        /// 获取属性
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <param name="name">属性名</param>
        /// <typeparam name="T">约束返回的T必须是引用类型</typeparam>
        /// <returns>T类型</returns>
        public static T GetProperty<T>(this object obj, string name)
        {
            var parameter = Expression.Parameter(obj.GetType(), "e");
            var property = Expression.PropertyOrField(parameter, name);
            return (T)Expression.Lambda(property, parameter).Compile().DynamicInvoke(obj);
        }

        /// <summary>
        /// 获取所有的属性信息
        /// </summary>
        /// <param name="obj">反射对象</param>
        /// <returns>属性信息</returns>
        public static PropertyInfo[] GetProperties(this object obj)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties(bf);
            return propertyInfos;
        }
        #endregion 属性字段设置

        /// <summary>
        /// 计算方法中的输出参数个数
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns>输出参数个数</returns>
        public static Int32 GetMethodOutParameterCount(this MethodInfo method)
        {
            Int32 count = 0;
            if (method == null) return count;
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters == null && parameters.Length == 0) return count;
            count = parameters.Where(p => p.IsOut == true).Count();
            return count;
        }
        /// <summary>
        /// 计算方法中的输出参数个数
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns>输出参数个数</returns>
        public static Int32 GetMethodOutParameterCount(this MethodBase method)
        {
            Int32 count = 0;
            if (method == null) return count;
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters == null && parameters.Length == 0) return count;
            count = parameters.Where(p => p.IsOut == true).Count();
            return count;
        }



        #region 创建实例

        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object GetInstance(this Type type)
        {
            return GetInstance<TypeToIgnore, object>(type, null);
        }

        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static T GetInstance<T>(this Type type) where T : class, new()
        {
            return GetInstance<TypeToIgnore, T>(type, null);
        }

        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static T GetInstance<T>(string type) where T : class, new()
        {
            return GetInstance<TypeToIgnore, T>(Type.GetType(type), null);
        }

        /// <summary>
        /// 获取默认实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object GetInstance(string type)
        {
            return GetInstance<TypeToIgnore, object>(Type.GetType(type), null);
        }

        /// <summary>
        /// 获取一个构造参数的实例
        /// </summary>
        /// <typeparam name="TArg">参数类型</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="argument">参数值</param>
        /// <returns></returns>
        public static T GetInstance<TArg, T>(this Type type, TArg argument) where T : class, new()
        {
            return GetInstance<TArg, TypeToIgnore, T>(type, argument, null);
        }

        /// <summary>
        /// 获取一个构造参数的实例
        /// </summary>
        /// <typeparam name="TArg">参数类型</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="argument">参数值</param>
        /// <returns></returns>
        public static T GetInstance<TArg, T>(string type, TArg argument) where T : class, new()
        {
            return GetInstance<TArg, TypeToIgnore, T>(Type.GetType(type), argument, null);
        }

        /// <summary>
        /// 获取2个构造参数的实例
        /// </summary>
        /// <typeparam name="TArg1">参数类型</typeparam>
        /// <typeparam name="TArg2">参数类型</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="argument1">参数值</param>
        /// <param name="argument2">参数值</param>
        /// <returns></returns>
        public static T GetInstance<TArg1, TArg2, T>(this Type type, TArg1 argument1, TArg2 argument2) where T : class, new()
        {
            return GetInstance<TArg1, TArg2, TypeToIgnore, T>(type, argument1, argument2, null);
        }

        /// <summary>
        /// 获取2个构造参数的实例
        /// </summary>
        /// <typeparam name="TArg1">参数类型</typeparam>
        /// <typeparam name="TArg2">参数类型</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="argument1">参数值</param>
        /// <param name="argument2">参数值</param>
        /// <returns></returns>
        public static T GetInstance<TArg1, TArg2, T>(string type, TArg1 argument1, TArg2 argument2) where T : class, new()
        {
            return GetInstance<TArg1, TArg2, TypeToIgnore, T>(Type.GetType(type), argument1, argument2, null);
        }

        /// <summary>
        /// 获取3个构造参数的实例
        /// </summary>
        /// <typeparam name="TArg1">参数类型</typeparam>
        /// <typeparam name="TArg2">参数类型</typeparam>
        /// <typeparam name="TArg3">参数类型</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="argument1">参数值</param>
        /// <param name="argument2">参数值</param>
        /// <param name="argument3">参数值</param>
        /// <returns></returns>
        public static T GetInstance<TArg1, TArg2, TArg3, T>(this Type type, TArg1 argument1, TArg2 argument2, TArg3 argument3) where T : class, new()
        {
            return InstanceCreationFactory<TArg1, TArg2, TArg3, T>.CreateInstanceOf(type, argument1, argument2, argument3);
        }

        /// <summary>
        /// 获取3个构造参数的实例
        /// </summary>
        /// <typeparam name="TArg1">参数类型</typeparam>
        /// <typeparam name="TArg2">参数类型</typeparam>
        /// <typeparam name="TArg3">参数类型</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="argument1">参数值</param>
        /// <param name="argument2">参数值</param>
        /// <param name="argument3">参数值</param>
        /// <returns></returns>
        public static T GetInstance<TArg1, TArg2, TArg3, T>(string type, TArg1 argument1, TArg2 argument2, TArg3 argument3) where T : class, new()
        {
            return InstanceCreationFactory<TArg1, TArg2, TArg3, T>.CreateInstanceOf(Type.GetType(type), argument1, argument2, argument3);
        }

        private class TypeToIgnore
        {
        }

        private static class InstanceCreationFactory<TArg1, TArg2, TArg3, TObject> where TObject : class, new()
        {
            private static readonly Dictionary<Type, Func<TArg1, TArg2, TArg3, TObject>> InstanceCreationMethods = new Dictionary<Type, Func<TArg1, TArg2, TArg3, TObject>>();

            public static TObject CreateInstanceOf(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            {
                CacheInstanceCreationMethodIfRequired(type);

                return InstanceCreationMethods[type](arg1, arg2, arg3);
            }

            private static void CacheInstanceCreationMethodIfRequired(Type type)
            {
                if (InstanceCreationMethods.ContainsKey(type))
                {
                    return;
                }

                var argumentTypes = new[]
                {
                    typeof(TArg1),
                    typeof(TArg2),
                    typeof(TArg3)
                };

                Type[] constructorArgumentTypes = argumentTypes.Where(t => t != typeof(TypeToIgnore)).ToArray();
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, constructorArgumentTypes, new ParameterModifier[0]);

                var lamdaParameterExpressions = new[]
                {
                    Expression.Parameter(typeof(TArg1), "param1"),
                    Expression.Parameter(typeof(TArg2), "param2"),
                    Expression.Parameter(typeof(TArg3), "param3")
                };

                var constructorParameterExpressions = lamdaParameterExpressions.Take(constructorArgumentTypes.Length).ToArray();
                var constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);
                var constructorCallingLambda = Expression.Lambda<Func<TArg1, TArg2, TArg3, TObject>>(constructorCallExpression, lamdaParameterExpressions).Compile();
                InstanceCreationMethods[type] = constructorCallingLambda;
            }
        }

        #endregion 创建实例
    
    }
}
