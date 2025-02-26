namespace UAApp.Domain.Common
{
    public interface ICurrentUserService
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserId { get; set; }
        public string AccessLevel { get; set; }
    }
}
