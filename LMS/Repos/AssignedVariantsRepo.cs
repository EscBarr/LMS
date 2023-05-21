using LMS.DTO;
using LMS.EntityСontext;
using Microsoft.EntityFrameworkCore;

namespace LMS.EntityContext
{
    public class AssignedVariantsRepo
    {
        private readonly ApplicationContext _db;
        //private readonly UsersRepo _users;
        //private readonly LabsRepo _labs;

        public AssignedVariantsRepo(ApplicationContext context)
        {
            _db = context;
        }

        public async Task<List<AssignedVariant>> GetAllWhereUser(int userId, int CourseID)
        {
            return await _db.AssignedVariants.Include(var => var.Variant).ThenInclude(lab => lab.LaboratoryWork).Where(var => var.UserId == userId && var.Variant.LaboratoryWork.CourseId == CourseID).ToListAsync();
        }

        public async Task<List<AssignedVariant>> GetAllWhereUserByLab(int userId, int CourseID, int LabId)
        {
            return await _db.AssignedVariants.Include(var => var.Variant).ThenInclude(lab => lab.LaboratoryWork).Where(var => var.UserId == userId && var.Variant.LaboratoryWork.CourseId == CourseID && var.Variant.LaboratoryWork.Id == LabId).ToListAsync();
        }

        public async Task<List<AssignedVariant>> GetAllWhereByLab(int CourseID, int LabId)
        {
            return await _db.AssignedVariants.Include(var => var.Variant).ThenInclude(lab => lab.LaboratoryWork).Where(var => var.Variant.LaboratoryWork.CourseId == CourseID && var.Variant.LaboratoryWork.Id == LabId).ToListAsync();
        }

        public async Task<AssignedVariant> GetById(int Id)
        {
            return await _db.AssignedVariants.Include(var => var.Variant).ThenInclude(lab => lab.LaboratoryWork).FirstOrDefaultAsync(var => var.AssignedVariantId == Id);
        }

        public async Task Create(AssignedVariant var)
        {
            await _db.AssignedVariants.AddAsync(var);
        }

        public async Task Update(AssignedVariant course)
        {
            _db.AssignedVariants.Update(course);
        }

        public async Task Delete(int ID)
        {
            AssignedVariant var = await GetById(ID);
            _db.AssignedVariants.Remove(var);
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}