using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace ServiceApi.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Content("Hello core world");
        }
    }
}
