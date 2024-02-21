using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;
using Microsoft.AspNetCore.Mvc;

namespace Matiran.Library.Api.Controllers
{
    [Route("MatiranApi/[controller]")]
    [ApiController]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet("SearchMembers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MemberViewModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> SearchMembers(string memberName)
        {
            try
            {
                IEnumerable<MemberViewModel> members = await _memberService.SearchMembers(memberName);

                if (members != null && members.Any())
                {
                    return Ok(members); // برای 200 OK
                }
                else
                {
                    return NotFound("هیچ عضوی یافت نشد."); // برای 404 Not Found
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

        [HttpPost("AddMember")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> AddMember(Member member)
        {

            try
            {
                MemberViewModel insertedMemberViewModel = await _memberService.AddMember(member);

                return Ok(insertedMemberViewModel); // برای 200 OK;
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

        [HttpDelete("RemoveMember")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RemoveMember(int memberId)
        {
            try
            {
                bool result = await _memberService.RemoveMember(memberId);
                var members = await _memberService.GetAllMembers();
                return Ok(members);// برای 200 OK
            }
            //catch (ArgumentException ex)
            //{
            //    return BadRequest(ex.Message); // برای 400 Bad Request
            //}
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,    $"خطا: {ex.Message}" ); // برای 500 Internal Server Error
            }
        }

        [HttpGet("GetAllMembers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MemberViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> GetAllMembers()
        {
            try
            {
                IEnumerable<MemberViewModel> allMembers = await _memberService.GetAllMembers();
                if (allMembers != null && allMembers.Any())
                {
                    return Ok(allMembers); // برای 200 OK
                }
                else
                {
                    return NotFound("هیچ کتابی یافت نشد."); // برای 404 Not Found
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطا: {ex.Message}");
            }
        }


    }
}
