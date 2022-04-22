using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace School.Entities
{
    // Dependent Entity
    public class Student
    {
        // Principal/Primary Key
        // if you name a variable 'Id' or '<ClassNane>Id' (for this class 'StudentId') it becomes a Primary Key for the entity.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 30, MinimumLength = 5,
            ErrorMessage = "The property {0} should have {1} maximum characters and {2} minimum characters")]
        public string? Name { get; set; }
        public string? LastName { get; set; }
        [RegularExpression(@"([0-9]+)",
          ErrorMessage = "Characters are not allowed.")]
        public int Age { get; set; }
        public string? FatherName { get; set; }
        public bool IsAdult { get; set; }

        // Foreign key
        public int GradeId { get; set; }
        // Simple/Reference Navigation Property
        public Grade Grade { get; set; }
        public Address Adress { get; set; }

    }
}
