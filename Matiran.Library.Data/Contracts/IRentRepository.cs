using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IRentRepository
    {
        Task<IEnumerable<RentviewModel>> SearchBooksRent(string BookTitle);
        Task<IEnumerable<RentviewModel>> SearchMembersRent(string MemberTitle);
        Task<RentviewModel> AddRent(Rent rent);
        Task<RentviewModel> ReturnBook(ReturnBookRequest returnBookRequest);
        Task<IEnumerable<RentviewModel>> LateReturn();
        Task<IEnumerable<RentviewModel>> GetAllRents();
        Task<int> GetBorrowedBooksCount(int memberId);
        Task<RentviewModel> GetRentDetails(int memberId, int bookId);
        Task<bool> IsBookBorrowed(int memberId, int bookId);
    }
}
