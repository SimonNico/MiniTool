using System;
using MiniTool.FrameWork.AOP;
using System.Collections.Generic;

namespace MiniTool.Attributes
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false)]
    public sealed class MathodAopAdviceAttribute:Attribute
    {
        private AdviceType advicetype;

        private Dictionary<string, Action> voidmethods;

        public MathodAopAdviceAttribute(AdviceType adviceType,Dictionary<string, Action> VoidMethods=null)
        {
            this.advicetype = adviceType;
            this.voidmethods = VoidMethods;
        }
        public AdviceType AdviceType
        {
            get
            {
                return this.advicetype;
            }
        }
    }
}
