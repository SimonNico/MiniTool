using System;

namespace MiniTool.Attributes
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false)]
    public class ExceptionCatchAttribute:Attribute
    {
        /// <summary>
        /// 异常处理方法所属对象
        /// </summary>
        public Type ClassType { get; private set; }
        /// <summary>
        /// 异常处理方法名称
        /// </summary>
        public string MethodName { get; private set; }

        public ExceptionCatchAttribute(Type classType, string methodName)
        {
            this.ClassType = classType;
            this.MethodName = methodName;
        }
    }
}
