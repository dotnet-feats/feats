using System;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Management
{
    [ApiController]
    [Route("/")]
    public class HomeController
    {
        public string Get()
        {
            return "mooo";
        }
    }
}

