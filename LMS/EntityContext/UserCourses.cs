namespace LMS.EntityСontext
{
    public class UserCourses
    {
        public int UserCoursesId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}