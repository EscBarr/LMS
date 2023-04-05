using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.EntityСontext;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;

namespace LMS.EntityContext
{
    public class CourseRepo
    {
        private readonly ApplicationContext _db;
        //private readonly UsersRepo _users;
        //private readonly LabsRepo _labs;

        public CourseRepo(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<List<Course>> GetAll(int userId)
        {
            //var Test = await _db.Courses.Include(course => (course.Users)).ToListAsync();
            //return Test.FindAll(course => course.UserId == userId);

            return await _db.Courses.Where(course => course.UserId == userId).Include(course => (course.Users)).ThenInclude(course => (course.LaboratoryWorks)).ToListAsync();
        }

        public async Task<List<Course>> GetAllWhereUser(int userId)//Получить все курсы, в которых состоит пользователь
        {
            return await _db.Courses.Where(course => course.Users.Any(us => us.Id == userId)).ToListAsync();
        }

        public async Task<Course> GetById(int? ID)
        {
            return await _db.Courses.FirstOrDefaultAsync(m => m.CourseId == ID);
        }

        public void Create(Course course)
        {
            _db.Courses.Add(course);
        }

        public void Update(Course course)
        {
            _db.Courses.Update(course);
        }

        public async Task AddUsers(int ID, List<int> UserID)
        {
            Course course = await _db.Courses.FindAsync(ID);
            var Users = _db.Users.Where(T => UserID.Contains(T.Id)).ToList();
            course.Users.Union(Users);
        }

        public async Task AddLab(int ID, int LabworkID)
        {
            Course course = await _db.Courses.FindAsync(ID);
            LaboratoryWork laboratory = await _db.LaboratoryWorks.FirstOrDefaultAsync(L => L.LaboratoryWorkId == LabworkID);
            course.LaboratoryWorks.Add(laboratory);
            _db.Courses.Remove(course);
        }

        public async Task Delete(int ID)
        {
            Course course = await _db.Courses.FindAsync(ID);
            _db.Courses.Remove(course);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}