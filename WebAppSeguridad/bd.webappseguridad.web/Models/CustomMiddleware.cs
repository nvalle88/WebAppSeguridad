﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bd.webappseguridad.web.Models
{
    public class CustomMiddleware
    {
            private readonly RequestDelegate _next;
            public CustomMiddleware(RequestDelegate next)
            {
                _next = next;
            }
            public async Task Invoke(HttpContext context)
            {
                await _next.Invoke(context);
            }
    }
}
