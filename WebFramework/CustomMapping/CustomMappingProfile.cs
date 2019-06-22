using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.CustomMapping
{
   public class CustomMappingProfile :Profile
    {

        public CustomMappingProfile(IEnumerable<IHaveCustomMapping> haveCustomMappings)
        {
            foreach (var item in haveCustomMappings)
            {
                item.CreateMappings(this);
            }
        }
    }
}
