using JiraCloneMVC.Web.Repositories;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JiraCloneMVC.Web.Attributes
{
    public class ProjectGroupAuthorizeAttribute : AuthorizeAttribute
    {
        public string ProjectIdQueryParam { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
            if (routeData.Values.ContainsKey("MS_DirectRouteMatches"))
                routeData = ((IEnumerable<RouteData>)routeData.Values["MS_DirectRouteMatches"]).ElementAt(0);
            if (!routeData.Values.ContainsKey(ProjectIdQueryParam))
                return false;
            if (!httpContext.User.Identity.IsAuthenticated || !int.TryParse(routeData.Values[ProjectIdQueryParam] as string, out int projectId))
                return false;
            return HasPermission(httpContext.User.Identity.GetUserId(), Roles, projectId) || base.AuthorizeCore(httpContext);
        }

        private bool HasPermission(string userId, string roles, int projectId)
        {
            var groupRepository = new GroupRepository(new ApplicationDbContext());
            var groupRoles = groupRepository.GetProjectRolesOfUser(userId, projectId);
            return groupRoles.Any(gr => roles.Contains(gr));
        }
    }
}