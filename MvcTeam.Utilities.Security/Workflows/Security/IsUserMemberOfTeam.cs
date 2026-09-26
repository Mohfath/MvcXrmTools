using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

public class Security_IsUserMemberOfTeam : CodeActivity
{
    [Input("User")]
    [RequiredArgument]
    [ReferenceTarget("systemuser")]
    public InArgument<EntityReference> User { get; set; }

    [Input("Team")]
    [RequiredArgument]
    [ReferenceTarget("team")]
    public InArgument<EntityReference> Team { get; set; }

    [Output("Is Member")]
    public OutArgument<bool> IsMember { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
        //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
        var user = User.Get(context);
        if (user == null) throw new InvalidPluginExecutionException("User is required.");
        var team = Team.Get(context);
        if (team == null) throw new InvalidPluginExecutionException("Team is required.");

        ITracingService tracingService = context.GetExtension<ITracingService>();
        IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

        //System service, so the check works even when the running user can't read teams
        IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);

        var isMember = new CrmService(systemService, tracingService).IsUserMemberOfTeam(user.Id, team.Id);

        tracingService.Trace("User {0} is member of team {1}: {2}", user.Id, team.Id, isMember);
        IsMember.Set(context, isMember);
    }
}
