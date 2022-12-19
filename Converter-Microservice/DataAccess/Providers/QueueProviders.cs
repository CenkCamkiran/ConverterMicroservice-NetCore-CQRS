using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Providers
{
    public class QueueProviders
    {
        private IServiceProvider serviceProvider { get; set; }

        public void SetServices(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public static QueueProviders Current
        {
            get
            {
                return SingletonProvider<QueueProviders>.Instance; //Yardımcı koda bak.
            }
        }
    }
}
