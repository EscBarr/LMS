using LMS.DTO;
using LMS.EntityContext;
using LMS.Entity�ontext;
using LMS.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher")]
    public class ManageCourseModel : PageModel
    {
        private readonly ApplicationContext _db;

        private readonly CourseRepo _courseRepo;

        private readonly LabWorksRepo _labWorksRepo;

        private readonly UsersRepo _usersRepo;

        public List<SelectListItem>? _Groups;

        public List<SelectListItem>? _CurrentUsers;

        public List<SelectListItem> _SelectedUsers;

        //public List<SelectListItem>? _AddedLabs;

        [BindProperty]
        public CourseDTO courseDTO { get; set; }//��������� �������� �����

        //public List<User>? CurUsers { get; set; }//��������� ���� �������������

        [BindProperty]
        public Course Cur_Course { get; set; }//���������� � �����

        public ManageCourseModel(ApplicationContext db, CourseRepo courseRepo, LabWorksRepo labWorksRepo, UsersRepo usersRepo)
        {
            _db = db;
            _courseRepo = courseRepo;
            _labWorksRepo = labWorksRepo;
            _usersRepo = usersRepo;
            Cur_Course = new Course();
        }

        public async Task<IActionResult> OnGet(int Id)
        {
            var UserId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Cur_Course = await _courseRepo.GetById(Id);
            if (Cur_Course == null)
            {
                return NotFound("���� �� ������");
            }
            _CurrentUsers = Cur_Course.Users.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name + " " + a.Surname + " " + a.Patronymic }).ToList();
            _Groups = _db.Groups.Select(a => new SelectListItem { Value = a.GroupId.ToString(), Text = a.Name }).ToList();
            _Groups[0].Selected = true;
            var UserGroup = await _usersRepo.GetAllByGroup(int.Parse(_Groups[0].Value));
            //TODO ��������� �������� �� �������, ������ ������ ��������� ��� ���������� ������� ��� ���� � �����
            _SelectedUsers = UserGroup.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name + " " + a.Surname + " " + a.Patronymic }).Where(a => !_CurrentUsers.Contains(a)).ToList();
            return Page();
        }

        public async void OnPostEdit(int CourseId)
        {
            var Course = await _courseRepo.GetById(CourseId);
            Course.Name = courseDTO.Name;
            _courseRepo.Update(Course);
            await _courseRepo.Save();

            //Course
        }

        public async Task OnPostGetUsers(int GroupId)
        {
            var UserGroup = await _usersRepo.GetAllByGroup(GroupId);
            //TODO ��������� �������� �� �������, ������ ������ ��������� ��� ���������� ������� ��� ���� � �����
            _SelectedUsers = UserGroup.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name + " " + a.Surname + " " + a.Patronymic }).Where(a => !_CurrentUsers.Contains(a)).ToList();
            //TODO ����� ������������ ������ ������ ��� ����������
            //return Partial()
        }

        public async void OnPostEdit(string Name)
        {
            Cur_Course.Name = Name;
            _courseRepo.Update(Cur_Course);
            await _courseRepo.Save();
        }

        public async Task OnPostAddUsers()
        {
            List<int> SelectedId = new List<int>();
            foreach (var item in _SelectedUsers)
            {
                if (item.Selected)
                {
                    SelectedId.Add(int.Parse(item.Value));
                }
            }
            await _courseRepo.AddUsers(Cur_Course.CourseId, SelectedId);
            await _courseRepo.Save();
        }

        public async Task OnPostDeleteUsers()
        {
            List<int> SelectedId = new List<int>();
            foreach (var item in _SelectedUsers)
            {
                if (item.Selected)
                {
                    SelectedId.Add(int.Parse(item.Value));
                }
            }
            await _courseRepo.DeleteUsers(Cur_Course.CourseId, SelectedId);
            await _courseRepo.Save();
        }

        public async Task OnPostDeleteUser(int UserId)
        {
            List<int> SelectedId = new List<int>();
            foreach (var item in _SelectedUsers)
            {
                if (item.Selected)
                {
                    SelectedId.Add(int.Parse(item.Value));
                }
            }
            await _courseRepo.DeleteUsers(Cur_Course.CourseId, SelectedId);
            await _courseRepo.Save();
        }

        public async Task OnPostAddLab()
        {
        }

        public async Task OnPostDeleteLab()
        {
        }
    }
}