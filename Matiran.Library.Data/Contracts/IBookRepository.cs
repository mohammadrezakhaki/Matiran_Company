using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookViewModel>> SearchBooks(string BookTitle);
        Task<BookViewModel> AddBook(Book book);
        Task<bool> RemoveBook(int bookId);
        Task<IEnumerable<BookViewModel>> GetAllBooks();
        BookViewModel GetBookById(int bookId);
    }
}
