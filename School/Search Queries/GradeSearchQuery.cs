using System.ComponentModel.DataAnnotations;

namespace School.Search_Queries
{
    public class GradeSearchQuery
    {
        [Required]
        public string Name { get; set; }
        public string? Letter { get; set; }
    }
}
