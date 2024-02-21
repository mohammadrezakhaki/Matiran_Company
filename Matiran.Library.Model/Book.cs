using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Matiran.Library.Model
{
    public class Book
    {
        public string Title { get; set; }
        public string? Publisher { get; set; }
        public string? ISBN { get; set; }
        public int Count { get; set; }

    }
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Publisher { get; set; }
        public string? ISBN { get; set; }
        public int Count { get; set; }

    }
}
