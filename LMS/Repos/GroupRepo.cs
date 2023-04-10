using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.EntityСontext;

namespace LMS.EntityContext
{
    public class GroupRepo
    {
        private readonly ApplicationContext _db;

        public GroupRepo(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<Group>> GetAll()
        {
            return await _db.Groups.ToListAsync();
        }

        public async Task<Group> GetById(int? GroupID)
        {
            return await _db.Groups.FirstOrDefaultAsync(m => m.GroupId == GroupID);
        }

        public void Create(Group group)
        {
            _db.Groups.Add(group);
        }

        public void Update(Group group)
        {
            _db.Update(group);
        }

        public async Task Delete(int typeID)
        {
            Group group = await _db.Groups.FindAsync(typeID);
            _db.Groups.Remove(group);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}