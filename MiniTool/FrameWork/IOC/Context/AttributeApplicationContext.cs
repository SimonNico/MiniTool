using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniTool.FrameWork.IOC.Context.Scanner;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;

namespace MiniTool.FrameWork.IOC.Context
{
   public  class AttributeApplicationContext:GenericApplicationContext
    {
       private BeanDefinitionScanner scanner;

       [EditorBrowsable(EditorBrowsableState.Never)]
       public AttributeApplicationContext() 
        {
            ///创建扫描对象
            scanner = new BeanDefinitionScanner(this);
        }
       /// <summary>
       /// 传入程序集信息
       /// </summary>
       /// <param name="assemblies"></param>
        public AttributeApplicationContext(params Assembly[]  assemblies)
            : this()
        {
            try
            {
                ///扫描dll文件
                scanner.doScan(assemblies);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
    }
}
