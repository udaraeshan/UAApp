namespace UAApp.Shared.Exceptions
{
    public class CustomException : Exception
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; }
        public CustomException()
            : base()
        {
            ErrorMessage = string.Empty; ;
        }
        public CustomException(string message)
            : base(message)
        {
            ErrorMessage = message;
        }
        public CustomException(string message, int statusCode)
           : base(message)
        {
            ErrorMessage = message;
            StatusCode = statusCode;
        }
        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorMessage = message;
        }
        public CustomException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
            ErrorMessage = string.Empty; ;
        }
    }
}
