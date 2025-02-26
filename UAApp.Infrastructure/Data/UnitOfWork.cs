using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAApp.Domain.Common;
using UAApp.Persistence.DB;

namespace UAApp.Infrastructure.Data
{
    public class UnitOfWork(ApplicationDbContext context, IHostingEnvironment env, IHttpContextAccessor httpcontext, IOptions<ApplicationSetting> applicationSetting, ICurrentUserService currentUserService, ILogger<UnitOfWork> logger) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly IOptions<ApplicationSetting> _appSettings = applicationSetting;
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
