﻿using AutoMapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebFramework.Api;

namespace MyApi.Models
{
   

        public class PostDto : BaseDto<PostDto, Post, Guid>
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public int CategoryId { get; set; }
            public int AuthorId { get; set; }
        }

        public class PostSelectDto : BaseDto<PostSelectDto, Post, Guid>
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string CategoryName { get; set; } //=> Category.Name
            public string AuthorFullName { get; set; } //=> Author.FullName
            public string FullTitle { get; set; } // => mapped from "Title (Category.Name)"

      

            public override void CustomMapping(IMappingExpression<Post, PostSelectDto> mapping)
            {
                mapping.ForMember(
                       dest => dest.FullTitle,
                      config => config.MapFrom(src => $"{src.Title} ({src.Category.Name})"));    
            }
        }
    }

