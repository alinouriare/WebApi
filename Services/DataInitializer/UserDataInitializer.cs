using Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly IUserRepository userRepository;

        public UserDataInitializer(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public void InitializeData()
        {
            if (!userRepository.TableNoTracking.Any(p => p.UserName == "Admin"))
            {
                userRepository.Add(new User
                {
                    UserName = "Admin",
                    Email = "admin@site.com",
                    Age = 25,
                    FullName = "محمدجواد ابراهیمی",
                    Gender = GenderType.Male
                });
            }
        }
    }
}
