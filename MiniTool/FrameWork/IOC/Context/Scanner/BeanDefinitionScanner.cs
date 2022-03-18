using System;
using System.Collections.Generic;
using MiniTool.FrameWork.IOC.Registry;
using MiniTool.FrameWork.IOC.Bean;
using System.Reflection;
using System.ComponentModel;


namespace MiniTool.FrameWork.IOC.Context.Scanner
{
    internal class BeanDefinitionScanner:CandidateComponentProvider
    {
        
        internal IRegistryBeanDefinition registry;

        private BeanNameGenerator beanNameGenertor = new BeanNameGenerator();
        private BeanScopeConfig scopeCnfig = new BeanScopeConfig();

      
        public BeanDefinitionScanner(IRegistryBeanDefinition registry):this(registry,true){

        }
        
         public BeanDefinitionScanner(IRegistryBeanDefinition registry,bool isDefault)
        {
            this.registry = registry;
            if (isDefault)
            {
                RegisterDefaultFilters();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void doScan(params Assembly[] assemblies){

            if (assemblies == null && assemblies.Length == 0) throw new ArgumentNullException("Can not scan the assemblies,cause they are null");
            foreach (var item in assemblies)
            {
                List<BeanDefinition> candidates = ScanCandidateComponents(item);
                {
                    foreach (BeanDefinition ben in candidates)
                    {
                        string beanName = beanNameGenertor.GeneratorBeanName(ben);
                        ben.BeanName = beanName;

                        ScopType scopeName = scopeCnfig.ConfigScope(ben);
                        ben.ScopeName = scopeName;
                        RegistryBeanDefinition(ben, registry);
                    }
                }
            }
        }

        private void RegistryBeanDefinition(BeanDefinition definition,IRegistryBeanDefinition registry)
        {
            BeanDefinitionUtils.BeanRegistry(definition, registry);
        }


    }
}
