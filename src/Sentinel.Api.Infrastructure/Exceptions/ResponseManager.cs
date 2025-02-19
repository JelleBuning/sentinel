using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sentinel.Api.Infrastructure.Exceptions
{
    public class ResponseManager : ControllerBase
    {
        // Returns status code based on thrown exception
        public IActionResult ReturnResponse(Exception ex)
        {
            return ex.InnerException switch
            {
                NotFoundException => NotFound(new ResponseMessage
                {
                    Message = ex.Message,
                }),
                UnauthorizedException => Unauthorized(new ResponseMessage
                {
                    Message = ex.Message,
                }),
                BadRequestException => BadRequest(new ResponseMessage
                {
                    Message = ex.Message,
                }),
                ForbiddenException => StatusCode(StatusCodes.Status403Forbidden, new ResponseMessage
                {
                    Message = ex.Message,
                }),
                InternalServerException => StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    Message = ex.Message,
                }),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            };
        }
    }
}

