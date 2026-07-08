
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Services.Interfaces;

namespace VariBillWebAPI.Data.Interceptors;

/// <summary>
/// Interceptor to audit entity changes.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts EF Core SaveChanges to capture audit logs for entity changes.
    /// </summary>
    private readonly ILogger<AuditInterceptor> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(ILogger<AuditInterceptor> logger, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }


    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context != null)
        {
            var auditEntries = new List<AuditLog>();
            foreach (var entry in context.ChangeTracker.Entries())
            {
                // Skip AuditLog itself and unchanged/detached
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var audit = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    EntityName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    UserId = _currentUserService.UserId,
                    // Optional: capture IP/UserAgent
                    // IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    // UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
                };

                // Set EntityId (primary key value)
                var key = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
                if (key != null)
                    audit.EntityId = key.CurrentValue?.ToString() ?? string.Empty;

                // Capture values
                if (entry.State == EntityState.Modified)
                {
                    var changed = entry.Properties.Where(p => p.IsModified).ToList();
                    audit.OldValues = JsonSerializer.Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()));
                    audit.NewValues = JsonSerializer.Serialize(changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
                }
                else if (entry.State == EntityState.Added)
                {
                    audit.NewValues = JsonSerializer.Serialize(
                        entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
                }
                else if (entry.State == EntityState.Deleted)
                {
                    audit.OldValues = JsonSerializer.Serialize(
                        entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()));
                }

                auditEntries.Add(audit);
            }

            if (auditEntries.Any())
            {
                context.Set<AuditLog>().AddRange(auditEntries);
                await context.SaveChangesAsync(cancellationToken); // Save audit logs separately
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}







//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Diagnostics;
//using System.Text.Json;
//using VariBillWebAPI.Data.Entities;

//namespace VariBillWebAPI.Data.Interceptors;

//public class AuditInterceptor : SaveChangesInterceptor
//{
//    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
//    {
//        var context = eventData.Context;
//        if (context != null)
//        {
//            var auditEntries = new List<AuditLog>();
//            foreach (var entry in context.ChangeTracker.Entries())
//            {
//                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
//                    continue;

//                var auditLog = new AuditLog
//                {
//                    Id = Guid.NewGuid(),
//                    EntityName = entry.Entity.GetType().Name,
//                    Action = entry.State.ToString(),
//                    Timestamp = DateTime.UtcNow
//                };

//                if (entry.State == EntityState.Modified)
//                {
//                    var changedProps = entry.Properties.Where(p => p.IsModified).ToList();
//                    auditLog.OldValues = JsonSerializer.Serialize(changedProps.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()));
//                    auditLog.NewValues = JsonSerializer.Serialize(changedProps.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
//                }
//                else if (entry.State == EntityState.Added)
//                {
//                    auditLog.NewValues = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue?.ToString()));
//                }
//                else if (entry.State == EntityState.Deleted)
//                {
//                    auditLog.OldValues = JsonSerializer.Serialize(entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue?.ToString()));
//                }

//                auditEntries.Add(auditLog);
//            }

//            if (auditEntries.Any())
//            {
//                context.Set<AuditLog>().AddRange(auditEntries);
//                await context.SaveChangesAsync(cancellationToken);
//            }
//        }

//        return await base.SavedChangesAsync(eventData, result, cancellationToken);
//    }
//}
