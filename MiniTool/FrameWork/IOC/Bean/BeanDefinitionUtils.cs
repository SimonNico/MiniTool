using MiniTool.FrameWork.IOC.Registry;
using System.ComponentModel;

namespace MiniTool.FrameWork.IOC.Bean
{
   internal static class BeanDefinitionUtils
    {
       /// <summary>
       /// bean 注册
       /// </summary>
       /// <param name="beanDefinition"></param>
       /// <param name="registry"></param>
       [EditorBrowsable(EditorBrowsableState.Never)]
       public static void BeanRegistry(BeanDefinition beanDefinition, IRegistryBeanDefinition registry)
       {
           registry.RegitryBeanDefinition(beanDefinition.BeanName, beanDefinition);
       }
       /// <summary>
       /// 获取bean
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
       [EditorBrowsable(EditorBrowsableState.Never)]
       public static string getBeanName<T>()
       {
           string className = typeof(T).Name;
           string beanName = className.Substring(0, 1).ToLower() + className.Substring(1);
           return beanName;
       }
       /// <summary>
       /// 转换小写 用于当做key存放字典中
       /// </summary>
       /// <param name="name"></param>
       /// <returns></returns>
       [EditorBrowsable(EditorBrowsableState.Never)]
       public static string ToBeanName(this string name)
       {
           string beanName = name.Substring(0, 1).ToLower() + name.Substring(1);
           return beanName;
       }

    }
}
