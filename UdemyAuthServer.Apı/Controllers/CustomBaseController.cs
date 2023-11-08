﻿using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;

namespace UdemyAuthServer.Apı.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult ActionResultInstance<T>(Response<T> response) where T : class
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
