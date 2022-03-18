using System;

namespace MiniTool.FrameWork.IOC.Attributes
{
     [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Constructor, AllowMultiple = true)]
    public class AutoWiredAttribute : Attribute
    {
         public string BeanName { get; set; }
    }
}
