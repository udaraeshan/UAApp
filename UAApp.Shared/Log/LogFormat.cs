namespace UAApp.Shared.Log
{
    public class LogFormat
    {
        public string Method { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public int StatusCode { get; set; } = 0;
        public string Severity { get; set; }
        public string Message { get; set; }
        public object Description { get; set; }
        public string UserID { get; set; }

    }
}
