using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using MvcTeam.Utilities.Services;
using System.Activities;

namespace Utility.Workflows
{
    //Sets a user's personal settings: paging limit, advanced find mode, time zone, help/UI language,
    //default calendar view and send-as. A value of 0 means "leave it unchanged", except for default calendar view and send-as, which are always written.
    //Based on SetUserSettings from Dynamics-365-Workflow-Tools (Ms-PL, Demian Rasko).
    public class SetUserSettings : CodeActivity
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
        [Default("0")]
        public InArgument<int> DefaultCalendarView { get; set; }
        //0 = day, 1 = week, 2 = month. Always written, so 0 sets the day view.

        [RequiredArgument]
        [Input("Is Send As Allowed")]
        [Default("False")]
        public InArgument<bool> IsSendAsAllowed { get; set; }

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
            var isSendAsAllowed = IsSendAsAllowed.Get(context);

            ITracingService tracingService = context.GetExtension<ITracingService>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

            //System service, so it works even when the running user can't read or update user settings
            IOrganizationService systemService = serviceFactory.CreateOrganizationService(null);

            new CrmService(systemService, tracingService).SetUserSettings(user, pagingLimit, advancedFindStartupMode,
                timeZoneCode, helpLanguageId, uiLanguageId, defaultCalendarView, isSendAsAllowed);

            tracingService.Trace("Set user settings for user {0}: pagingLimit={1} advancedFind={2} timeZone={3} " +
                "helpLang={4} uiLang={5} calendarView={6} sendAs={7}",
                user.Id, pagingLimit, advancedFindStartupMode, timeZoneCode, helpLanguageId, uiLanguageId,
                defaultCalendarView, isSendAsAllowed);
        }
    }
}