using System;

namespace MiniTool.Attributes
{
    /// <summary>
    /// 标记可以注入实现的接口
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface,AllowMultiple=false)]
    public class InjectInterfaceAttribute:Attribute
    {
    }
}
