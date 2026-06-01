using ERP.Approval.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Service.API.Approval;

public static class ApprovalServiceCollectionExtensions
{
    public static IServiceCollection AddApprovalServiceAdapters(this IServiceCollection services)
    {
        services.AddScoped<IApprovalUserContext, ApprovalCurrentUserContext>();
        services.AddScoped<IApprovalTargetHandler, DayoffApprovalTargetHandler>();
        services.AddScoped<IApprovalTargetHandler, DocumentApprovalTargetHandler>();
        services.AddScoped<IApprovalTargetHandler, PurchaseApprovalTargetHandler>();

        return services;
    }
}
