using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IMemberRepository
    {
        Task<IEnumerable<MemberViewModel>> SearchMembers(string member);
        Task<MemberViewModel> AddMember(Member member);
        Task<bool> RemoveMember(int memberId);
        Task<IEnumerable<MemberViewModel>> GetAllMembers();
        Task<MemberViewModel> GetMemberById(int memberId);
    }
}
