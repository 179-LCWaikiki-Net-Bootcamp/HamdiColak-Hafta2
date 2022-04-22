using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Entities
{
    // Princibal Entity
    public class Address
    {
        // Principal/Primary Key
        // if you name a variable 'Id' or '<ClassNane>Id' (for this class 'AddressId') it becomes a Primary Key for the entity.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? State { get; set; }
        public string Country { get; set; }
        [RegularExpression(@"([0-9]+)",
          ErrorMessage = "Characters are not allowed.")]
        public int? ZipCode { get; set; }

        public int StudentId { get; set; }
        // Collection Navigation Property
        public Student Student { get; set; }
    }
}
