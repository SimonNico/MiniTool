using System;

namespace MiniTool.Attributes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Class,AllowMultiple=false)]
    public sealed class TransactionalAttribute:Attribute
    {
    }
}
