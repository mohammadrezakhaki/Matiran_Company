using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;

namespace Matiran.Library.Data.Repositories
{
    public class MemberService : IMemberService
    {
        IMemberRepository _memberRepository;

        public MemberService(IMemberRepository bookRepository)
        {
            _memberRepository = bookRepository;
        }

        public async Task<IEnumerable<MemberViewModel>> SearchMembers(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                throw new ArgumentException("نام عضو نمی‌تواند خالی باشد.");
            }

            IEnumerable<MemberViewModel> foundMembers = await _memberRepository.SearchMembers(memberName);

            if (foundMembers == null || !foundMembers.Any())
            {
                throw new ArgumentException("هیچ عضوی با این نام یافت نشد.");
            }

            return foundMembers;
        }

        public async Task<MemberViewModel> AddMember(Member member)
        {
            if (string.IsNullOrWhiteSpace(member.FName))
            {
                throw new ArgumentException("نام عضو نمی‌تواند خالی باشد");
            }
            if (string.IsNullOrWhiteSpace(member.LName))
            {
                throw new ArgumentException("نام خانوادگی عضو نمی‌تواند خالی باشد");
            }


            return await _memberRepository.AddMember(member);
        }

        public async Task<bool> RemoveMember(int memberId)
        {
            bool memberExists = await _memberRepository.RemoveMember(memberId);

            if (!memberExists)
            {
                throw new ArgumentException($"عضو با شناسه '{memberId}' یافت نشد.");
            }

            return true;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembers()
        {
            IEnumerable<MemberViewModel> allMembers = await _memberRepository.GetAllMembers();

            if (allMembers == null || !allMembers.Any())
            {
                throw new ArgumentException("هیچ عضوی یافت نشد.");
            }

            return allMembers;
        }
    }
}
