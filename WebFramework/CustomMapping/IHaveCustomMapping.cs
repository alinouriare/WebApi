using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.CustomMapping
{
   public interface IHaveCustomMapping
    {
        void CreateMappings(Profile profile);
    }
}
