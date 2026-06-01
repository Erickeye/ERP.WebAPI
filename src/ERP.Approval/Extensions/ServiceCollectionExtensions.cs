using ERP.Approval.Abstractions;
using ERP.Approval.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Approval.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApprovalModule(this IServiceCollection services)
    {
        services.AddScoped<IApprovalWorkflowService, ApprovalWorkflowService>();
        services.AddScoped<IApprovalSettingsService, ApprovalSettingsService>();
        services.AddScoped<IApprovalTargetHandlerResolver, ApprovalTargetHandlerResolver>();

        return services;
    }
}
