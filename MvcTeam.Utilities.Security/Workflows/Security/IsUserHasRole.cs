using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

public class Security_IsUserHasRole : CodeActivity
{
    [Input("User")]
    [RequiredArgument]
    [ReferenceTarget("systemuser")]
    public InArgument<EntityReference> User { get; set; }

    [Input("Security Role")]
    [RequiredArgument]
    [ReferenceTarget("role")]
    public InArgument<EntityReference> Role { get; set; }

    [Output("Is User Has Role")]
    public OutArgument<bool> HasRole { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
        //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
        var user = User.Get(context);
        if (user == null) throw new InvalidPluginExecutionException("User is required.");
        var role = Role.Get(context);
        if (role == null) throw new InvalidPluginExecutionException("Security Role is required.");

        ITracingService tracingService = context.GetExtension<ITracingService>();
        IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

        //System service, so the check works even when the running user can't read roles
        IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);

        var hasRole = new CrmService(systemService, tracingService).UserHasRole(user.Id, role.Id);

        tracingService.Trace("User {0} has role {1}: {2}", user.Id, role.Id, hasRole);
        HasRole.Set(context, hasRole);
    }
}
