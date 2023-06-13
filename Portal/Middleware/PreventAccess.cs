namespace Portal.Middleware
{
    public class PreventAccess : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/");
            }

            base.OnActionExecuting(context);
        }
    }
}
