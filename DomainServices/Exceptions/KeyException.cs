namespace DomainServices.Exceptions
{
    public class KeyException : Exception
    {
        public string Key { get; }
        public string Message { get; }

        public KeyException(string key, string message)
        {
            Key = key;
            Message = message;
        }
    }
}
