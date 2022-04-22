using System.Text.Json.Serialization;

namespace School.Entity_DTOs
{
    public class AddressDTO
    {
        public int Id { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? ZipCode { get; set; }
        public int StudentId { get; set; } = 0;

    }
}
