using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookViewModel>> SearchBooks(string bookTitle);
        Task<BookViewModel> AddBook(Book book);
        Task<bool> RemoveBook(int bookId);
        Task<IEnumerable<BookViewModel>> GetAllBooks();
    }

}
