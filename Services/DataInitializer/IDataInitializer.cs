using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.DataInitializer
{
   public interface IDataInitializer: IScopedDependency
    {
        void InitializeData();
    }
}
