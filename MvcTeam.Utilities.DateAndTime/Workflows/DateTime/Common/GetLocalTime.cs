using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using MvcTeam.Utilities.Services;
using System;
using System.Linq;

namespace MvcTeam.Utilities.Workflows.Common
{
    public class GetLocalTime
    {
        //The time zone code of the user running the workflow; null when that user has no settings record or no time zone set
        public static int? RetrieveTimeZoneCode(IOrganizationService service)
        {
            var currentUserSettings = service.RetrieveMultiple(
                new QueryExpression("usersettings")
                {
                    ColumnSet = new ColumnSet("timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions = {
                            new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                        }
                    }
                });

            return currentUserSettings.Entities.FirstOrDefault()?.GetAttributeValue<int?>("timezonecode");
        }

        //Without a time zone code the time is given in Iran time, the zone the other date steps use
        public static DateTime RetrieveLocalTimeFromUtcTime(DateTime utcTime, int? timeZoneCode, IOrganizationService service)
        {
            if (!timeZoneCode.HasValue)
                return IranTime.FromUtc(utcTime);

            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode.Value,
                UtcTime = utcTime.ToUniversalTime()
            };

            OrganizationResponse response = service.Execute(request);
            return (DateTime)response.Results["LocalTime"];
        }
    }
}