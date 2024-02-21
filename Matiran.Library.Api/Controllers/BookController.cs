using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;
using Microsoft.AspNetCore.Mvc;

namespace Matiran.Library.Api.Controllers
{
    [Route("MatiranApi/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("SearchBooks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookViewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> SearchBooks(string BookTitle)
        {
            try
            {
                IEnumerable<BookViewModel> _book = await _bookService.SearchBooks(BookTitle);

                if (_book != null && _book.Any())
                {
                    return Ok(_book); // برای 200 OK
                }
                else
                {
                    return NotFound("هیچ کتابی با این عنوان یافت نشد." ); // برای 404 Not Found
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"خطا: {ex.Message}" }); // برای 500 Internal Server Error
            }
        }

        [HttpPost("AddBook")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddBook(Book book)
        {
            try
            {
                BookViewModel insertedBookViewModel = await _bookService.AddBook(book);

                return Ok(insertedBookViewModel); // برای 200 OK
            }
            catch (ArgumentException ex)
            {
                return BadRequest( ex.Message ); // برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"خطا: {ex.Message}" }); // برای 500 Internal Server Error
            }
        }

        [HttpDelete("RemoveBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveBook(int BookId)
        {
            try
            {
                bool _result = await _bookService.RemoveBook(BookId);
                var books = await _bookService.GetAllBooks();
                return Ok(books);// برای 200 OK
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // برای 400 Bad Request
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,  $"خطا: {ex.Message}"); // برای 500 Internal Server Error
            }
        }

        [HttpGet("GetAllBooks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                IEnumerable<BookViewModel> allBooks = await _bookService.GetAllBooks();

                if (allBooks != null && allBooks.Any())
                {
                    return Ok(allBooks); // برای 200 OK
                }
                else
                {
                    return NotFound( "هیچ کتابی یافت نشد." ); // برای 404 Not Found
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"خطا: {ex.Message}" }); // برای 500 Internal Server Error
            }
        }


    }


}
