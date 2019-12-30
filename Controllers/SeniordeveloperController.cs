using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProsjektoppgaveITE1811Gruppe7.Models;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [Authorize(Roles = "SeniorDeveloper")]
    public class SeniordeveloperController : Controller
    {
        private readonly IOrderRepository _repository;
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        IHostingEnvironment _appEnvironment;

        public SeniordeveloperController(IOrderRepository repository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IHostingEnvironment appEnvironment)
        {
            _repository = repository;
            _roleManager = roleManager;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View(_repository.GetOrdersWithStatus(1));
        }

        public IActionResult Details(int id)
        {
            Order order = _repository.GetOrdersById(id);
            OrderDetails orderDetails = new OrderDetails();

            orderDetails.OrderId = order.OrderId;
            orderDetails.DateCreated = order.DateCreated;
            orderDetails.DateOfDeadline = order.DateOfDeadline;
            orderDetails.OrderName = order.OrderName;
            orderDetails.OrderTask = order.OrderTask;
            orderDetails.Status = order.Status;
            orderDetails.NumberOfStrings = order.NumberOfStrings;
            if (order.SolutionFile != null)
            {
                orderDetails.SolutionStrings = _repository.getSolutionStrings(_appEnvironment.WebRootPath + order.SolutionFile.Path);
                orderDetails.SolutionFileId = order.SolutionFile.Id;
            }

            orderDetails.ClientId = order.Client.Id;

            if (order.SeniorUtvikler != null)
            {
                orderDetails.SeniorUtvikler.UserName = order.SeniorUtvikler.UserName;
                orderDetails.Frilanser.Id = order.Frilanser.Id;
            }
            if (order.Frilanser != null)
            {

                orderDetails.Frilanser.UserName = order.Frilanser.UserName;

            }
            if (order.SpecializationRole != null)
                orderDetails.SpecializationRoleName = order.SpecializationRole.Name;

            return View(orderDetails);
        }

        public IActionResult GetFile(int id)
        {
            FileModel fileForClient = _repository.GetFileById(id);
            string file_path = Path.Combine(_appEnvironment.WebRootPath, fileForClient.Path.Remove(0, 1));
            string file_type = "application/txt";
            string file_name = fileForClient.Name;
            return PhysicalFile(file_path, file_type, file_name);
        }

        // GET: AproveSolution
        public IActionResult AproveSolution(int id)
        {
            Order aprovedOrder = _repository.GetOrdersById(id);
            aprovedOrder.Status = 2; //aproved
            _repository.UpdateOrder(aprovedOrder);

            return RedirectToAction(nameof(Index));
        }
    }
}