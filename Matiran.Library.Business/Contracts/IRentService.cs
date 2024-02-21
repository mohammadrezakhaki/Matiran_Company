using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IRentService
    {
        Task<IEnumerable<RentviewModel>> SearchBooksRent(string BookTitle);
        Task<IEnumerable<RentviewModel>> SearchMemebersRent(string MemberTitle);
        Task<RentviewModel> AddRent(Rent rent);
        Task<RentviewModel> ReturnBook(ReturnBookRequest returnBookRequest);
        Task<IEnumerable<RentviewModel>> LateReturn();
        Task<IEnumerable<RentviewModel>> GetAllRents();
        Task<RentviewModel> GetRentDetails(int memberId, int bookId);
        Task<bool> IsBookBorrowed(int memberId, int bookId);
        Task<int> GetBorrowedBooksCount(int memberId);
    }
}
