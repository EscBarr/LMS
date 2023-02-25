using LMS.EntityСontext;

namespace LMS.DTO
{
    public class LaboratoryWorkDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }
}