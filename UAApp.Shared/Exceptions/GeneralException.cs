namespace UAApp.Shared.Exceptions
{
    public class GeneralException : Exception
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; }
        public GeneralException()
            : base()
        {
            ErrorMessage = string.Empty; ;
        }
        public GeneralException(string message)
            : base(message)
        {
            ErrorMessage = message;
        }
        public GeneralException(string message, int statusCode)
           : base(message)
        {
            ErrorMessage = message;
            StatusCode = statusCode;
        }
        public GeneralException(string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorMessage = message;
        }
        public GeneralException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
            ErrorMessage = string.Empty; ;
        }
    }
}
