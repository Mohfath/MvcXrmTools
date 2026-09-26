using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

//Sets a user's personal settings: paging limit, advanced find mode, time zone, help/UI language,
//default calendar view and send-as. A value of 0 means "leave it unchanged" (calendar view: -1; send-as: Update Send As = False).
//Based on SetUserSettings from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko).
public class Security_SetUserSettings : CodeActivity
{
    [RequiredArgument]
    [Input("User")]
    [ReferenceTarget("systemuser")]
    public InArgument<EntityReference> User { get; set; }

    [RequiredArgument]
    [Input("Paging Limit")]
    [Default("0")]
    public InArgument<int> PagingLimit { get; set; }
    //How many records per view. Value can be 25, 50, 75, 100, 250. 0 means leave unchanged.

    [RequiredArgument]
    [Input("Advanced Find Startup Mode")]
    [Default("1")]
    public InArgument<int> AdvancedFindStartupMode { get; set; }
    //1 = simple, 2 = detail. 0 means leave unchanged.

    [RequiredArgument]
    [Input("Time Zone Code")]
    [Default("0")]
    public InArgument<int> TimeZoneCode { get; set; }
    //Use Get-CrmTimeZones to see all options. 0 means leave unchanged.

    [RequiredArgument]
    [Input("Help Language Id")]
    [Default("0")]
    public InArgument<int> HelpLanguageId { get; set; }
    //Unique identifier of the help language. 0 means leave unchanged.

    [RequiredArgument]
    [Input("UI Language Id")]
    [Default("0")]
    public InArgument<int> UILanguageId { get; set; }
    //Unique identifier of the language in which to view the UI. 0 means leave unchanged.

    [RequiredArgument]
    [Input("Default Calendar View")]
    [Default("-1")]
    public InArgument<int> DefaultCalendarView { get; set; }
    //0 = day, 1 = week, 2 = month, -1 = leave unchanged.

    [RequiredArgument]
    [Input("Is Send As Allowed")]
    [Default("False")]
    public InArgument<bool> IsSendAsAllowed { get; set; }

    //Is Send As Allowed is only written when this is True, so the default never changes a user's Send As
    [RequiredArgument]
    [Input("Update Send As")]
    [Default("False")]
    public InArgument<bool> UpdateSendAs { get; set; }

    protected override void Execute(CodeActivityContext context)
    {
        //RequiredArgument only applies in the designer; a dynamic value can still be empty at run time
        var user = User.Get(context);
        if (user == null) throw new InvalidPluginExecutionException("User is required.");

        var pagingLimit = PagingLimit.Get(context);
        var advancedFindStartupMode = AdvancedFindStartupMode.Get(context);
        var timeZoneCode = TimeZoneCode.Get(context);
        var helpLanguageId = HelpLanguageId.Get(context);
        var uiLanguageId = UILanguageId.Get(context);
        var defaultCalendarView = DefaultCalendarView.Get(context);
        bool? isSendAsAllowed = UpdateSendAs.Get(context) ? IsSendAsAllowed.Get(context) : (bool?)null;

        ITracingService tracingService = context.GetExtension<ITracingService>();
        IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
        IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

        //Runs as the workflow's user, so CRM's own permissions decide who may change whose settings
        //(a system service would let anyone who can build a workflow, for example, grant Send As to any user)
        IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

        new CrmService(service, tracingService).SetUserSettings(user, pagingLimit, advancedFindStartupMode,
            timeZoneCode, helpLanguageId, uiLanguageId, defaultCalendarView, isSendAsAllowed);

        tracingService.Trace("Set user settings for user {0}: pagingLimit={1} advancedFind={2} timeZone={3} " +
            "helpLang={4} uiLang={5} calendarView={6} sendAs={7}",
            user.Id, pagingLimit, advancedFindStartupMode, timeZoneCode, helpLanguageId, uiLanguageId,
            defaultCalendarView, isSendAsAllowed);
    }
}
