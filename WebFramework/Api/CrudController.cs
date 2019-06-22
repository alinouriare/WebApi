using AutoMapper.QueryableExtensions;
using Data;
using Entities.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebFramework.Api
{

    [ApiVersion("1")]
    public class CrudController<TDto, TSelectDto, TEntity, TKey> : BaseController
            where TDto : BaseDto<TDto, TEntity, TKey>, new()
            where TSelectDto : BaseDto<TSelectDto, TEntity, TKey>, new()
            where TEntity : BaseEntity<TKey>, new()


    {

        private readonly IRepository<TEntity> _repository;

        public CrudController(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        [HttpGet]

        public virtual async Task<ActionResult<List<TSelectDto>>> Get(CancellationToken cancellationToken)
        {



            var list = await _repository.TableNoTracking.ProjectTo<TSelectDto>()

            .ToListAsync(cancellationToken);


            return Ok(list);
        }



        [HttpGet("{id:guid}")]
        public virtual async Task<ActionResult<TSelectDto>> Get(TKey id, CancellationToken cancellationToken)
        {



            var dto = await _repository.TableNoTracking.ProjectTo<TSelectDto>()
            .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();



            return Ok(dto);

        }


        [HttpPost]
        public virtual async Task<ActionResult<TSelectDto>> Craete(TDto postDto, CancellationToken cancellationToken)
        {

            var model = postDto.ToEntity();

            await _repository.AddAsync(model, cancellationToken);



            var Result = await _repository.TableNoTracking.ProjectTo<TSelectDto>().SingleOrDefaultAsync(postdto => postdto.Id.Equals(model.Id), cancellationToken);





            return Result;


        }
        [HttpPut("{id:guid}")]
        public virtual async Task<ActionResult<TSelectDto>> Update(TKey id, TDto dto, CancellationToken cancellationToken)
        {

            var model = await _repository.GetByIdAsync(cancellationToken, id);



            dto.ToEntity(model);



            await _repository.UpdateAsync(model, cancellationToken);





            var resultDto = await _repository.TableNoTracking.ProjectTo<TSelectDto>().SingleOrDefaultAsync(postdto => postdto.Id.Equals(model.Id), cancellationToken);



            return resultDto;

        }
        [HttpDelete("{id:guid}")]
        public virtual async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var model = await _repository.GetByIdAsync(cancellationToken, id);

            await _repository.DeleteAsync(model, cancellationToken);
            return Ok();


        }


    }
    public class CrudController<TDto, TSelectDto, TEntity> : CrudController<TDto, TSelectDto, TEntity, int>
     where TDto : BaseDto<TDto, TEntity, int>, new()
             where TSelectDto : BaseDto<TSelectDto, TEntity, int>, new()
     where TEntity : BaseEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository) : base(repository)
        {

        }
    }


    public class CrudController<TDto, TEntity> : CrudController<TDto, TDto, TEntity, int>
       where TDto : BaseDto<TDto, TEntity, int>, new()
       where TEntity : BaseEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository)
            : base(repository)
        {
        }
    }
}
