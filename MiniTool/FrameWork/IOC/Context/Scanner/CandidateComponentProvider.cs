using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTool.FrameWork.IOC.Bean;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using MiniTool.Attributes;

namespace MiniTool.FrameWork.IOC.Context.Scanner
{
    public class CandidateComponentProvider
    {
        private List<string> _includeFilters = new List<string>();

        public List<string> IncludeFilters { get { return this._includeFilters; } set { this._includeFilters = value; } }

        protected void RegisterDefaultFilters()
        {
            IncludeFilters.Add(typeof(ComponentAttribute).Name);
        }
       
        internal List<BeanDefinition> ScanCandidateComponents(Assembly assembly)
        {
            List<BeanDefinition> list = new List<BeanDefinition>();
       
            //需要获取所有标记为ComponentAttribute特性的类
            var types = assembly.GetTypes().Where(p => p.IsDefined(typeof(ComponentAttribute), true));
          
            foreach (var type in types)
            {
                if (!type.IsInterface && !type.IsAbstract)
                {
                    ///封装成BeanDefinition
                    BeanDefinition beanDefinition = new BeanDefinition();
                    beanDefinition.BeanClass = type;
                    list.Add(beanDefinition);

                }
            }
            return list.Distinct().ToList();////去重
        }
    }
}
