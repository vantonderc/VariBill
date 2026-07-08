using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace VeriBill.Identity;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Minimal host for identity project scaffolding
        builder.Services.AddRazorPages();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapRazorPages();

        app.Run();
    }
}
