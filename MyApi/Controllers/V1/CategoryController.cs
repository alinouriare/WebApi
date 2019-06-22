using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using WebFramework.Api;

namespace MyApi.Controllers.V1
{
  
    public class CategoryController : CrudController<CategoryDto, Category>
    {
        public CategoryController(IRepository<Category> repository) : base(repository)
        {
        }
    }
}