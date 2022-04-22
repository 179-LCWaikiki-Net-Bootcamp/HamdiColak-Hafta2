using System.ComponentModel.DataAnnotations;

namespace School.Search_Queries
{
    public class AddressSearchQuery
    {
        public string? State { get; set; }
        [Required]
        public string Country { get; set; }
        public int ZipCode { get; set; }
    }
}
