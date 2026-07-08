using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace VeriBill.Identity.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication ConfigureVeriBillIdentity(this WebApplication app)
    {
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapRazorPages();
        return app;
    }
}
