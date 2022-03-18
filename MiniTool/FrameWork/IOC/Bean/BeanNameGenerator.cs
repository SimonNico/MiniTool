using System;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using MiniTool.Attributes;
namespace MiniTool.FrameWork.IOC.Bean
{
    internal class BeanNameGenerator
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GeneratorBeanName(BeanDefinition definition)
        {
            var attr = definition.BeanClass.GetCustomAttribute<ComponentAttribute>(false);
            string beanName = attr.Alias;
            if (!string.IsNullOrEmpty(beanName))
            {
                return beanName;
            }
            ///按照默认方式生成别名
            return BuildDefaultName(definition);
        }

        /// <summary>
        /// 默认是类名首字母是小写 生成别名
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        private string BuildDefaultName(BeanDefinition definition)
        {
            Type type = definition.BeanClass.GetInterfaces().Where(p => p.IsDefined(typeof(InjectInterfaceAttribute), true)).FirstOrDefault();
            
            string className = type==null?definition.BeanClass.Name:type.Name;


            string beanName = className.Substring(0, 1).ToLower() + className.Substring(1);
            return beanName;
        }
    }
}
