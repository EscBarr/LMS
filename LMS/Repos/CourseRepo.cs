using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.EntityСontext;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using LMS.DTO;

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
            return await _db.Courses.Where(course => course.UserId == userId).Include(course => (course.LaboratoryWorks)).Include(course => (course.Users)).ToListAsync();
        }

        public async Task<List<Course>> GetAllWhereUser(int userId)//Получить все курсы, в которых состоит пользователь
        {
            return await _db.Courses.Where(course => course.Users.Any(us => us.Id == userId)).ToListAsync();
        }

        public async Task<Course> GetById(int? ID)
        {
            return await _db.Courses.Include(course => course.LaboratoryWorks).Include(course => (course.Users)).FirstOrDefaultAsync(m => m.CourseId == ID);
        }

        public void Create(Course course)
        {
            _db.Courses.Add(course);
        }

        public void Update(Course course)
        {
            _db.Courses.Update(course);
        }

        public async Task AddUsers(int ID, int[] UserID)
        {
            Course course = await GetById(ID);
            var Users = await _db.Users.Where(T => UserID.Contains(T.Id)).ToListAsync();
            course.Users = course.Users.Union(Users).ToList();
            Update(course);
        }

        public async Task DeleteUsers(int ID, int[] UserID)
        {
            //Course course = await _db.Courses.FindAsync(ID);
            Course course = await GetById(ID);
            var Users = await _db.Users.Where(T => UserID.Contains(T.Id)).ToListAsync();
            foreach (var user in Users)
            {
                course.Users.Remove(user);
            }
            Update(course);
        }

        public async Task DeleteUser(int ID, int UserID)
        {
            //Course course = await _db.Courses.FindAsync(ID);
            Course course = await GetById(ID);
            var User = course.Users.FirstOrDefault((T => T.Id == UserID));
            if (User != null)
            {
                course.Users.Remove(User);

                Update(course);
            }
        }

        public async Task AddLab(int ID, LaboratoryWorkDTO Labwork)
        {
            Course course = await GetById(ID);
            LaboratoryWork laboratory = new LaboratoryWork { Name = Labwork.Name, Description = Labwork.Description, CourseId = Labwork.CourseId, UserId = Labwork.UserId, MaxMark = Labwork.MaxMark };
            course.LaboratoryWorks.Add(laboratory);
            Update(course);
        }

        public async Task DeleteLab(int ID, int LabworkID)
        {
            Course course = await GetById(ID);
            LaboratoryWork laboratory = await _db.LaboratoryWorks.FirstOrDefaultAsync(L => L.Id == LabworkID);
            course.LaboratoryWorks.Remove(laboratory);
            Update(course);
        }

        public async Task Delete(int ID)
        {
            Course course = await GetById(ID);
            _db.Courses.Remove(course);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}