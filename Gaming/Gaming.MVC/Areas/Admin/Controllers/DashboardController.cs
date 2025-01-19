using Microsoft.AspNetCore.Mvc;

namespace Gaming.MVC.Areas.Admin.Controllers;
[Area("Admin")]
public class DashboardController : Controller
{
    // GET: DashboardController
    public ActionResult Index()
    {
        return View();
    }

}
