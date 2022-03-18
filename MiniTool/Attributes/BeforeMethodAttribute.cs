using System;


namespace MiniTool.Attributes
{
    /// <summary>
    /// 调用方法前调用其他方法，方法返回值类型为void或者bool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true,Inherited=false)]
    public class BeforeMethodAttribute:Attribute 
    {
        public Type t{get;private set;}

        public  string methodName { get;private set; }

        public Boolean isHasParameter { get { return this.ishasparameters; } }

        private Boolean ishasparameters=false;

        public string[] parametersname { get; private set; }

        public Boolean isInvokersParam { get { return this.isinvokersparam; } }

        private Boolean isinvokersparam=true;

        public object[] paramters{get;private set;}

        public Boolean isDebug { get { return this.isdebug; } }

        private Boolean isdebug=false;

        public Int32 OrderNo { get { return this.orderno; } }
        private Int32 orderno = -1;

     /// <summary>
     /// 执行方法前调用其他无返回方法
     /// </summary>
     /// <param name="classType">调用方法的对象</param>
     /// <param name="MethodName">方法名</param>
     /// <param name="isDebug">是否debug模式</param>
     /// <param name="isInvokersParam">是否使用调用者的参数</param>
        public BeforeMethodAttribute(Type classType, string MethodName,Boolean isDebug=false, Boolean isInvokersParam=false)
        {
            if (isInvokersParam) ishasparameters = true;
            this.isdebug = isDebug;
            this.isinvokersparam = isInvokersParam;
            this.t = classType;
            this.methodName = MethodName;
        }


       /// <summary>
       /// 执行方法前调用其他无返回方法
       /// </summary>
        /// <param name="classType">调用方法的对象</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="isDebug">是否debug模式</param>
        /// <param name="parametersName">是否使用调用者的参数</param>
        public BeforeMethodAttribute(Type classType, string MethodName,Boolean isDebug=false, Boolean isInvokersParam=false,params string[] parametersName)
        {
            this.ishasparameters = true;
            this.isinvokersparam = isInvokersParam;
            this.t = classType;
            this.methodName = MethodName;
            this.isdebug = isDebug;
            this.parametersname=parametersName;
        }
        /// <summary>
        /// 执行方法前调用其他方法
        /// </summary>
        /// <param name="classType">调用方法的对象</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="Parameters">传入自定义固定参数</param>
        public BeforeMethodAttribute(Type classType, string MethodName, Boolean isDebug = false, params object[] Parameters)
        {
            this.methodName = MethodName;
            this.t = classType;
            this.paramters = Parameters;
        }
    }
}
