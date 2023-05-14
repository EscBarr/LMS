using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.EntityСontext;

namespace LMS.EntityContext
{
    public class LabWorksRepo
    {
        private readonly ApplicationContext _db;

        public LabWorksRepo(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<IEnumerable<LaboratoryWork>> GetAll()
        {
            return await _db.LaboratoryWorks.ToListAsync();
        }

        public async Task<LaboratoryWork> GetById(int? LabID)
        {
            return await _db.LaboratoryWorks.Include(L => L.Variants).FirstOrDefaultAsync(m => m.Id == LabID);
        }

        public async Task<IEnumerable<LaboratoryWork>> GetAllByCourseId(int? CourseID)
        {
            return await _db.LaboratoryWorks.Where(lw => lw.CourseId == CourseID).Include(L => L.Variants).ToListAsync();
        }

        public void Create(LaboratoryWork laboratory)
        {
            _db.LaboratoryWorks.Add(laboratory);
        }

        public void Update(LaboratoryWork laboratory)
        {
            _db.LaboratoryWorks.Update(laboratory);
        }

        public async Task Delete(int? LabID)
        {
            LaboratoryWork laboratory = await _db.LaboratoryWorks.FindAsync(LabID);
            _db.LaboratoryWorks.Remove(laboratory);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}