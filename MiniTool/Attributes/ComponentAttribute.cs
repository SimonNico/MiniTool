using System;

namespace MiniTool.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
   public sealed class ComponentAttribute:Attribute
    {

        public string Alias { get; private set; }

        public ComponentAttribute(string alias=null)
        {
            this.Alias = alias;
        }
    }
}
