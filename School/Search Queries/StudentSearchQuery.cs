using System.ComponentModel.DataAnnotations;

namespace School.Search_Queries
{
    public class StudentSearchQuery
    {
        [Required]
        public string Name { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public int Age { get; set; }
    }
}
