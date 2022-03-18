using System;

namespace MiniTool.Attributes
{
     [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Constructor, AllowMultiple = false)]
    public class AutoWiredAttribute : Attribute
    {
         public string beanName { get; private set; }
         public AutoWiredAttribute(string BeanName=null)
         {
             this.beanName = BeanName;
         }
    }
}
