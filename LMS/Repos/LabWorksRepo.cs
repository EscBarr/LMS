using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.EntityСontext;
using LMS.DTO;

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

        public async Task AddVariant(int? ID, Variant variant)
        {
            LaboratoryWork laboratory = await GetById(ID);
            laboratory.Variants.Add(variant);
            Update(laboratory);
        }

        public async Task UpdateVariant(int? ID, Variant variant)
        {
            _db.Variants.Update(variant);
        }

        public async Task DeleteVariant(int ID, int VariantID)
        {
            LaboratoryWork laboratory = await GetById(ID);
            Variant variant = await _db.Variants.FirstOrDefaultAsync(L => L.VariantId == VariantID);
            laboratory.Variants.Remove(variant);
            Update(laboratory);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}