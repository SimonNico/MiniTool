using System;
using System.Linq;
using System.Reflection;

namespace MiniTool
{
    /// <summary>
    /// 单例工具类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingleInstanceFactory<T> where T:class
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() =>
        {
            var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);////获取所有的构造函数
            if (constructors.Count() != 1)
                throw new InvalidOperationException(String.Format("Type {0} must have exactly one constructor.", typeof(T)));
            var ctor = constructors.SingleOrDefault(c => c.GetParameters().Count() == 0 && c.IsPrivate);  ////构造函数必须有不带参数并且私有的
            if (ctor == null)
                throw new InvalidOperationException(String.Format("The constructor for {0} must be private and take no parameters.", typeof(T)));
            return (T)ctor.Invoke(null);
        });
        public static T Current
        {
            get { return _instance.Value; }
        }
    }
}
