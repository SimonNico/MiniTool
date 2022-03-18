using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTool.FrameWork.IOC.Bean
{
    public  class BeanDefinition
    {
        private ScopType _scopename = ScopType.Singleton;

        public string BeanName { get; set; }

        public Type BeanClass { get; set; }

        /// <summary>
        /// 生命周期类型  singleton or prototype
        /// </summary>
        public ScopType ScopeName
        {
            get
            {
                return this._scopename;
            }
            set
            {
                this._scopename = value;
            }
        }
    }
}
