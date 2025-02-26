namespace UAApp.Application.AppUserService.Dto
{
    public class AuthRequestDto
    {
        public required string Email { get; set; }
        public string? UserPassword { get; set; }
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public string? AccessLevel { get; set; }
        public required string AuthType { get; set; }
        public string? o365AccessToken { get; set; }
    }
}
