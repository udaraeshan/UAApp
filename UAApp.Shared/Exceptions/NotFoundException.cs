namespace UAApp.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; }
        public NotFoundException()
            : base()
        {
            ErrorMessage = string.Empty; ;
        }
        public NotFoundException(string message)
            : base(message)
        {
            ErrorMessage = message;
        }
        public NotFoundException(string message, int statusCode)
           : base(message)
        {
            ErrorMessage = message;
            StatusCode = statusCode;
        }
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorMessage = message;
        }
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
            ErrorMessage = string.Empty; ;
        }
    }
}
