using Dapper;
using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Matiran.Library.Data.Repositories
{
    public class MemberRepository: IMemberRepository
    {

        private readonly IConfiguration _configuration;

        public MemberRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<MemberViewModel>> SearchMembers(string member)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                IEnumerable<MemberViewModel> foundMembers = await dbConnection.QueryAsync<MemberViewModel>(
                    "SELECT * FROM Members WHERE FName LIKE @Keyword OR LName LIKE @Keyword",new { Keyword = $"%{member}%" });

                return foundMembers;
            }
        }

        public async Task<MemberViewModel> AddMember(Member member)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                bool memberExists = await dbConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM Members WHERE FName = @FName AND LName = @LName",
                    new { FName = member.FName, LName = member.LName });

                if (memberExists)
                {
                    throw new InvalidOperationException($"عضو با نام '{member.FName} {member.LName}' در دیتابیس وجود دارد.");
                }

                string insertQuery = "INSERT INTO Members (FName, LName, NID, Mobile) OUTPUT INSERTED.Id VALUES (@FName, @LName, @NID, @Mobile)";

                int insertedMemberId = await dbConnection.ExecuteScalarAsync<int>(insertQuery, member);

                MemberViewModel insertedMemberViewModel = await GetMemberById(insertedMemberId);

                return insertedMemberViewModel;
            }
        }

        public async Task<bool> RemoveMember(int memberId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();
                bool memberExists = await dbConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM Members WHERE Id = @Id", new { Id = memberId });
                if (!memberExists)
                {
                    throw new InvalidOperationException($"عضوی با شناسه '{memberId.ToString()}' در دیتابیس وجود ندارد.");
                }

                // افراز کتاب‌های به امانت گرفته شده توسط این عضو
                bool hasBorrowedBooks = await dbConnection.ExecuteScalarAsync<bool>("SELECT TOP (1) * FROM Rents WHERE MemberId = @MemberId AND IsReturned = 0",
                                                                                     new { MemberId = memberId });

                if (hasBorrowedBooks)
                {
                    throw new InvalidOperationException($"عضو با شناسه '{memberId}' کتاب به امانت گرفته و هنوز بازگردانده و نمی‌توانید آن را حذف کنید.");
                }

                await dbConnection.ExecuteAsync("DELETE FROM Members WHERE Id = @MemberId", new { MemberId = memberId });

                return true;
            }
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembers()
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                int memberCount = await dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Members");

                if (memberCount == 0)
                {
                    throw new InvalidOperationException("هیچ عضوی در پایگاه داده وجود ندارد");
                }

                return await dbConnection.QueryAsync<MemberViewModel>("SELECT * FROM Members");
            }
        }

        public async Task<MemberViewModel> GetMemberById(int memberId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                MemberViewModel memberViewModel = await dbConnection.QueryFirstOrDefaultAsync<MemberViewModel>(
                    "SELECT Id, FName, LName, NID, Mobile FROM Members WHERE Id = @Id",
                    new { Id = memberId });

                if (memberViewModel == null)
                {
                    throw new InvalidOperationException($"عضو با شناسه '{memberId}' در دیتابیس یافت نشد.");
                }

                return memberViewModel;
            }
        }

    }
}
