using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;

namespace Matiran.Library.Data.Repositories
{
    public class RentService : IRentService
    {
        IRentRepository _rentRepository;
        private readonly IRentValidator _rentValidator;

        public RentService(IRentRepository rentRepository,IRentValidator rentValidator)
        {
            _rentRepository = rentRepository;
            _rentValidator = rentValidator;
        }

        public async Task<IEnumerable<RentviewModel>> SearchBooksRent(string BookTitle)
        {
            if (string.IsNullOrWhiteSpace(BookTitle))
            {
                throw new ArgumentException("عنوان کتاب نمی‌تواند خالی باشد.");
            }

            return await _rentRepository.SearchBooksRent(BookTitle);
        }

        public async Task<IEnumerable<RentviewModel>> SearchMemebersRent(string MemberTitle)
        {
            if (string.IsNullOrWhiteSpace(MemberTitle))
            {
                throw new ArgumentException("نام یا نام خانوادگی عضو نمی‌تواند خالی باشد.");
            }

            return await _rentRepository.SearchMembersRent(MemberTitle);
        }

        public async Task<RentviewModel> AddRent(Rent rent)
        {
            await _rentValidator.ValidateRent(rent);

            return await _rentRepository.AddRent(rent);
        }

        public async Task<RentviewModel> ReturnBook(ReturnBookRequest returnBookRequest)
        {
            await _rentValidator.ValidateReturnRent(returnBookRequest);

            var success = await _rentRepository.ReturnBook(returnBookRequest);

            if (success == null)
            {
                throw new InvalidOperationException("خطا در ثبت بازگشت کتاب.");
            }

            return await _rentRepository.GetRentDetails(returnBookRequest.MemberId, returnBookRequest.BookId);
        }

        public async Task<IEnumerable<RentviewModel>> LateReturn()
        {
            return await _rentRepository.LateReturn();
        }

        public async Task<IEnumerable<RentviewModel>> GetAllRents()
        {
            return await _rentRepository.GetAllRents();
        }

        public async Task<RentviewModel> GetRentDetails(int memberId, int bookId)
        {
            return await _rentRepository.GetRentDetails(memberId, bookId);
        }

        public async Task<bool> IsBookBorrowed(int memberId, int bookId)
        {
            return await _rentRepository.IsBookBorrowed(memberId, bookId);
        }

        public async Task<int> GetBorrowedBooksCount(int memberId)
        {
            return await _rentRepository.GetBorrowedBooksCount(memberId);
        }
    }
}
