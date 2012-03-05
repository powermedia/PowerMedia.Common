using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Web.Routing;

namespace PowerMedia.Common.System.Web
{
    public static class RequestExtensions
    {
        public static ForwardableRoute ForwardRoute(this HttpRequestBase request)
        {
            return new ForwardableRoute(getRouteValues(request.QueryString));
        }

        public static bool GetBooleanParameterValue(this HttpRequestBase request, string name, bool defaultValue = false)
        {
            var result = DetermineBooleanParameterValue(request.QueryString, name);
            if (result.HasValue)
            {
                return result.Value;
            }
            return defaultValue;
        }

        private static bool? DetermineBooleanParameterValue(this HttpRequestBase request, string name)
        {
            return DetermineBooleanParameterValue(request.QueryString, name);
        }

        private static bool? DetermineBooleanParameterValue(NameValueCollection parameters, string name)
        {
            var parameter = parameters[name];
            if (parameter == null) { return null; }
            var values = parameter.Split(',');
            bool result = false;
            var parsedAnyBool = false;
            foreach (var value in values)
            {
                bool parsedValue;
                parsedAnyBool |= Boolean.TryParse(value, out parsedValue);
                if (parsedValue)
                {
                    result = true;
                    break;
                }
            }
            if (parsedAnyBool) { return result; }
            return null;
        }

        private static RouteValueDictionary getRouteValues(NameValueCollection parameters)
        {
            var routeValues = new RouteValueDictionary();

            foreach (string key in parameters.Keys)
            {
                var value = DetermineBooleanParameterValue(parameters, key);
                if (value.HasValue)
                {
                    routeValues.Add(key, value.Value);
                }
                else
                {
                    routeValues.Add(key, parameters[key]);
                }

            }
            return routeValues;
        }
    }
}