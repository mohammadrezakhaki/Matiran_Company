using Dapper;
using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Matiran.Library.Data.Repositories
{
    public class BookRepository : IBookRepository
    {

        private readonly IConfiguration _configuration;

        public BookRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<BookViewModel>> SearchBooks(string book)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                IEnumerable<BookViewModel> foundBooks = await dbConnection.QueryAsync<BookViewModel>("SELECT * FROM Books WHERE Title LIKE @Keyword", new { Keyword = $"%{book}%" });

                return foundBooks;

            }
        }

        public async Task<BookViewModel> AddBook(Book book)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                bool bookExists = await dbConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM Books WHERE Title = @Title", new { Title = book.Title });

                if (bookExists)
                {
                    throw new InvalidOperationException($"کتاب با نام '{book.Title}' در دیتابیس وجود دارد.");
                }

                string insertQuery = "INSERT INTO Books (Title, Publisher, ISBN, Count) OUTPUT INSERTED.Id VALUES (@Title, @Publisher, @ISBN, @Count)";

                int insertedBookId = await dbConnection.ExecuteScalarAsync<int>(insertQuery, book);

                BookViewModel insertedBookViewModel =  GetBookById(insertedBookId);

                return insertedBookViewModel;
            }
        }

        public async Task<bool> RemoveBook(int bookId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();
                bool bookExists = await dbConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM Books WHERE Id = @Id", new { Id = bookId });
                if (!bookExists)
                {
                    throw new InvalidOperationException($"کتابی با شناسه '{bookId.ToString()}' در دیتابیس وجود ندارد.");
                }

                // چک کردن وجود کتاب در جدول Rents
                bool isBookBorrowed = await dbConnection.ExecuteScalarAsync<bool>("SELECT TOP 1 *  FROM Rents WHERE BookId = @BookId AND IsReturned = 0",
                                                                                         new { BookId = bookId });

                if (isBookBorrowed)
                {
                    throw new InvalidOperationException($"کتاب با شناسه '{bookId}' به امانت گرفته شده است و نمی‌توانید آن را حذف کنید.");
                }

                await dbConnection.ExecuteAsync("DELETE FROM Books WHERE Id = @BookId", new { BookId = bookId });

                return true;
            }
        }

        public async Task<IEnumerable<BookViewModel>> GetAllBooks()
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                int bookCount = await dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Books");

                if (bookCount == 0)
                {
                    throw new InvalidOperationException("کتابی در پایگاه داده وجود ندارد");
                }

                return await dbConnection.QueryAsync<BookViewModel>("SELECT * FROM Books");
            }
        }

        public BookViewModel GetBookById(int bookId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                BookViewModel bookViewModel =  dbConnection.QueryFirstOrDefault<BookViewModel>(
                    "SELECT Id, Title, Publisher, ISBN, Count FROM Books WHERE Id = @Id",
                    new { Id = bookId });

                if (bookViewModel == null)
                {
                    throw new InvalidOperationException($"کتاب با شناسه '{bookId}' در دیتابیس یافت نشد.");
                }

                return bookViewModel;
            }
        }


    }
}
