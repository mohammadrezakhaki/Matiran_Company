using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;

namespace Matiran.Library.Data.Repositories
{
    public class RentValidator : IRentValidator
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IRentRepository _rentRepository;

        public RentValidator(IBookRepository bookRepository, IMemberRepository memberRepository, IRentRepository rentRepository)
        {
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
            _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
            _rentRepository = rentRepository ?? throw new ArgumentNullException(nameof(rentRepository));
        }

        public async Task ValidateRent(Rent rent)
        {
            if (rent == null)
            {
                throw new ArgumentNullException(nameof(rent), "اطلاعات امانت نمی‌تواند خالی باشد.");
            }

            // چک کردن موجودی کتاب و وجود کتاب
            var bookInfo =  _bookRepository.GetBookById(rent.BookId);

            if (bookInfo == null)
            {
                throw new InvalidOperationException($"کتاب با شناسه '{rent.BookId}' در دیتابیس موجود نیست.");
            }

            int currentCount = bookInfo.Count;

            if (currentCount <= 0)
            {
                throw new InvalidOperationException($"موجودی کتاب با شناسه '{rent.BookId}' صفر است.");
            }

            // چک کردن وجود عضو
            var memberExists = await _memberRepository.GetMemberById(rent.MemberId);

            if (memberExists == null)
            {
                throw new InvalidOperationException($"عضو با شناسه '{rent.MemberId}' در دیتابیس موجود نیست.");
            }

            // چک کردن تعداد کتاب‌های به امانت گرفته شده توسط هر فرد
            int borrowedBooksCount = await _rentRepository.GetBorrowedBooksCount(rent.MemberId);

            if (borrowedBooksCount >= 3)
            {
                throw new InvalidOperationException("هر فرد نمی‌تواند بیش از 3 کتاب به امانت بگیرد.");
            }
        }

        public async Task ValidateReturnRent(ReturnBookRequest returnBookRequest)
        {
            if (returnBookRequest == null)
            {
                throw new ArgumentNullException(nameof(returnBookRequest), "اطلاعات بازگشت کتاب نمی‌تواند خالی باشد.");
            }

            // چک کردن وجود عضو
            var memberExists = await _memberRepository.GetMemberById(returnBookRequest.MemberId);

            if (memberExists == null)
            {
                throw new InvalidOperationException($"عضو با شناسه '{returnBookRequest.MemberId}' در دیتابیس موجود نیست.");
            }

            // چک کردن وجود کتاب
            var bookInfo =  _bookRepository.GetBookById(returnBookRequest.BookId);

            if (bookInfo == null)
            {
                throw new InvalidOperationException($"کتاب با شناسه '{returnBookRequest.BookId}' در دیتابیس موجود نیست.");
            }

            // چک کردن آیا کتاب به امانت گرفته شده است
            bool isBookBorrowed = await _rentRepository.IsBookBorrowed(returnBookRequest.MemberId, returnBookRequest.BookId);

            if (!isBookBorrowed)
            {
                throw new InvalidOperationException($"کتاب با شناسه '{returnBookRequest.BookId}' به امانت گرفته نشده است یا قبلاً بازگردانده شده است.");
            }
        }
    }
}
