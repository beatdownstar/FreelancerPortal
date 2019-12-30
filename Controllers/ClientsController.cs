using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using ProsjektoppgaveITE1811Gruppe7.Hubs;
using ProsjektoppgaveITE1811Gruppe7.Models;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [Authorize(Roles = "Client")]
    public class ClientsController : Controller
    {
        private readonly IOrderRepository _repository;
        private readonly IUsersWalletRepository _usersWalletRepository;
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        IHostingEnvironment _appEnvironment;
        private readonly IHubContext<NotificationsHub> _notifHubContext;


        public ClientsController(IOrderRepository repository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IHostingEnvironment appEnvironment, IHubContext<NotificationsHub> notifHubContext, IUsersWalletRepository usersWalletRepository)
        {
            _repository = repository;
            _roleManager = roleManager;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _notifHubContext = notifHubContext;
            _usersWalletRepository = usersWalletRepository;
        }


        // GET: Orders     
        public async Task<IActionResult> Index()
        {

            ViewBag.ClientsName = ViewBag.FrilanserName = User.Identity.Name;
            return View(await _repository.GetClientsOrdersAsync(User));
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            NewOrderViewModel newOrder = new NewOrderViewModel();
            newOrder.DateOfDeadline = DateTime.Now;
            var IListOfFrilansers = await _userManager.GetUsersInRoleAsync("Frilanser");
            List<Frilansers> ListOfFrilansers = new List<Frilansers>();
            newOrder.SpecializationRoles = _roleManager.Roles.Where(r => r.Name.Contains("Specialization")).ToList();

            for (int i = 0; i < IListOfFrilansers.Count; i++)
            {
                ListOfFrilansers.Add(new Frilansers { Id = IListOfFrilansers[i].Id, UserName = IListOfFrilansers[i].UserName });
            }

            newOrder.Frilansers = ListOfFrilansers;
            return View(newOrder);
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DateOfDeadline, OrderName, OrderTask, FrilanserId, SpecializationRoleId")] NewOrderViewModel newOrder)
        {
            if (ModelState.IsValid)
            {
                Order order = new Order();
                order.DateOfDeadline = newOrder.DateOfDeadline;
                order.OrderName = newOrder.OrderName;
                order.OrderTask = newOrder.OrderTask;
               newOrder.SpecializationRoles = _roleManager.Roles.ToList();

                if (newOrder.FrilanserId != "null")
                {
                    order.Frilanser = await _userManager.FindByIdAsync(newOrder.FrilanserId);
                    order.SpecializationRole = await _roleManager.FindByNameAsync("Universal (Specialization)");
                }
                else if (newOrder.SpecializationRoleId != null)
                {
                    order.SpecializationRole = await _roleManager.FindByIdAsync(newOrder.SpecializationRoleId);
                }
                else
                {
                    order.SpecializationRole = await _roleManager.FindByNameAsync("Universal (Specialization)");
                }

                try
                {
                    int newId = await _repository.CreateNewOrder(order, User);
                    string SpecializationRole = null;

                    if (order.SpecializationRole != null)
                    {
                        SpecializationRole = order.SpecializationRole.Name;

                    }
                    string frilanser = null;

                    if (order.Frilanser != null)
                    {
                        frilanser = order.Frilanser.UserName;


                    }
                    await _notifHubContext.Clients
                        .All
                        .SendAsync("NewOrderNorificationAssignedFrilanser", newId, frilanser, User.Identity.Name, order.OrderName, order.DateOfDeadline, SpecializationRole);

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(order);
                }
            }
            else
            {
                return View("Create");
            }
        }

        // GET: Clients/OrderDetails
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

        // GET: Clients/AproveSolution
        public IActionResult AproveSolution(int id)
        {
            Order aprovedOrder = _repository.GetOrdersById(id);
            aprovedOrder.Status = 4; //aproved
            _repository.UpdateOrder(aprovedOrder);
            InvoiceModel invoice = new InvoiceModel()
            {
                Status = false,
                Amount = aprovedOrder.NumberOfStrings * 0.023,
                order = aprovedOrder
            };

            _repository.CreateInvoice(invoice);

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/ListOfInvoices
        public async Task<IActionResult> ListOfInvoices()
        {
            ClientsInvoicesViewModel clientsInvoicesViewModel = new ClientsInvoicesViewModel();
            List<InvoiceModel> invoices = await _repository.GetClientsInvoicesAsync(User.Identity.Name);
            if (invoices.Count != 0)
            {
                clientsInvoicesViewModel.invoices = invoices;
                clientsInvoicesViewModel.PriceForString = 0.0023;
                return View(clientsInvoicesViewModel);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Pay(int id)
        {
            InvoiceModel invoice = _repository.GetInvoice(id);
            List<string> FromAddresses = new List<string>();
            List<string> ToAddresses = new List<string>();
            List<string> Amounts = new List<string>();
            FromAddresses.Add(_usersWalletRepository.getWalletAddressById(2));
            ToAddresses.Add(_usersWalletRepository.getWalletAddressById(3));

            Amounts.Add(invoice.Amount.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")));

            Payments pc = new Payments();
            try
            {
                APIResponse response = _repository.withdrawFromAddresses(pc, FromAddresses, ToAddresses, Amounts);
                invoice.Status = true;
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("ListOfInvoices");
        }


        //Get Chat 
        public IActionResult ChatWithFrilanser(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.FrilanserName = _repository.GetFrilansersNameByOrderId(id);
            return View();
        }

        //Get Wallet
        public IActionResult Wallet()
        {
            WalletViewModel walletViewModel = new WalletViewModel();

            string address = _usersWalletRepository.getWalletAddressById(2);
            Payments pc = new Payments();
            List<string> Addresses = new List<string>();
            Addresses.Add(address);

            try
            {
                APIResponse responseBalance = _repository.getBalance(pc, "addresses", Addresses);
                APIResponse responseTransactions = _repository.getTransactions(pc, "sent", Addresses);

                if (responseBalance.Status == "success")
                {
                    walletViewModel.Balance = (string)responseBalance.Data["available_balance"];
                    Dictionary<long, string> listOftransactions = new Dictionary<long, string>();
                    if ((JArray)responseTransactions.Data["txs"] != null)
                    {
                        foreach (var item in (JArray)responseTransactions.Data["txs"])
                        {
                            long date = (long)item["time"];
                            string ammount = "";

                            foreach (var item2 in (JArray)item["amounts_sent"])  
                            {
                                ammount = (string)item2["amount"];
                            }

                            listOftransactions.Add(date, ammount);
                        }
                        walletViewModel.Transactions = listOftransactions;
                    }
                }

                return View(walletViewModel);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}