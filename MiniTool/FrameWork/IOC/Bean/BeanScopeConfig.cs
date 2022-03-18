using MiniTool.Attributes;
using System.ComponentModel;
using System.Reflection;

namespace MiniTool.FrameWork.IOC.Bean
{
    public class BeanScopeConfig
    {
        /// <summary>
        /// 设置BeanDefinition的作用域
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal ScopType ConfigScope(BeanDefinition definition)
        {
            var attr = definition.BeanClass.GetCustomAttribute<ScopeAttribute>();
            ScopType scoptype = ScopType.Singleton;
            if (null != attr)
            {
                scoptype = attr.Scotype;
            }
            return scoptype;
        }
    }
}
