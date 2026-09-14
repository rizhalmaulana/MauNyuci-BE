using Hangfire.Dashboard;

namespace MauNyuci.Api.Middleware
{
    public class HangfireAuthFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Sesuai permintaan User: Akses Hangfire Dashboard harus melalui proteksi berlapis
            // Untuk development/lokal, kita izinkan jika dari localhost
            if (httpContext.Connection.LocalIpAddress != null &&
                httpContext.Connection.RemoteIpAddress != null &&
                httpContext.Connection.LocalIpAddress.Equals(httpContext.Connection.RemoteIpAddress))
            {
                return true;
            }

            // Jika di-deploy ke server pubik, wajib memiliki Token/Autentikasi (Misal Admin Role)
            // Hangfire Dashboard tidak mendukung JWT Header secara native, biasanya menggunakan Cookie atau Query String Auth.
            return httpContext.User.Identity?.IsAuthenticated == true;
        }
    }
}
