using System;

namespace MiniTool.FrameWork.IOC.Attributes
{
     [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ScopeAttribute:Attribute
    {
         public string Name { get; set; }
    }
}
