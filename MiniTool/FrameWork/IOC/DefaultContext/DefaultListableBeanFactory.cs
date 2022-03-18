using System;
using MiniTool.FrameWork.IOC.Registry;
using System.Collections.Concurrent;
using MiniTool.FrameWork.IOC.Bean;
using System.Reflection;
using System.ComponentModel;
using MiniTool.Util;
using System.Linq.Expressions;
using MiniTool.Attributes;

namespace MiniTool.FrameWork.IOC.DefaultContext
{
    public  class DefaultListableBeanFactory : IRegistryBeanDefinition
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ConcurrentDictionary<string, BeanDefinition> beanDefinitionDict = new ConcurrentDictionary<string, BeanDefinition>();

        /// <summary>
        /// 通过发射来动态创建指定别名类的实例
        /// </summary>
        /// <param name="beanName"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object GetBean(string beanName)
        {
            BeanDefinition value = null;
            object obj = null;
            if (!beanDefinitionDict.TryGetValue(beanName, out  value))
            {
                throw new Exception("The object is not registered with MiniTool containner");
            }
            if (value.ScopeName == ScopType.Singleton)////从缓存中获取对象
            {
                obj = value.BeanClass.GetInstance();
            }
            else
            {
                ///通过表达式获取无参数构造函数对象
                var newExpression = Expression.New(value.BeanClass);
                var lambda = Expression.Lambda<Func<object>>(newExpression);
                obj = lambda.Compile()();
            }
           
            ///执行依赖注入
            PouplateBean(obj);
            return obj;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public T GetBean<T>() 
        {
            BeanDefinition value = null;
            object obj = null;
            string name = BeanDefinitionUtils.getBeanName<T>();
            if (!beanDefinitionDict.TryGetValue(name, out value))
            {
                throw new Exception("The object is not registered with MiniTool containner");
            }
            if (value.ScopeName == ScopType.Singleton)
            {
                obj = (T)value.BeanClass.GetInstance();
            }
            else
            {
                var newExpression = Expression.New(value.BeanClass);
                var lambda = Expression.Lambda<Func<T>>(newExpression);
                obj = lambda.Compile()();
            }
           
            ///执行依赖注入(反射)
            PouplateBean(obj);
            return (T)obj;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>

        private void PouplateBean(object obj)
        {
            Type type = obj.GetType();
            FieldInfo[] propertyInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo item in propertyInfos)
            {

                string injectName = item.FieldType.Name.ToBeanName();
                AutoWiredAttribute attr = item.GetCustomAttribute<AutoWiredAttribute>();
                string autowiredName = attr.beanName;
                if (null != attr && null != autowiredName)
                {
                    injectName = autowiredName;
                }
                object injectInstance = GetBean(injectName);
                item.SetValue(obj, injectInstance);
            }

        }


        /// <summary>
        /// 真正在IOC容器中进行管理BeanDefinition对象的注册
        /// </summary>
        /// <param name="beanName"></param>
        /// <param name="beanDefinition"></param>
      [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegitryBeanDefinition(string beanName, BeanDefinition beanDefinition)
        {
            if (beanDefinitionDict.ContainsKey(beanName))
            {
                throw new Exception(string.Format("{0} is registered repeatedly", beanDefinition.BeanClass.Name));
            }
            beanDefinitionDict.TryAdd(beanName, beanDefinition);
        }
    }
}
