using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matiran.Library.Model
{
    public class Rent
    {

        [Required(ErrorMessage = "شناسه کتاب اجباری است")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "شناسه اعضا اجباری است")]
        public int MemberId { get; set; }
 
    }
    public class RentviewModel
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public string BookName { get; set; }
        public string MemberName { get; set; }

        public string ReturnStatus => IsReturned ? "بازگردانده شده اشت" : "هنوز به کتابخانه بازگردانده نشده است";

    }
    public class ReturnBookRequest
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }
    }

}


