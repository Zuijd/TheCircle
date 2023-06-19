namespace Portal.Middleware
{
    public class TLSAccess : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var url = "";
            var currentUrl = $"{request.Host}{request.Path}".ToLower();

            if (currentUrl.Equals("localhost:5215/user/login") || currentUrl.Equals("localhost:5215/user/register"))
            {
                url = $"https://localhost:7194{request.Path}";
                context.Result = new RedirectResult(url);
            }
            else if (request.IsHttps && !(currentUrl.Equals("localhost:7194/user/login") || currentUrl.Equals("localhost:7194/user/register")))
            {
                url = $"http://localhost:5215{request.Path}";
                context.Result = new RedirectResult(url);
            }

            base.OnActionExecuting(context);
        }
    }
}