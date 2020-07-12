using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationLayer.IService
{
    public interface IEngin
    {
        T Resolve<T>();
    }
}
