using System.Web;
using System.Web.Mvc;

namespace Fanex.NAP.SPU.Graph.Core.TestWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
