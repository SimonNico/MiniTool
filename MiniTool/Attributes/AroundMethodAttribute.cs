using System;

namespace MiniTool.Attributes
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false)]
    public sealed class AroundMethodAttribute:Attribute
    {
        private Type t;

        private string methodName;

        private Boolean ishasparameters=false;

        private string[] parametersname;

        private Boolean isinvokersparam=true;

        private object[] paramters;

        private Boolean isdebug=false;

     /// <summary>
     /// 执行方法前调用其他无返回方法
     /// </summary>
     /// <param name="classType">调用方法的对象</param>
     /// <param name="MethodName">方法名</param>
     /// <param name="isDebug">是否debug模式</param>
     /// <param name="isInvokersParam">是否使用调用者的参数</param>
        public AroundMethodAttribute(Type classType, string MethodName,Boolean isDebug=false, Boolean isInvokersParam=false)
        {
            if (isInvokersParam) ishasparameters = true;
            this.isdebug = isDebug;
            this.isinvokersparam = isInvokersParam;
            this.t = classType;
            this.methodName = MethodName;
        }


       /// <summary>
       /// 执行方法其他无返回方法
       /// </summary>
        /// <param name="classType">调用方法的对象</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="isDebug">是否debug模式</param>
        /// <param name="parametersName">是否使用调用者的参数</param>
        public AroundMethodAttribute(Type classType, string MethodName,Boolean isDebug=false, Boolean isInvokersParam=false,params string[] parametersName)
        {
            this.ishasparameters = true;
            this.isinvokersparam = isInvokersParam;
            this.t = classType;
            this.methodName = MethodName;
            this.isdebug = isDebug;
            this.parametersname=parametersName;
        }
        /// <summary>
        /// 执行方法调用其他方法
        /// </summary>
        /// <param name="classType">调用方法的对象</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parameters">传入自定义固定参数</param>
        public AroundMethodAttribute(Type classType, string MethodName, Boolean isDebug = false, params object[] Parameters)
        {
            this.methodName = MethodName;
            this.t = classType;
            this.paramters = Parameters;
        }
    }
}
