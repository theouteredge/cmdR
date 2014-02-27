using System;

namespace cmdR
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CmdRouteAttribute : Attribute
    {
        public string Route { get; set; }
        public string Description { get; set; }
        
        public bool OverwriteRoutes { get; set; }

        
        public CmdRouteAttribute(string route, string description, bool overwriteRoutes)
        {
            Route = route;
            Description = description;
            OverwriteRoutes = overwriteRoutes;
        }

        public void RegisterRoute(CmdR cmdR)
        {
            
        }
    }
}