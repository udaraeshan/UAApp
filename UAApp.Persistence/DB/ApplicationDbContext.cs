using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAApp.Domain.Common;
using UAApp.Domain.Entities;

namespace UAApp.Persistence.DB
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private IDbContextTransaction? _currentTransaction;
        private readonly IConfiguration _configuration;
        //public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options, ICurrentUserService currentUserService, IConfiguration configuration) : base(options)
        {
            _currentUserService = currentUserService;
            _configuration = configuration;
        }
        public string CurrentUserId { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ApplicationUser>().ToTable(nameof(this.ApplicationUsers));
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in ChangeTracker.Entries<CommonEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.CreatedByName = _currentUserService.Name;
                        entry.Entity.CreatedEmail = _currentUserService.Email;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = _currentUserService.UserId;
                        entry.Entity.UpdatedDate = DateTime.UtcNow;
                        entry.Entity.UpdatedByName = _currentUserService.Name;
                        entry.Entity.UpdatedByEmail = _currentUserService.Email;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }
            _currentTransaction = await base.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
        }
        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);

                _currentTransaction?.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
