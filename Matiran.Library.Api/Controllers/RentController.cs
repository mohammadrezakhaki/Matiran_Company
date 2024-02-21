using Microsoft.AspNetCore.Mvc;
using Matiran.Library.Model;
using Matiran.Library.Data.Contracts;

namespace Matiran.Library.Api.Controllers
{
    [Route("MatiranApi/[controller]")]
    [ApiController]
    public class RentController : Controller
    {
        private readonly IRentService _rentService;

        public RentController(IRentService rentService)
        {
            _rentService = rentService;
        }

        [HttpGet("SearchBooksRent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RentviewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> SearchBooksRent(string BookTitle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BookTitle))
                {
                    return BadRequest("عنوان کتاب نمی‌تواند خالی باشد.");
                }

                IEnumerable<RentviewModel> foundRents = await _rentService.SearchBooksRent(BookTitle);

                if (foundRents == null || !foundRents.Any())
                {
                    return NotFound();// برای 404 Not Found
                }

                return Ok(foundRents);// برای 200 OK
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);// برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطا: {ex.Message}");// برای 500 Internal Server Error
            }
        }

        [HttpGet("SearchMembersRent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RentviewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> SearchMembersRent(string MemberTitle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MemberTitle))
                {
                    return BadRequest("نام یا نام خانوادگی عضو نمی‌تواند خالی باشد.");
                }

                IEnumerable<RentviewModel> foundRents = await _rentService.SearchMemebersRent(MemberTitle);

                if (foundRents == null || !foundRents.Any())
                {
                    return NotFound();// برای 404 Not Found
                }

                return Ok(foundRents);// برای 200 OK
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);// برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطا: {ex.Message}");// برای 500 Internal Server Error
            }
        }

        [HttpPost("AddRent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RentviewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddRent(Rent rent)
        {
            try
            {
                var rentViewModel = await _rentService.AddRent(rent);

                if (rentViewModel == null)
                {
                    return BadRequest("خطا در اضافه کردن امانت.");
                }

                return Ok(rentViewModel);// برای 200 OK
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);// برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطا: {ex.Message}");// برای 500 Internal Server Error
            }
        }

        [HttpPost("ReturnBook")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> ReturnBook(ReturnBookRequest returnBookRequest)
        {
            try
            {
                var success = await _rentService.ReturnBook(returnBookRequest);

                if (success == null)
                {
                    return NotFound($"خطا در ثبت امانت کتاب");
                }

                return Ok(true);// برای 200 OK
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);// برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطا: {ex.Message}");// برای 500 Internal Server Error
            }
        }

        [HttpGet("LateReturn")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RentviewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> LateReturn()
        {
            try
            {
                var lateReturns = await _rentService.LateReturn();

                return Ok(lateReturns);// برای 200 OK
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);// برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطا: {ex.Message}");// برای 500 Internal Server Error
            }
        }

        [HttpGet("GetAllRents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RentviewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllRents()
        {
            try
            {
                var allRents = await _rentService.GetAllRents();

                return Ok(allRents);// برای 200 OK
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);// برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"خطا: {ex.Message}");// برای 500 Internal Server Error
            }
        }

    }

}

