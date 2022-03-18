using System;

namespace MiniTool.FrameWork.AOP
{
    public class InvokeMethodInfo
    {
        public Type ClassType { get; set; }

        public string MethodName { get; set; }

        public Boolean isHasParameter { get; set; }

        public Boolean isInvokeParameter { get; set; }

        public Boolean isDebug { get; set; }

        public string[] parameterName { get; set; }

        public object[] parameters { get; set; }

        public Int32 orderNo { get; set; }
    }
}
