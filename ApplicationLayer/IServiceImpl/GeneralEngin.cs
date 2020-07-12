using ApplicationLayer.IService;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationLayer.IServiceImpl
{
    public class GeneralEngin : IEngin
    {
        private IServiceProvider _serviceProvider;
        public GeneralEngin(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public T Resolve<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
