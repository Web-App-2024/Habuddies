namespace HaBuddies.Middleware
{
    public class UserIdentityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserIdentityMiddleware> _logger;

        public UserIdentityMiddleware(RequestDelegate next, ILogger<UserIdentityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!new string[] { "/api/user/login", "/api/user/register" }.Any(route => context.Request.Path.StartsWithSegments(route)))
            {
                string userId = context.Session.GetString("userId");
                if (userId == null)
                {
                    // Log the error
                    _logger.LogWarning("Unauthorized access attempt for {path}", context.Request.Path);
                    throw new UnauthorizedAccessException();
                }

                context.Items["userId"] = userId;
            }

            await _next(context);
        }
    }
}