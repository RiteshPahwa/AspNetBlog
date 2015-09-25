using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/12/15*/

namespace ASPNetBlog.App_Common.Extensions
{
    // Read More @ http://jameschambers.com/2014/06/day-14-bootstrap-alerts-and-mvc-framework-tempdata/
    public class Alert
    {
        public string AlertStyle { get; set; }
        public string Message { get; set; }
        public bool Dismissable { get; set; }
    }

    public static class AlertStyle
    {
        public const string Default = nameof(Default); //C# 6.0
        public const string Info = nameof(Info);
        public const string Success = nameof(Success); 
        public const string Warning = nameof(Warning);
        public const string Danger = nameof(Danger);
    }

    public static class Alerts
    {
        public const string TempDataKey = nameof(TempDataKey); //C# 6.0

        public static ITempDataDictionary Default(this ITempDataDictionary TempData, string message, bool dismissable = true)
        {
            return TempData.AddAlert(AlertStyle.Default, message, dismissable);
        }

        public static ITempDataDictionary Info(this ITempDataDictionary TempData, string message, bool dismissable = true)
        {
            return TempData.AddAlert(AlertStyle.Info, message, dismissable);
        }

        public static ITempDataDictionary Success(this ITempDataDictionary TempData, string message, bool dismissable = true)
        {
            return TempData.AddAlert(AlertStyle.Success, message, dismissable);
        }

        public static ITempDataDictionary Warning(this ITempDataDictionary TempData, string message, bool dismissable = false)
        {
            return TempData.AddAlert(AlertStyle.Warning, message, dismissable);
        }

        public static ITempDataDictionary Danger(this ITempDataDictionary TempData, string message, bool dismissable = false)
        {
            return TempData.AddAlert(AlertStyle.Danger, message, dismissable);
        }

        private static ITempDataDictionary AddAlert(this ITempDataDictionary TempData, string alertStyle, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(Alerts.TempDataKey) ? (List<Alert>)TempData[Alerts.TempDataKey] : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle.ToLower(),
                Message = message,
                Dismissable = dismissable
            });

            TempData[Alerts.TempDataKey] = alerts;

            return TempData;
        }
    }
}
