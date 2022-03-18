
using System.ComponentModel;
namespace MiniTool.Log
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum LogLevel
    {
        /// <summary>
        /// 信息级别
        /// </summary>
        Info,

        /// <summary>
        /// debug级别
        /// </summary>
        Debug,

        /// <summary>
        /// 错误级别
        /// </summary>
        Error,

        /// <summary>
        /// 致命级别
        /// </summary>
        Fatal
    }
}
