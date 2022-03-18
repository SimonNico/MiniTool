using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Linq;
using MiniTool.Util;
using MiniTool.Attributes;
using System.Runtime.Remoting.Activation;
using System.Linq.Expressions;

namespace MiniTool.FrameWork.AOP
{
    public class AopProxy : RealProxy,IAdivce
    {
        List<InvokeMethodInfo> preList = new List<InvokeMethodInfo>();
        List<InvokeMethodInfo> postList = new List<InvokeMethodInfo>();
        List<InvokeMethodInfo> aroundList = new List<InvokeMethodInfo>();
        Type _type = null;
        bool isdebug = false;
        public AopProxy(Type serverType)
            : base(serverType)
        {
            this._type = serverType;
            isDebug(serverType);
        }

        /// <summary>
        /// 判断调用者是否debug模式调用
        /// </summary>
        /// <param name="type"></param>
        private void isDebug(Type type)
        {
            Assembly assembly = type.Assembly;
            IEnumerable<object> objs = assembly.GetCustomAttributes(false).Where(p => p.GetType() == typeof(System.Diagnostics.DebuggableAttribute));
            foreach (var item in objs)
            {
                if (((System.Diagnostics.DebuggableAttribute)item).IsJITTrackingEnabled)
                {
                    this.isdebug = false;
                    break;
                }
            }
        }

        /// <summary>
        /// 重写调用方法
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage call = (IMethodCallMessage)msg;
            IEnumerable<BeforeMethodAttribute> preAttr = call.MethodBase.GetCustomAttributes<BeforeMethodAttribute>();
            IEnumerable<AfterMethodAttribute> postAttr = call.MethodBase.GetCustomAttributes<AfterMethodAttribute>();
            IEnumerable<AroundMethodAttribute> adAttr = call.MethodBase.GetCustomAttributes<AroundMethodAttribute>();
            ExceptionCatchAttribute exch = call.MethodBase.GetCustomAttribute<ExceptionCatchAttribute>();

            TransactionalAttribute tranAttr = call.MethodBase.GetCustomAttribute<TransactionalAttribute>();
            IMessage returnMsg = null; 
            try
            {
                if (msg is IConstructionCallMessage)
                {
                    returnMsg = CustructureProcess(msg);
                }
                else
                {
                    if (preAttr != null && preAttr.Count() > 0)
                    {
                        returnMsg = preProcess(preAttr, msg);
                    }
                    returnMsg = invokeProcess(call);

                }
            }
            catch (Exception ex)
            {
                if (exch != null)
                {
                    object tobj = new object();
                    ExceptionProcess(exch, ex);
                    returnMsg = new ReturnMessage(ex,call);
                }
                else
                {
                    object tobj = new object();
                    returnMsg = new ReturnMessage(ex,call);
                   // returnMsg = new ReturnMessage(tobj, call);
                }
               
            }
            return returnMsg;
        }


