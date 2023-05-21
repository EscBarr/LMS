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

        public async Task<List<User>> GetAllUsersFromCourse(int CourseId)//Получить всех пользователей из курса и назначенные им варианты
        {
            return await _db.Courses.Where(course => course.CourseId == CourseId).SelectMany(c => c.Users).Include(c => c.AssignedVariants).ToListAsync();
        }

        public async Task<List<User>> GetAllUsersFromCoursePerLab(int CourseId, int LabId)//Получить всех пользователей из курса и назначенные им варианты в конкретной лабораторной работе
        {
            return await _db.Courses.Where(course => course.CourseId == CourseId).SelectMany(c => c.Users).Include(c => c.AssignedVariants.Where(a => a.Variant.LaboratoryWorkId == LabId)).ToListAsync();
        }

        public async Task<Course> GetById(int? ID)
        {
            return await _db.Courses.Include(course => course.LaboratoryWorks).Include(course => (course.Users)).FirstOrDefaultAsync(m => m.CourseId == ID);
        }

        public async Task Create(Course course)
        {
            await _db.Courses.AddAsync(course);
        }

        public async Task Update(Course course)
        {
            _db.Courses.Update(course);
        }

        public async Task AddUsers(int ID, int[] UserID)
        {
            Course course = await GetById(ID);
            var Users = await _db.Users.Where(T => UserID.Contains(T.Id)).ToListAsync();
            course.Users = course.Users.Union(Users).ToList();
            await Update(course);
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
            await Update(course);
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