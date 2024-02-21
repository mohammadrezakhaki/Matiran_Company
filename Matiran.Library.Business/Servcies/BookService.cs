using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;

namespace Matiran.Library.Data.Repositories
{
    public class BookService : IBookService
    {
        IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookViewModel>> SearchBooks(string book)
        {
            if (string.IsNullOrWhiteSpace(book))
            {
                throw new ArgumentException("عنوان کتاب نمی‌تواند خالی باشد.");
            }

            IEnumerable<BookViewModel> foundBooks = await _bookRepository.SearchBooks(book);

            if (foundBooks == null || !foundBooks.Any())
            {
                throw new ArgumentException("هیچ کتابی با این عنوان یافت نشد.");
            }

            return foundBooks;
        }

        public async Task<BookViewModel> AddBook(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
            {
                throw new ArgumentException("عنوان کتاب نمی‌تواند خالی باشد");
            }

            if (book.Count == 0)
            {
                throw new ArgumentException("موجودی کتاب نمی‌تواند صفر باشد");
            }

            return await _bookRepository.AddBook(book);
        }

        public async Task<bool> RemoveBook(int bookId)
        {
            bool bookExists = await _bookRepository.RemoveBook(bookId);

            if (!bookExists)
            {
                throw new ArgumentException($"کتاب با شناسه '{bookId}' یافت نشد.");
            }

            return true;
        }

        public async Task<IEnumerable<BookViewModel>> GetAllBooks()
        {
            IEnumerable<BookViewModel> allBooks = await _bookRepository.GetAllBooks();

            if (allBooks == null || !allBooks.Any())
            {
                throw new ArgumentException("هیچ کتابی یافت نشد.");
            }

            return allBooks;
        }

    }
}
