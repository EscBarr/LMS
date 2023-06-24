using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LMS.EntityСontext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LMS.Pages
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Teacher,Student")]
    public class RepositoryUserModel : PageModel
    {
        private ApplicationContext _context;
        public List<RepositoryEntity> Repos;

        public RepositoryUserModel(ApplicationContext Context)
        {
            _context = Context;
        }

        public async Task OnGetAsync()
        {
            Repos = await Task.Run(() => _context.Repos
                                           .Include(x => x.User)
                                           .Where(_ => true)
                                           .OrderBy(x => x.Id).ToList());
        }
    }
}