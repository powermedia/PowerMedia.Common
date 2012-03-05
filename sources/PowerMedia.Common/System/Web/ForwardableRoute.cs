using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Reflection;

namespace PowerMedia.Common.System.Web
{
    public class ForwardableRoute
    {
        private RouteValueDictionary _routeValues;
        public ForwardableRoute(RouteValueDictionary routeValues)
        {
            _routeValues = routeValues;
        }

        public ForwardableRoute Add(object newParameters)
        {
            foreachProperty(newParameters, (name, value) => _routeValues.Add(name, value));
            return this;
        }

        public ForwardableRoute Set(object modifiedParameters)
        {
            foreachProperty(modifiedParameters, (name, value) => _routeValues[name] = value);
            return this;
        }

        public ForwardableRoute Remove(params string[] parameterKeys)
        {
            Array.ForEach(parameterKeys, key => _routeValues.Remove(key));
            return this;
        }

        private void foreachProperty(object target, Action<string, object> propertyAction)
        {
            foreach (PropertyInfo pi in target.GetType().GetProperties())
            {
                propertyAction(pi.Name, pi.GetValue(target, null));
            }
        }

        public static implicit operator RouteValueDictionary(ForwardableRoute route)
        {
            return route._routeValues;
        }
    }
}
