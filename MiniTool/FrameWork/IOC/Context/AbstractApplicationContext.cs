
using System.ComponentModel;
namespace MiniTool.FrameWork.IOC.Context
{

    public abstract class AbstractApplicationContext
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract object GetBean(string Name);
    }
}