        /// <summary>
        /// 调用前切入的方法
        /// </summary>
        /// <param name="beforeAttrs">beforeAttribute集合</param>
        /// <param name="msg">调用者信息</param>
        /// <param name="param">传入的自定义参数，调用者的参数通过msg获取</param>
        /// <returns></returns>
        private IMessage preProcess( IEnumerable<BeforeMethodAttribute> beforeAttrs, IMessage msg)
        {
            List<InvokeMethodInfo> methods = null;
            foreach (var att in beforeAttrs)
            {
                InvokeMethodInfo mfo = new InvokeMethodInfo();
                mfo.ClassType = att.t;
                mfo.MethodName = att.methodName;
                mfo.isDebug = att.isDebug;
                mfo.isHasParameter = att.isHasParameter;
                mfo.isInvokeParameter = att.isInvokersParam;
                mfo.orderNo = att.OrderNo;
                mfo.parameterName = att.parametersname;
                mfo.parameters = att.paramters;
                methods.Add(mfo);
            }
            IMessage resultMsg = null;
            IMethodCallMessage callMsg = (IMethodCallMessage)msg;
            methods = methods.OrderByDescending(p => p.orderNo).ToList();
            object result;
            foreach (InvokeMethodInfo item in methods)
            {
                var instance = item.ClassType.GetInstance();
                if (item.isDebug && this.isdebug)
                {
                    ////无参数
                    if (!item.isHasParameter)
                    {
                        try
                        {
                            MethodInfo method = item.ClassType.GetMethod(item.MethodName, new Type[] { });
                            result = method.Invoke(instance, null);
                            if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                            {
                                Exception ex = new Exception("Result is false,Stop invoking method");
                                resultMsg = new ReturnMessage(ex, callMsg);
                                return resultMsg;
                            }
                            resultMsg = new ReturnMessage(result, null, 0, callMsg.LogicalCallContext, callMsg);
                        }
                        catch (Exception ex)
                        {
                            resultMsg = new ReturnMessage(ex, callMsg);
                            throw new Exception("Invoke before method has Exception:",ex);
                        }
                        return resultMsg;
                    }
                    else
                    {
                        ///使用调用方法的参数
                        if (item.isInvokeParameter)
                        {
                            ////使用部分参数，需要传入参数名
                            if (item.parameterName.Length > 0)
                            {
                                Dictionary<string, object> paraDic = new Dictionary<string, object>();
                                object[] par = new object[item.parameterName.Length];
                                object[] pa = callMsg.Args;
                                for (int i = 0; i < pa.Length; i++)
                                {
                                    paraDic.Add(callMsg.GetArgName(i), callMsg.GetArg(i));
                                }
                                for (int j = 0; j < item.parameterName.Length; j++)
                                {
                                    par[j] = paraDic[item.parameterName[j]];
                                }
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, par.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, par);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, par, par.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    throw new Exception("Invoke before method has Exception:", ex);
                                }
                                return resultMsg;

                            }
                            else
                            {
                                ///全部参数
                                object[] pa = callMsg.Args;
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, pa.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, pa);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, pa, pa.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    throw new Exception("Invoke before method has Exception:", ex);
                                }
                                return resultMsg;

                            }
                        }
                        else
                        {
                            ///自己传入自定义参数
                            try
                            {
                                MethodInfo method = item.ClassType.GetMethod(item.MethodName,item.parameters.Select(p => p.GetType()).ToArray());
                                result = method.Invoke(instance, item.parameters);
                                if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                {
                                    Exception ex = new Exception("Result is false,Stop invoking method");
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    return resultMsg;
                                }
                                resultMsg = new ReturnMessage(result, item.parameters, item.parameters.Length, callMsg.LogicalCallContext, callMsg);
                            }
                            catch (Exception ex)
                            {
                                resultMsg = new ReturnMessage(ex, callMsg);
                                throw new Exception("Invoke before method has Exception:", ex);
                            }
                            return resultMsg;
                        }
                    }
                }
                else ////非debug类型
                {
                    if (!item.isHasParameter)
                    {
                        try
                        {
                            MethodInfo method = item.ClassType.GetMethod(item.MethodName, new Type[] { });
                            result = method.Invoke(instance, null);
                            if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                            {
                                Exception ex = new Exception("Result is false,Stop invoking method");
                                resultMsg = new ReturnMessage(ex, callMsg);
                                return resultMsg;
                            }
                            resultMsg = new ReturnMessage(result, null, 0, callMsg.LogicalCallContext, callMsg);
                        }
                        catch (Exception ex)
                        {
                            resultMsg = new ReturnMessage(ex, callMsg);
                            throw new Exception("Invoke before method has Exception:", ex);
                        }
                        return resultMsg;
                    }
                    else
                    {
                        if (item.isInvokeParameter)
                        {
                            if (item.parameterName.Length > 0)
                            {
                                Dictionary<string, object> paraDic = new Dictionary<string, object>();
                                object[] par = new object[item.parameterName.Length];
                                object[] pa = callMsg.Args;
                                for (int i = 0; i < pa.Length; i++)
                                {
                                    paraDic.Add(callMsg.GetArgName(i), callMsg.GetArg(i));
                                }
                                for (int j = 0; j < item.parameterName.Length; j++)
                                {
                                    par[j] = paraDic[item.parameterName[j]];
                                }
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, par.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, par);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, par, par.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    throw new Exception("Invoke before method has Exception:", ex);
                                }
                                return resultMsg;

                            }
                            else
                            {
                                object[] pa = callMsg.Args;
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, pa.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, pa);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, pa, pa.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    throw new Exception("Invoke before method has Exception:", ex);
                                }
                                return resultMsg;

                            }
                        }
                        else
                        {
                            try
                            {
                                MethodInfo method = item.ClassType.GetMethod(item.MethodName, item.parameters.Select(p => p.GetType()).ToArray());
                                result = method.Invoke(instance, item.parameters);
                                if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                {
                                    Exception ex = new Exception("Result is false,Stop invoking method");
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    return resultMsg;
                                }
                                resultMsg = new ReturnMessage(result, item.parameters, item.parameters.Length, callMsg.LogicalCallContext, callMsg);
                            }
                            catch (Exception ex)
                            {
                                resultMsg = new ReturnMessage(ex, callMsg);
                                throw new Exception("Invoke before method has Exception:", ex);
                            }
                            return resultMsg;
                        }
                    }
                }

            }
            return resultMsg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AfterAttrs"></param>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        /// <returns></returns>

        private IMessage PostProcess(IEnumerable<AfterMethodAttribute> AfterAttrs, IMessage msg, object[] param)
        {
            List<InvokeMethodInfo> methods = null;
            foreach (var att in AfterAttrs)
            {
                InvokeMethodInfo mfo = new InvokeMethodInfo();
                mfo.ClassType = att.t;
                mfo.MethodName = att.methodName;
                mfo.isDebug = att.isDebug;
                mfo.isHasParameter = att.isHasParameter;
                mfo.isInvokeParameter = att.isInvokersResult;
                mfo.orderNo = att.OrderNo;
                mfo.parameterName = att.parametersname;
                mfo.parameters = att.paramters;
                methods.Add(mfo);
            }
            IMessage resultMsg=null;
            IMethodCallMessage callMsg = (IMethodCallMessage)msg;
            methods = methods.OrderByDescending(p => p.orderNo).ToList();
            object result;
            foreach (InvokeMethodInfo item in methods)
            {
                var instance = item.ClassType.GetInstance();
                if (item.isDebug && this.isdebug)
                {
                    ////无参数
                    if (!item.isHasParameter)
                    {
                        try
                        {
                            MethodInfo method = item.ClassType.GetMethod(item.MethodName, new Type[] { });
                            result = method.Invoke(instance, null);
                            if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                            {
                                Exception ex = new Exception("Result is false,Stop invoking method");
                                resultMsg = new ReturnMessage(ex, callMsg);
                                return resultMsg;
                            }
                            resultMsg = new ReturnMessage(result, null, 0, callMsg.LogicalCallContext, callMsg);
                        }
                        catch (Exception ex)
                        {
                            resultMsg = new ReturnMessage(ex, callMsg);
                        }
                        return resultMsg;
                    }
                    else
                    {
                        ///使用调用方法的返回的结果
                        if (item.isInvokeParameter)
                        {
                            ////使用部分参数，需要传入参数名
                            if (item.parameterName.Length > 0)
                            {
                                Dictionary<string, object> paraDic = new Dictionary<string, object>();
                                object[] par = new object[item.parameterName.Length];
                                object[] pa = callMsg.Args;
                                for (int i = 0; i < pa.Length; i++)
                                {
                                    paraDic.Add(callMsg.GetArgName(i), callMsg.GetArg(i));
                                }
                                for (int j = 0; j < item.parameterName.Length; j++)
                                {
                                    par[j] = paraDic[item.parameterName[j]];
                                }
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, par.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, par);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, par, par.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                }
                                return resultMsg;

                            }
                            else
                            {
                                ///全部参数
                                object[] pa = callMsg.Args;
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, pa.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, pa);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, pa, pa.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                }
                                return resultMsg;

                            }
                        }
                        else
                        {
                            ///自己传入自定义参数
                            try
                            {
                                MethodInfo method = item.ClassType.GetMethod(item.MethodName, param.Select(p => p.GetType()).ToArray());
                                result = method.Invoke(instance, param);
                                if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                {
                                    Exception ex = new Exception("Result is false,Stop invoking method");
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    return resultMsg;
                                }
                                resultMsg = new ReturnMessage(result, param, param.Length, callMsg.LogicalCallContext, callMsg);
                            }
                            catch (Exception ex)
                            {
                                resultMsg = new ReturnMessage(ex, callMsg);
                            }
                            return resultMsg;
                        }
                    }
                }
                else ////非debug类型
                {
                    if (!item.isHasParameter)
                    {
                        try
                        {
                            MethodInfo method = item.ClassType.GetMethod(item.MethodName, new Type[] { });
                            result = method.Invoke(instance, null);
                            if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                            {
                                Exception ex = new Exception("Result is false,Stop invoking method");
                                resultMsg = new ReturnMessage(ex, callMsg);
                                return resultMsg;
                            }
                            resultMsg = new ReturnMessage(result, null, 0, callMsg.LogicalCallContext, callMsg);
                        }
                        catch (Exception ex)
                        {
                            resultMsg = new ReturnMessage(ex, callMsg);
                        }
                        return resultMsg;
                    }
                    else
                    {
                        if (item.isInvokeParameter)
                        {
                            if (item.parameterName.Length > 0)
                            {
                                Dictionary<string, object> paraDic = new Dictionary<string, object>();
                                object[] par = new object[item.parameterName.Length];
                                object[] pa = callMsg.Args;
                                for (int i = 0; i < pa.Length; i++)
                                {
                                    paraDic.Add(callMsg.GetArgName(i), callMsg.GetArg(i));
                                }
                                for (int j = 0; j < item.parameterName.Length; j++)
                                {
                                    par[j] = paraDic[item.parameterName[j]];
                                }
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, par.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, par);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, par, par.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                }
                                return resultMsg;

                            }
                            else
                            {
                                object[] pa = callMsg.Args;
                                try
                                {
                                    MethodInfo method = item.ClassType.GetMethod(item.MethodName, pa.Select(p => p.GetType()).ToArray());
                                    result = method.Invoke(instance, pa);
                                    if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                    {
                                        Exception ex = new Exception("Result is false,Stop invoking method");
                                        resultMsg = new ReturnMessage(ex, callMsg);
                                        return resultMsg;
                                    }
                                    resultMsg = new ReturnMessage(result, pa, pa.Length, callMsg.LogicalCallContext, callMsg);
                                }
                                catch (Exception ex)
                                {
                                    resultMsg = new ReturnMessage(null, callMsg);
                                }
                                return resultMsg;

                            }
                        }
                        else
                        {
                            try
                            {
                                MethodInfo method = item.ClassType.GetMethod(item.MethodName, param.Select(p => p.GetType()).ToArray());
                                result = method.Invoke(instance, param);
                                if (result != null && result.GetType() == typeof(Boolean) && (Boolean)result == false)
                                {
                                    Exception ex = new Exception("Result is false,Stop invoking method");
                                    resultMsg = new ReturnMessage(ex, callMsg);
                                    return resultMsg;
                                }
                                resultMsg = new ReturnMessage(result, param, param.Length, callMsg.LogicalCallContext, callMsg);
                            }
                            catch (Exception ex)
                            {
                                resultMsg = new ReturnMessage(ex, callMsg);
                            }
                            return resultMsg;
                        }
                    }
                }

            }
            return resultMsg;
        }

        private void ExceptionProcess(ExceptionCatchAttribute exAtt, Exception ex)
        {
            
                Type classType = exAtt.ClassType;
                
                string methodName = exAtt.MethodName;

                //object obj = classType.GetMethod(methodName, new Type[] { typeof(Exception) });
                object obj = classType.InvokeMember(methodName,BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null,new object[] { ex });
                //ParameterExpression param = Expression.Parameter(typeof(Exception), "i");
                //ParameterExpression type = Expression.Parameter(classType, "e");
                //MethodInfo method = classType.GetMethod(methodName, new Type[] { typeof(Exception) });
                //MethodCallExpression nexp = Expression.Call(null, method, new Expression[] { param });
                //Expression.Lambda<Func<Exception, object>>(nexp, type, param);

        
        }

        public  virtual IMessage CustructureProcess(IMessage callMsg)
        {
            IConstructionCallMessage cmsg = callMsg as IConstructionCallMessage;
            IConstructionReturnMessage remsg = this.InitializeServerObject(cmsg);
            RealProxy.SetStubData(this, remsg.ReturnValue);
            return remsg;
        }


        public virtual IMessage invokeProcess(IMessage msg)
        {
            IMethodCallMessage callMsg = msg as IMethodCallMessage;
            IMessage message;
            try
            {
                object[] args = callMsg.Args;
                var server = GetUnwrappedServer();
                object o = callMsg.MethodBase.Invoke(GetUnwrappedServer(), args);
                message = new ReturnMessage(o, args, args.Length, callMsg.LogicalCallContext, callMsg);
            }
            catch (Exception ex)
            {

                message = new ReturnMessage(ex, callMsg);
                throw new Exception("An error happened while invoke method", ex);
            }
            return message;
        }


        public void preprocess(IMessage msg)
        {
            throw new NotImplementedException();
        }

        public void postProcess(IMessage requestMsg, IMessage responseMsg)
        {
            throw new NotImplementedException();
        }
    }
}
