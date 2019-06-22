using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using WebFramework;

namespace MyApi.Controllers.V2
{
    [ApiVersion("2")]
    public class PostController : V1.PostController
    {
        public PostController(IRepository<Post> repository) : base(repository)
        {
        }
        [HttpGet("Test")]
        public ActionResult Test()
        {
            return Content("This is test");
        }



        public async override Task<ActionResult<List<PostSelectDto>>> Get(CancellationToken cancellationToken)
        {
            return await Task.FromResult(new List<PostSelectDto>
            {
                new PostSelectDto
                {
                     FullTitle = "FullTitle",
                     AuthorFullName =  "AuthorFullName",
                     CategoryName = "CategoryName",
                     Description = "Description",
                     Title = "Title",
                }
            });
        }


        [NonAction]
        public override Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override Task<ActionResult<PostSelectDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            return base.Get(id, cancellationToken);
        }

        public override Task<ActionResult<PostSelectDto>> Craete(PostDto postDto, CancellationToken cancellationToken)
        {
            return base.Craete(postDto, cancellationToken);
        }

        public override Task<ActionResult<PostSelectDto>> Update(Guid id, PostDto dto, CancellationToken cancellationToken)
        {
            return base.Update(id, dto, cancellationToken);
        }
    }
}