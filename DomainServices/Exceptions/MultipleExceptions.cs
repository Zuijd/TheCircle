namespace DomainServices.Exceptions
{
    public class MultipleExceptions : Exception
    {
        public List<Exception> InnerExceptions { get; }

        public MultipleExceptions(List<Exception> innerExceptions)
        {
            InnerExceptions = innerExceptions;
        }
    }
}
