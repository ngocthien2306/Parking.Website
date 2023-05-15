using InfrastructureCore.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Modules.Parking.Controllers
{
    public class ParkingDeviceController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
