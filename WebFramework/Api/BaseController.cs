using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebFramework.Api
{
    [ApiController]
   
    [ApiResultFilter]
    
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    //[Route("api/[controller]")]//Query String 
    public class BaseController:ControllerBase
    {

        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
   
    }
}
