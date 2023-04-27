using LMS.EntityСontext;
using Microsoft.EntityFrameworkCore;

namespace LMS.Repos
{
    public class UsersRepo
    {
        private readonly ApplicationContext _db;

        public UsersRepo(ApplicationContext context)
        {
            _db = context;
        }

        //TODO ИСПРАВИТЬ ВЫБОРКУ ВСЕ ПОЛЯ НЕ НУЖНЫ
        public async Task<IEnumerable<User>> GetAllByGroup(int groupId)
        {
            if (await _db.Groups.Include(group => group.Users).FirstOrDefaultAsync(group => group.GroupId == groupId) != null)
            {
                return await _db.Users.Where(user => user.GroupId == groupId).ToListAsync();
            }
            return null;
        }

        public async Task<User> GetById(int? UserID)
        {
            return await _db.Users.FirstOrDefaultAsync(user => user.Id == UserID);
        }

        public void Create(User User)
        {
            _db.Users.Add(User);
        }

        public void Update(User User)
        {
            _db.Users.Update(User);
        }

        public async Task Delete(int? UserID)
        {
            User user = await _db.Users.FindAsync(UserID);
            _db.Users.Remove(user);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}