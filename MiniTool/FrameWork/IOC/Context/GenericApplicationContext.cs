using MiniTool.FrameWork.IOC.Registry;
using MiniTool.FrameWork.IOC.DefaultContext;
using MiniTool.FrameWork.IOC.Bean;
using System.ComponentModel;

namespace MiniTool.FrameWork.IOC.Context
{
    public  class GenericApplicationContext : AbstractApplicationContext, IRegistryBeanDefinition
    {
        private DefaultListableBeanFactory beanFactory;
    
        public GenericApplicationContext()
        {
            this.beanFactory = new DefaultListableBeanFactory();
        }

        public override object GetBean(string Name)
        {
            return this.beanFactory.GetBean(Name);
        }

        public T GetBean<T>() where T:class
        {
            return this.beanFactory.GetBean<T>();
        }
        
        public DefaultListableBeanFactory GetBeanFactory()
        {
            return this.beanFactory;
        }

        /// <summary>
        /// BeanDefinition对象注册到IOC的方法
        /// </summary>
        /// <param name="beanName"></param>
        /// <param name="beanDefinition"></param>
        public  void RegitryBeanDefinition(string beanName, BeanDefinition beanDefinition)
        {
            this.beanFactory.RegitryBeanDefinition(beanName, beanDefinition);
        }
    }
}
