using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matiran.Library.Model
{
    public class Member
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string? NID { get; set; }
        public string? Mobile { get; set; }
        public string FullName => $"{FName} {LName}";

    }
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string? NID { get; set; }
        public string? Mobile { get; set; }
        public string FullName => $"{FName} {LName}";

    }
}
