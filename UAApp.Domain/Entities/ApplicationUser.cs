using UAApp.Domain.Common;

namespace UAApp.Domain.Entities
{
    public class ApplicationUser : CommonEntity
    {
        public virtual required string UserEmail { get; set; }
        public virtual required string FullName { get; set; }
        public virtual required string PhoneNumber { get; set; }
        public virtual required string Password { get; set; }


    }
}
