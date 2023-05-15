using Microsoft.AspNetCore.Mvc;
using Modules.Pleiger.Services.IService;

namespace Modules.Pleiger.Controllers
{
    public class CompanyController : Controller
    {
        private ICompanyService companyService;

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        public IActionResult Index()
        {
            var company= companyService.GetCompanyInforByID(1);
            return View(company);
        }
        
        public IActionResult Popup()
        {
            return View();
        }
    }
}
