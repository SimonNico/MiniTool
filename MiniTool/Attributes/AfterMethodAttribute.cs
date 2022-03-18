using System;
using MiniTool.FrameWork.AOP;

namespace MiniTool.Attributes
{
    /// <summary>
    /// 调用方法后调用其他方法，方法返回值类型为void或者bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=true,Inherited=false)]
    public class AfterMethodAttribute:Attribute
    {
        public Type t { get; private set; }

        public string methodName { get; private set; }

        public Boolean isHasParameter { get { return this.ishasparameters; } }

        private Boolean ishasparameters = false;

        public string[] parametersname { get; private set; }

        public Boolean isInvokersResult { get { return this.isinvokerresult; } }

        private Boolean isinvokerresult = true;

        public object[] paramters { get; private set; }

        public Boolean isDebug { get { return this.isdebug; } }

        private Boolean isdebug = false;

        public Int32 OrderNo { get { return this.orderno; } }
        private Int32 orderno = -1;

        private AspectMethodReturnType returntype = AspectMethodReturnType.Asvoid;

 /// <summary>
 /// 调用方法前切入的方法
 /// </summary>
 /// <param name="classType">切入方法所属对象类型</param>
 /// <param name="MethodName">切入方法的名称</param>
 /// <param name="returnType">切入方法返回值类型 Asvoid 无返回值 Asbool 返回值为bool类型</param>
 /// <param name="isDebug">是否调试模式</param>
 /// <param name="isInvokersResult">是否使用调用者的参数，两者参数类型要一致，否则异常</param>
        public AfterMethodAttribute(Type classType, string MethodName, AspectMethodReturnType returnType=AspectMethodReturnType.Asvoid,Int32 orderNo=-1, Boolean isDebug = false, Boolean isInvokersResult = false)
        {
            if (isInvokersResult) ishasparameters = true;
            this.returntype = returnType;
            this.isdebug = isDebug;
            this.orderno = orderNo;
            this.isinvokerresult = isInvokersResult;
            this.t = classType;
            this.methodName = MethodName;
        }

       /// <summary>
        /// 调用方法前切入的方法
       /// </summary>
        /// <param name="classType">切入方法所属对象类型</param>
        /// <param name="MethodName">切入方法的名称</param>
        /// <param name="returnType">切入方法返回值类型 Asvoid 无返回值 Asbool 返回值为bool类型</param>
        /// <param name="isDebug">是否debug模式</param>
        /// <param name="parametersName">是否使用调用者的参数，两者参数类型要一致，否则异常</param>
        public AfterMethodAttribute(Type classType, string MethodName, AspectMethodReturnType returnType = AspectMethodReturnType.Asvoid,Int32 orderNo=-1, Boolean isDebug = false, Boolean isInvokersResult = false, params string[] parametersName)
        {
            this.ishasparameters = true;
            this.isinvokerresult = isInvokersResult;
            this.t = classType;
            this.orderno = orderNo;
            this.methodName = MethodName;
            this.isdebug = isDebug;
            this.parametersname=parametersName;
        }

        // <summary>
        /// 调用方法前切入的方法
        /// </summary>
        /// <param name="classType">切入方法所属对象类型</param>
        /// <param name="MethodName">切入方法的名称</param>
        /// <param name="returnType">切入方法返回值类型 Asvoid 无返回值 Asbool 返回值为bool类型</param>
        /// <param name="isDebug">是否debug模式</param>
        /// <param name="Parameters">传入自定义固定参数</param>
        public AfterMethodAttribute(Type classType, string MethodName,AspectMethodReturnType returnType=AspectMethodReturnType.Asvoid,Int32 orderNo=-1,  Boolean isDebug = false, params object[] Parameters)
        {
            this.methodName = MethodName;
            this.ishasparameters = true;
            this.orderno = orderNo;
            this.t = classType;
            this.paramters = Parameters;
        }
    }
}
