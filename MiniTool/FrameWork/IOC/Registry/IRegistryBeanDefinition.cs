using MiniTool.FrameWork.IOC.Bean;

namespace MiniTool.FrameWork.IOC.Registry
{
    public  interface IRegistryBeanDefinition
    {
         void RegitryBeanDefinition(string beanName, BeanDefinition beanDefinition);
    }
}
