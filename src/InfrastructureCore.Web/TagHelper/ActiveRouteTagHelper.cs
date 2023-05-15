using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using InfrastructureCore.Http.Extensions;
using InfrastructureCore.Models.Menu;

namespace InfrastructureCore.Web.TagHelper
{
    [HtmlTargetElement(Attributes = "is-active-route")]
    public class ActiveRouteTagHelper : AnchorTagHelper
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IHtmlGenerator htmlGenerator;
        SYMenu menu = null;

        // private IDictionary<string, string> _routeValues;
        public ActiveRouteTagHelper(IHttpContextAccessor contextAccessor, IHtmlGenerator htmlGenerator) : base(htmlGenerator)
        {
            this.contextAccessor = contextAccessor;
            this.htmlGenerator = htmlGenerator;
            menu = this.contextAccessor.HttpContext.Session.Get<SYMenu>("session_menu");
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "parent-menu-id");
            if (menu != null)
            {
                if (menu.MenuParentID.ToString() == classAttr.Value.ToString())
                {
                    output.Attributes.SetAttribute("is-active-show", "true");
                }
            }
        }
    }

    [HtmlTargetElement(Attributes = "is-active-color")]
    public class ActiveColorMenuTagHelper : AnchorTagHelper
    {
        private readonly IHttpContextAccessor contextAccessor;
        private IHtmlGenerator htmlGenerator;
        SYMenu menu = new SYMenu();

        public ActiveColorMenuTagHelper(IHttpContextAccessor contextAccessor, IHtmlGenerator htmlGenerator) : base(htmlGenerator)
        {
            this.htmlGenerator = htmlGenerator;
            this.contextAccessor = contextAccessor;
            menu = this.contextAccessor.HttpContext.Session.Get<SYMenu>("session_menu");
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "menu-id");
            if (menu != null)
            {
                if (menu.MenuID.ToString() == classAttr.Value.ToString())
                {
                    output.Attributes.SetAttribute("is-active-color", "true");
                }
            }

        }
    }
}
