using Microsoft.EntityFrameworkCore;

namespace VeriBill.Identity.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Add identity-related DbSets later (users, roles, etc.)
}
