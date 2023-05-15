
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InfrastructureCore.Web
{
    public interface IRazorViewRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}

