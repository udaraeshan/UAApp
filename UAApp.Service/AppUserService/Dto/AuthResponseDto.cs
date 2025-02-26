namespace UAApp.Application.AppUserService.Dto
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
        public required string UserID { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public List<string> ModuleAccess { get; set; } = new List<string>();

    }
}
