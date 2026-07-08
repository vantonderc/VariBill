using Microsoft.Extensions.Diagnostics.HealthChecks;
using VariBillWebAPI.Data.Context;

namespace VariBillWebAPI.HealthChecks;

/// <summary>
/// Health check for database connectivity.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly VeriBillTestDBContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="DatabaseHealthCheck"/>.
    /// </summary>
    /// <param name="context">The database context to check connectivity with.</param>
    public DatabaseHealthCheck(VeriBillTestDBContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is unreachable.", ex);
        }
    }
}
