using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Data;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;
using WebFramework;

namespace MyApi.Controllers.V1
{
    //[ApiController]
    //[AllowAnonymous]
    //[ApiResultFilter]
    //[Route("api/[controller]")]
    public class OldPostController : ControllerBase
    {

        private readonly IRepository<Post> _repository;

        public OldPostController(IRepository<Post> repository)
        {
            _repository = repository;
        }
        [HttpGet]
    
        public async Task<ActionResult<List<PostDto>>> Get(CancellationToken cancellationToken)
        {

            #region Old

            //var post = await _repository.TableNoTracking.Include(p => p.Author)
            //    .Include(c => c.Category).ToListAsync(cancellationToken);
            //var post = await _repository.TableNoTracking.
            //    Include(p=>p.Category).Include(p=>p.Author).
            //    ToListAsync(cancellationToken);

            //var list = post.Select(p =>
            //  {

            //    var dto=  AutoMapper.Mapper.Map<PostDto>(p);
            //      return dto;
            //  }).ToList();


            //var list = await _repository.TableNoTracking.Select(r => new PostDto
            //{

            //    Id = r.Id,
            //    Title = r.Title,
            //    Description = r.Description,
            //    AuthorId = r.AuthorId,
            //    AuthorFullName = r.Author.FullName,
            //    CategoryId = r.CategoryId,
            //    CategoryName = r.Category.Name


            //}).ToListAsync(cancellationToken);
            #endregion


            var list = await _repository.TableNoTracking.ProjectTo<PostSelectDto>()
            //    .Where(postdto => postdto.CategoryName.Contains("test") ||

            //postdto.AuthorFullName.Contains("test"))
            .ToListAsync(cancellationToken);
                

            return Ok(list);
        }


        //[HttpGet("{id:guid}")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PostSelectDto>> Get(Guid id, CancellationToken cancellationToken)
        {

            #region Old
            //var model = await _repository.GetByIdAsync(cancellationToken, id);


            //var Dto = AutoMapper.Mapper.Map<PostDto>(model);

            var dto = await _repository.TableNoTracking.ProjectTo<PostSelectDto>()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (dto == null)
                return NotFound();


            //Post a = null; //example find database entity with includ
            //var result = PostDto.FromEntity(a);




            //var pDto = new PostDto
            //{
            //    Id = model.Id,
            //    Title = model.Title,
            //    Description = model.Description,
            //    CategoryId = model.CategoryId,
            //    CategoryName = model.Category.Name,
            //    AuthorId = model.AuthorId,
            //    AuthorFullName = model.Author.FullName




            //};

            return Ok(dto);
            #endregion
        }


        [HttpPost]
        public async Task<ActionResult<PostSelectDto>> Craete(PostDto postDto, CancellationToken cancellationToken)
        {
            #region Old

            //var model = new Post
            //{

            //    Title = postDto.Title,
            //    Description = postDto.Description,
            //    CategoryId = postDto.CategoryId,
            //    AuthorId = postDto.AuthorId
            //};

            //var model = AutoMapper.Mapper.Map<Post>(postDto);

            var model = postDto.ToEntity();

            await _repository.AddAsync(model, cancellationToken);


            //await _repository.LoadReferenceAsync(model, p => p.Category, cancellationToken);
            //await _repository.LoadReferenceAsync(model, p => p.Author, cancellationToken);

            //var modelDto = await _repository.TableNoTracking.Include(p => p.Category).
            //    Include(c => c.Author).SingleOrDefaultAsync(d => d.Id == model.Id, cancellationToken);


            //var Result = new PostDto
            //{

            //    Id = model.Id,
            //    Title = model.Title,
            //    Description = model.Description,
            //    CategoryId = model.CategoryId,
            //    AuthorId = model.AuthorId,
            //    CategoryName = model.Category.Name,
            //    AuthorFullName = model.Author.FullName


            //};
            //var Result = AutoMapper.Mapper.Map<PostDto>(modelDto);
            var Result = await _repository.TableNoTracking.ProjectTo<PostSelectDto>().SingleOrDefaultAsync(postdto => postdto.Id == model.Id,cancellationToken);


            //var resultDto = await _repository.TableNoTracking.Select(p => new PostDto
            //{
            //    Id = p.Id,
            //    Title = p.Title,
            //    Description = p.Description,
            //    CategoryId = p.CategoryId,
            //    AuthorId = p.AuthorId,
            //    AuthorFullName = p.Author.FullName,
            //    CategoryName = p.Category.Name
            //}).SingleOrDefaultAsync(p => p.Id == model.Id, cancellationToken);


            return Result;
            #endregion

        }
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PostSelectDto>> Update(Guid id, PostDto dto, CancellationToken cancellationToken)
        {

            var model = await _repository.GetByIdAsync(cancellationToken, id);

            #region Old

            //AutoMapper.Mapper.Map(dto, model);

            dto.ToEntity(model);

            //model.Title = dto.Title;
            //model.Description = dto.Description;
            //model.CategoryId = dto.CategoryId;
            //model.AuthorId = dto.AuthorId;


            await _repository.UpdateAsync(model, cancellationToken);

            //var resultDto = await _repository.TableNoTracking.Select(p => new PostDto
            //{
            //    Id = p.Id,
            //    Title = p.Title,
            //    Description = p.Description,
            //    CategoryId = p.CategoryId,
            //    CategoryName = p.Category.Name,
            //    AuthorId = p.AuthorId,
            //    AuthorFullName = p.Author.FullName,



            //}).SingleOrDefaultAsync(c => c.Id == model.Id);



            var resultDto = await _repository.TableNoTracking.ProjectTo<PostSelectDto>().SingleOrDefaultAsync(postdto => postdto.Id == model.Id, cancellationToken);

            #endregion


            return resultDto;

        }
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var model = await _repository.GetByIdAsync(cancellationToken, id);

            await _repository.DeleteAsync(model, cancellationToken);
            return Ok();


        }
    }
}