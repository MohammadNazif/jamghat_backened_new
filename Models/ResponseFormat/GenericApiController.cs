using Microsoft.AspNetCore.Mvc;

namespace jamghat.Models.ResponseFormat
{
    [ApiController]
    public abstract class GenericApiController : ControllerBase
    {
        // ---------------- ASYNC RESPONSE ----------------
        protected async Task<IActionResult> RespondAsync<T>(
            Func<Task<T>> func,
            string successMessage = "Success")
        {
            try
            {
                var result = await func();

                if (result == null)
                {
                    return NotFound(new ApiResponse<T>(false, "Resource not found"));
                }

                return Ok(new ApiResponse<T>(true, successMessage, result));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<object>(false, "Unauthorized"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500,
                    new ApiResponse<object>(false, "An unexpected error occurred"));
            }
        }

        // ---------------- SYNC RESPONSE ----------------
        protected IActionResult Respond<T>(
            T result,
            string successMessage = "Success")
        {
            if (result == null)
            {
                return NotFound(new ApiResponse<T>(false, "Resource not found"));
            }

            return Ok(new ApiResponse<T>(true, successMessage, result));
        }

        protected IActionResult Fail(string message)
        {
            return BadRequest(new ApiResponse<object>(false, message));
        }
    }
}
