using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Modules.UIRender.Utils
{
    public static class ControllerExtensions
    {
        public static async Task<ActionResult> GetPlainJavaScriptAsync(
            this Controller controller, string viewName = null, object model = null)
        {
            const string JsPattern = @"^<script[^>]*>(.*)</script>$";
            var viewResult = await RenderPartialViewToStringAsync(controller, viewName, model);
            var content = Regex.Replace(viewResult, JsPattern, "$1", RegexOptions.Singleline);

            return new ContentResult
            {
                //ContentType = new MediaTypeHeaderValue("text/javascript"),
                Content = content
            };
        }

        private static async Task<string> RenderPartialViewToStringAsync(
            this Controller controller, string viewName = null, object model = null)
        {
            viewName = viewName ?? controller.ControllerContext.ActionDescriptor.DisplayName;
            controller.ViewData.Model = model;

            using (StringWriter stringWriter = new StringWriter())
            {
                var engine = (ICompositeViewEngine)controller.HttpContext.RequestServices
                    .GetService(typeof(ICompositeViewEngine));
                var viewEngineResult = engine.FindView(
                    controller.ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    controller.ControllerContext, viewEngineResult.View, controller.ViewData,
                    controller.TempData, stringWriter, new HtmlHelperOptions());

                await viewEngineResult.View.RenderAsync(viewContext);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
