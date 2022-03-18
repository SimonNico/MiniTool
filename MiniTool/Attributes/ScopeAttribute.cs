using System;
using MiniTool.FrameWork.IOC;

namespace MiniTool.Attributes
{
     [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScopeAttribute:Attribute
    {
         public ScopType Scotype { get; private set; }

         public ScopeAttribute(ScopType type = ScopType.Singleton)
         {
             this.Scotype = type;
         }
         
    }
}
