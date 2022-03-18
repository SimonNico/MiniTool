using System.Runtime.Remoting.Messaging;

namespace MiniTool.FrameWork.AOP
{
    public interface IAdivce
    {
        void preprocess(IMessage msg);

        void postProcess(IMessage requestMsg, IMessage responseMsg);
    }
}
