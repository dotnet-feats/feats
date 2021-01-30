using System.Diagnostics.CodeAnalysis;
using System.Net;
using Feats.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Feats.Evaluations
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var errorContext = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            
            switch (errorContext.Error)
            {
                case ValidationException ex:
                    return this.Problem(
                        statusCode: (int)HttpStatusCode.BadRequest,
                        title: "Validation Exception",
                        detail: ex.Message
                    );
                case NotFoundException ex:
                    return this.Problem(
                        statusCode: (int)HttpStatusCode.NotFound,
                        title: "Not Found Exception",
                        detail: ex.Message
                    );
                case ConflictException ex:
                    return this.Problem(
                        statusCode: (int)HttpStatusCode.Conflict,
                        title: "Not Found Exception",
                        detail: ex.Message
                    );
                default:
                    return this.Problem(
                        statusCode: (int)HttpStatusCode.InternalServerError,
                        title: "Unhandled Exception",
                        detail: errorContext.Error.Message
                    );
            }
        }
    }
}