using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProsjektoppgaveITE1811Gruppe7.Models;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [Authorize(Roles = "Frilanser")]
    public class FrilanserController : Controller
    {
        private readonly IOrderRepository _repository;
        private readonly IUsersWalletRepository _usersWalletRepository;
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        IHostingEnvironment _appEnvironment;

        public FrilanserController(IOrderRepository repository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IHostingEnvironment appEnvironment, IUsersWalletRepository usersWalletRepository)
        {
            _repository = repository;
            _roleManager = roleManager;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
            _usersWalletRepository = usersWalletRepository;
        }

        // GET: Orders        
        public async Task<IActionResult> Index()
        {
            ViewBag.FrilanserName = User.Identity.Name;
            return View(await _repository.GetFrilansersOrdersAsync(User));
        }

        //UploadSolution get
        public IActionResult UploadSolution(int id)
        {
            Order order = _repository.GetOrdersById(id);
            UploadSolutionViewModel upSolution = new UploadSolutionViewModel();
            upSolution.OrderId = id;
            upSolution.OrderName = order.OrderName;
            upSolution.OrderTask = order.OrderTask;
            upSolution.ClientName = order.Client.UserName;
            upSolution.DateOfDeadline = order.DateOfDeadline;
            return View(upSolution);
        }

        //UploadSolution post
        [HttpPost]
        public async Task<IActionResult> UploadSolution([Bind("OrderId, Solution, uploadedFile")] UploadSolutionViewModel upSolution)
        {
            Order order = _repository.GetOrdersById(upSolution.OrderId);

            order.Solution = upSolution.Solution;

            if (upSolution.uploadedFile != null)
            {
                string now = DateTime.Now.ToString("ddHHmmss");

                string path = "/Files/" + now + upSolution.OrderId + upSolution.uploadedFile.FileName;

                using (var fileStream = _repository.getFilestream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await upSolution.uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = now + upSolution.OrderId + upSolution.uploadedFile.FileName, Path = path };


                order.NumberOfStrings = _repository.getNumberOfStrings(_appEnvironment.WebRootPath + path);

                order.SolutionFile = _repository.AddFile(file);

            }
            order.Status = 1;

            _repository.saveSolution(order);

            return RedirectToAction(nameof(Index));
        }

        //UploadSolution get
        public IActionResult Details(int id)
        {
            Order order = _repository.GetOrdersById(id);
            UploadSolutionViewModel upSolution = new UploadSolutionViewModel();
            upSolution.OrderId = id;
            upSolution.OrderName = order.OrderName;
            upSolution.OrderTask = order.OrderTask;
            upSolution.ClientName = order.Client.UserName;
            upSolution.DateOfDeadline = order.DateOfDeadline;

            return View(upSolution);
        }

        public async Task<IActionResult> TakeOrder(int id)
        {
            Order order = _repository.GetOrdersById(id);
            IdentityUser frilanser = await _userManager.FindByNameAsync(User.Identity.Name);
            order.Frilanser = frilanser;
            _repository.saveSolution(order);
            return RedirectToAction("Index");
        }

        //Take order post
        public IActionResult RefuseOrder(int id)
        {
            Order order = _repository.GetOrdersById(id);

            order.Frilanser = null;
            _repository.saveSolution(order);
            return RedirectToAction("Index");
        }

        //Get Chat 
        public IActionResult ChatWithClient(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.ClientName = _repository.GetClientsNameByOrderId(id);
            ViewBag.FrilanserName = User.Identity.Name;
            return View();
        }

        public async Task<IActionResult> EditSpecialization()
        {
            // get user
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            // get list of users roles
            var userRoles = await _userManager.GetRolesAsync(user);
            userRoles.Remove("Frilanser");
            var allRoles = _roleManager.Roles.Where(r => r.Name.Contains("Specialization")).ToList();
            ChangeRoleViewModel model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSpecialization(List<string> roles)
        {
            // get user
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            roles.Add("Frilanser");
            // get list of users roles
            var userRoles = await _userManager.GetRolesAsync(user);
            // get all roles
            var allRoles = _roleManager.Roles.ToList();
            // get list of added roles
            var addedRoles = roles.Except(userRoles);
            // get deleted roles
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);

            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction("Index");
        }

        //Get Wallet
        public async Task<IActionResult> Wallet()
        {
            WalletViewModel walletViewModel = new WalletViewModel();

            if (_usersWalletRepository.isUserHasAddress(User).Result)
            {
                string address = await _usersWalletRepository.GetUsersWalletAddress(User);
                Payments pc = new Payments();
                List<string> Addresses = new List<string>();
                Addresses.Add(address);

                try
                {
                    APIResponse responseBalance = _repository.getBalance(pc, "addresses", Addresses);
                    APIResponse responseTransactions = _repository.getTransactions(pc, "received", Addresses);

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

                                foreach (var item2 in (JArray)item["amounts_received"])  //amounts_received// amounts_sent
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
            return View(walletViewModel);
        }

        //Get  Create Wallet-address
        public async Task<IActionResult> CreateWallet()
        {
            IdentityUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Payments pc = new Payments();
            
            APIResponse response = _repository.getAddress(pc);

            if (response.Status == "success")
            {
                UsersWalletsModel wallet = new UsersWalletsModel();
                wallet.Address = (string)response.Data["address"];
                wallet.User = user;
                _usersWalletRepository.CreateWalletAddress(wallet);
                return RedirectToAction("Wallet");
            }

            return RedirectToAction("Index");

        }
    }
}