using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProsjektoppgaveITE1811Gruppe7.Models;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            if (HttpContext.User.IsInRole("Client"))
            {
                return Redirect("/Clients");
            }
            else if (HttpContext.User.IsInRole("ProjectManager"))
            {
                return Redirect("/ProjectManager");
            }
            else if (HttpContext.User.IsInRole("SeniorDeveloper"))
            {
                return Redirect("/SeniorDeveloper");
            }
            else if (HttpContext.User.IsInRole("Frilanser"))
            {
                return Redirect("/Frilanser");
            }
            else if (HttpContext.User.IsInRole("ProjectManager"))
            {
                return Redirect("/ProjectManager");
            }
            else if (HttpContext.User.IsInRole("SeniorDeveloper"))
            {
                return Redirect("/SeniorDeveloper");
            }
            else
            {
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
