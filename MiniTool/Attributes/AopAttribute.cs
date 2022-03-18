using System;
using System.Runtime.Remoting.Proxies;
using MiniTool.FrameWork.AOP;
namespace MiniTool.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
    public sealed class AopAttribute:ProxyAttribute
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            AopProxy aop = new AopProxy(serverType);
            return aop.GetTransparentProxy() as MarshalByRefObject;
        }
    }
}
