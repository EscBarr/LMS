using LMS.DTO;
using LMS.EntityContext;
using LMS.EntityСontext;
using LMS.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;

//< a class= "btn btn-primary" asp - page = "AssignTasks" asp - route - Id = "@item.Id" >< i class= "fa-solid fa-pen-to-square" ></ i ></ a >

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class ManageCourseModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        private readonly UsersRepo _usersRepo;

        public List<SelectListItem> _Groups { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedGroupID { get; set; }

        public List<SelectListItem> _CurrentUsers { get; set; }

        public List<SelectListItem> _SelectedUsers { get; set; }

        [BindProperty]
        public int[] SelectedUsersIDs { get; set; }

        [BindProperty]
        public int[] SelectedUsersDeletion { get; set; }

        [BindProperty]
        public string CourseName { get; set; }//Изменение названия курса

        //public CourseDTO courseDTO { get; set; }//Изменение названия курса

        [BindProperty]
        public Course Cur_Course { get; set; }//Информация о курсе

        [BindProperty]
        public LaboratoryWorkDTO LabDTO { get; set; }//Информация о курсе

        public ManageCourseModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
            _usersRepo = usersRepo;
        }

        public async Task<IActionResult> OnGet()
        {
            var test = this.Id;
            HttpContext.Session.SetInt32("CourseId", Id);
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Cur_Course = await _courseRepo.GetById(Id);
            if (Cur_Course == null)
            {
                return NotFound("Курс не найден");
            }
            CourseName = Cur_Course.Name;
            _Groups = _db.Groups.Select(a => new SelectListItem { Value = a.GroupId.ToString(), Text = a.Name }).ToList();
            _CurrentUsers = Cur_Course.Users.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name + " " + a.Surname + " " + a.Patronymic }).ToList();
            Cur_Course.Users = Cur_Course.Users.OrderBy(x => x.Surname).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostEditName()
        {
            var id = (int)HttpContext.Session.GetInt32("CourseId");
            Cur_Course = await _courseRepo.GetById(id);
            Cur_Course.Name = CourseName;
            _courseRepo.Update(Cur_Course);
            await _courseRepo.Save();
            return RedirectToPage("Manage", id);
        }

        public async Task<JsonResult> OnGetGroupUsers()
        {
            var id = (int)HttpContext.Session.GetInt32("CourseId");
            Cur_Course = await _courseRepo.GetById(id);
            Cur_Course.Users = Cur_Course.Users.OrderBy(x => x.Surname).ToList();
            _CurrentUsers = Cur_Course.Users.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name + " " + a.Surname + " " + a.Patronymic }).ToList();
            var UserGroup = await _usersRepo.GetAllByGroup(SelectedGroupID);
            _SelectedUsers = new List<SelectListItem>();
            //TODO ПРОВЕРИТЬ РАБОТАЕТ ЛИ ВЫБОРКА, СПИСОК ГРУППЫ ВЫВОДИТСЯ БЕЗ УЧАСТНИКОВ КОТОРЫЕ УЖЕ ЕСТЬ В КУРСЕ
            foreach (var user in UserGroup)
            {
                var SelUser = new SelectListItem { Value = user.Id.ToString(), Text = user.Name + " " + user.Surname + " " + user.Patronymic };
                if (!_CurrentUsers.Exists(a => a.Value == SelUser.Value))
                {
                    _SelectedUsers.Add(SelUser);
                }
            }
            //_SelectedUsers = UserGroup.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name + " " + a.Surname + " " + a.Patronymic }).Where(a => !_CurrentUsers.Contains(a)).ToList();
            //TODO Здесь возвращается список группы для добавления
            return new JsonResult(_SelectedUsers);
        }

        public async Task<IActionResult> OnPostAddUsers()
        {
            var id = (int)HttpContext.Session.GetInt32("CourseId");
            if (SelectedUsersIDs.Length > 0)
            {
                await _courseRepo.AddUsers(id, SelectedUsersIDs);
                await _courseRepo.Save();
            }

            return RedirectToPage("Manage", id);
        }

        public async Task<IActionResult> OnPostDeleteUsers()
        {
            var id = (int)HttpContext.Session.GetInt32("CourseId");
            if (SelectedUsersIDs.Length > 0)
            {
                await _courseRepo.DeleteUsers(id, SelectedUsersIDs);
                await _courseRepo.Save();
            }

            return RedirectToPage("Manage", id);
        }

        public async Task<IActionResult> OnPostDeleteUser(int UserId)
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            await _courseRepo.DeleteUser(CourseId, UserId);
            await _courseRepo.Save();
            return RedirectToPage("Manage", CourseId);
        }

        public async Task<IActionResult> OnPostAddLab()
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            LabDTO.UserId = UserId;
            LabDTO.CourseId = CourseId;
            await _courseRepo.AddLab(CourseId, LabDTO);
            await _courseRepo.Save();
            return RedirectToPage("Manage", CourseId);
        }

        public async Task<IActionResult> OnPostDeleteLab(int LabId)
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            await _courseRepo.DeleteLab(CourseId, LabId);
            await _courseRepo.Save();
            return RedirectToPage("Manage", CourseId);
        }

        public async Task<IActionResult> OnPostDeleteCourse()
        {
            var CourseId = (int)HttpContext.Session.GetInt32("CourseId");
            await _courseRepo.Delete(CourseId);
            await _courseRepo.Save();
            return RedirectToPage("Index");
        }
    }
}