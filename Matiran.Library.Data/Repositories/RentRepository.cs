using Dapper;
using Matiran.Library.Data.Contracts;
using Matiran.Library.Model;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Matiran.Library.Data.Repositories
{
    public class RentRepository: IRentRepository
    {
        private readonly IConfiguration _configuration;

        public RentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<RentviewModel>> SearchBooksRent(string BookTitle)
        {

            if (string.IsNullOrWhiteSpace(BookTitle))
            {
                throw new ArgumentException("عنوان کتاب نمی‌تواند خالی باشد.");
            }

            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                bool bookExists = dbConnection.ExecuteScalar<bool>("SELECT COUNT(1) FROM Books WHERE Title LIKE @Keyword", new { Keyword = $"%{BookTitle}%" });

                if (!bookExists)
                {
                    throw new ArgumentException($"هیچ کتابی با این عنوان  در پایگاه داده یافت نشد: {BookTitle}");
                }

                string query = @"SELECT Rents.*, Books.Title AS BookName, CONCAT(Members.FName, ' ', Members.LName) AS MemberName
                FROM Rents
                LEFT JOIN Books ON Rents.BookId = Books.Id
                LEFT JOIN Members ON Rents.MemberId = Members.Id
                WHERE Books.Title LIKE @Keyword";

                IEnumerable<RentviewModel> foundRentBooks = dbConnection.Query<RentviewModel>(query, new { Keyword = $"%{BookTitle}%" });

                if (foundRentBooks == null || !foundRentBooks.Any())
                {
                    throw new ArgumentException("هیچ کتابی با این عنوان که به امانت گرفته شده باشد یافت نشد.");
                }

                return foundRentBooks;
            }
        }

        public async Task<IEnumerable<RentviewModel>> SearchMembersRent(string keyword)
        {

            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("نام یا نام خانوادگی عضو نمی‌تواند خالی باشد.");
            }

            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                bool memberExists = dbConnection.ExecuteScalar<bool>("SELECT COUNT(1) FROM Members WHERE FName LIKE @Keyword OR LName LIKE @Keyword", new { Keyword = $"%{keyword}%" });

                if (!memberExists)
                {
                    throw new ArgumentException($"هیچ عضوی با این نام یا نام خانوادگی در پایگاه داده یافت نشد: {keyword}");
                }

                string query = @"SELECT Rents.*, CONCAT(Members.FName, ' ', Members.LName) AS MemberName, Books.Title AS BookName
                FROM Rents
                LEFT JOIN Members ON Rents.MemberId = Members.Id
                LEFT JOIN Books ON Rents.BookId = Books.Id
                WHERE Members.FName LIKE @Keyword OR Members.LName LIKE @Keyword";

                IEnumerable<RentviewModel> foundMembersRent = dbConnection.Query<RentviewModel>(query, new { Keyword = $"%{keyword}%" });

                if (foundMembersRent == null || !foundMembersRent.Any())
                {
                    throw new ArgumentException($"هیچ عضوی با این نام یا نام خانوادگی که کتابی به امانت گرفته شده یافت نشد: {keyword}");
                }

                return foundMembersRent;
            }
        }
        
        public async Task<RentviewModel> AddRent(Rent rent)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                // ثبت امانت کتاب
                dbConnection.Execute("INSERT INTO Rents (MemberId, BookId, FromDate, ReturnDate, IsReturned) VALUES (@MemberId, @BookId, GETDATE(), NULL, 0)", new { MemberId = rent.MemberId, BookId = rent.BookId });

                // کاهش موجودی کتاب در جدول Books
                dbConnection.Execute("UPDATE Books SET Count = Count - 1 WHERE Id = @Id", new { Id = rent.BookId });


                var rentViewModel = dbConnection.QueryFirstOrDefault<RentviewModel>(
                    "SELECT Rents.*, Books.Title AS BookName, Members.FName, Members.LName " +
                    "FROM Rents " +
                    "LEFT JOIN Books ON Rents.BookId = Books.Id " +
                    "LEFT JOIN Members ON Rents.MemberId = Members.Id " +
                    "WHERE Rents.BookId = @BookId AND Rents.MemberId = @MemberId",
                    new { BookId = rent.BookId, MemberId = rent.MemberId });

                return rentViewModel;
            }
        }

        public async Task<RentviewModel> ReturnBook(ReturnBookRequest returnBookRequest)
        {

            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                // ثبت بازگرداندن کتاب
                dbConnection.Execute("UPDATE Rents SET ReturnDate = GETDATE(), IsReturned = 1 WHERE MemberId = @MemberId AND BookId = @BookId AND IsReturned = 0",
                                     new { MemberId = returnBookRequest.MemberId, BookId = returnBookRequest.BookId });

                // افزایش موجودی کتاب در جدول Books
                dbConnection.Execute("UPDATE Books SET Count = Count + 1 WHERE Id = @Id", new { Id = returnBookRequest.BookId });

                // بازگشت اطلاعات جدید برای نمایش
                string query = @"SELECT Rents.*, Books.Title AS BookName, CONCAT(Members.FName, ' ', Members.LName) AS MemberName
                         FROM Rents
                         LEFT JOIN Books ON Rents.BookId = Books.Id
                         LEFT JOIN Members ON Rents.MemberId = Members.Id
                         WHERE Rents.MemberId = @MemberId AND Rents.BookId = @BookId AND Rents.IsReturned = 1";

                return dbConnection.QueryFirstOrDefault<RentviewModel>(query,new { MemberId = returnBookRequest.MemberId, BookId = returnBookRequest.BookId });
            }
        }

        public async Task<IEnumerable<RentviewModel>> LateReturn()
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();


                int rentCount = dbConnection.ExecuteScalar<int>("SELECT COUNT(1) FROM Rents WHERE IsReturned = 0");

                if (rentCount == 0)
                {
                    throw new ArgumentException("کتابی به امانت در اختیار هیچ عضوی نیست که بازگردانده نشده باشد");
                }

                var borrowedBooks = dbConnection.Query<RentviewModel>(
                    "SELECT R.Id, R.BookId, R.MemberId, R.FromDate, R.ReturnDate, R.IsReturned, B.Title AS BookName, CONCAT(M.FName, ' ', M.LName) AS MemberName " +
                    "FROM Rents R " +
                    "INNER JOIN Members M ON R.MemberId = M.Id " +
                    "INNER JOIN Books B ON R.BookId = B.Id " +
                    "WHERE R.IsReturned = 0");

                return borrowedBooks;
            }
        }

        public async Task<IEnumerable<RentviewModel>> GetAllRents()
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                int rentCount = dbConnection.ExecuteScalar<int>("SELECT COUNT(1) FROM Rents");

                if (rentCount == 0)
                {
                    throw new InvalidOperationException("هیچ کتابی به امانت از کتابخانه خارج نشده است");
                }

                return dbConnection.Query<RentviewModel>("SELECT R.Id, R.BookId, R.MemberId, R.FromDate, R.ReturnDate, R.IsReturned, B.Title AS BookName, CONCAT(M.FName, ' ', M.LName) AS MemberName " +
                    "FROM Rents R " +
                    "INNER JOIN Members M ON R.MemberId = M.Id " +
                    "INNER JOIN Books B ON R.BookId = B.Id " );
            }
        }

        public async Task<int> GetBorrowedBooksCount(int memberId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                var borrowedBooksCount = await dbConnection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(*) FROM Rents WHERE MemberId = @MemberId AND IsReturned = 0",
                    new { MemberId = memberId });

                return borrowedBooksCount;
            }
        }
     
        public async Task<RentviewModel> GetRentDetails(int memberId, int bookId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                string query = @"SELECT Rents.*, Books.Title AS BookName, CONCAT(Members.FName, ' ', Members.LName) AS MemberName
                        FROM Rents
                        LEFT JOIN Books ON Rents.BookId = Books.Id
                        LEFT JOIN Members ON Rents.MemberId = Members.Id
                        WHERE Rents.MemberId = @MemberId AND Rents.BookId = @BookId";

                return await dbConnection.QueryFirstOrDefaultAsync<RentviewModel>(query, new { MemberId = memberId, BookId = bookId });

            }
        }
      
        public async Task<bool> IsBookBorrowed(int memberId, int bookId)
        {
            using (IDbConnection dbConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                dbConnection.Open();

                // چک کردن آیا کتاب به امانت گرفته شده است
                string query = "SELECT TOP (1) * FROM Rents WHERE MemberId = @MemberId AND BookId = @BookId AND IsReturned = 0";

                bool isBookBorrowed = await dbConnection.ExecuteScalarAsync<bool>(query, new { MemberId = memberId, BookId = bookId });

                return isBookBorrowed;
            }
        }
    }
}
