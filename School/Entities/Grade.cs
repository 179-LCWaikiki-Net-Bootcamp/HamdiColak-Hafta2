using System.ComponentModel.DataAnnotations.Schema;

namespace School.Entities
{
    // Princibal Entity
    public class Grade
    {
        // Principal/Primary Key
        // if you name a variable 'Id' or '<ClassNane>Id' (for this class 'GradeId') it becomes a Primary Key for the entity.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Letter { get; set; }

        // Collection Navigation Property
        // This definition means 'Grade' can repeat more than once in 'Student'
        // 'to many' part of '1 to many' relationship
        public ICollection<Student> Students { get; set; }
    }
}
