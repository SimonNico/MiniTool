using System;

namespace MiniTool.FrameWork.IOC.Attributes
{
    /// <summary>
    /// 标记可以注入实现的接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface,AllowMultiple=true)]
    public class InjectInterfaceAttribute:Attribute
    {
    }
}
