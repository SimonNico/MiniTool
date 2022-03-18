using System;

namespace MiniTool.FrameWork.IOC.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
   public class ComponentAttribute:Attribute
    {
        public string BeanName { get; set; }
    }
}
