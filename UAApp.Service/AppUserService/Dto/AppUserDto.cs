using System;


namespace UAApp.Application.AppUserService.Dto
{
    public class AppUserDto
    {
        public required int Id { get; set; }
        public required string UserEmail { get; set; }
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public bool IsActive { get; set; }
    }
}
