using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> SearchMembers(string memberName);
        Task<MemberViewModel> AddMember(Member member);
        Task<bool> RemoveMember(int memberId);
        Task<IEnumerable<MemberViewModel>> GetAllMembers();
    }

}
