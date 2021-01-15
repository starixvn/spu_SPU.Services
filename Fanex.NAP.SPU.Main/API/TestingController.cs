using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "testing";
        }
    }
}
