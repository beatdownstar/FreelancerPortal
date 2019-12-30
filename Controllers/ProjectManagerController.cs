using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProsjektoppgaveITE1811Gruppe7.Models;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;

namespace ProsjektoppgaveITE1811Gruppe7.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    [Authorize(Roles = "ProjectManager")]
    public class ProjectManagerController : Controller
    {
        private readonly IOrderRepository _repository;
        IUsersWalletRepository _usersWalletRepository;
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;

        public ProjectManagerController(IOrderRepository repository, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IUsersWalletRepository usersWalletRepository)
        {
            _repository = repository;
            _roleManager = roleManager;
            _userManager = userManager;
            _usersWalletRepository = usersWalletRepository;
        }

        public async Task<IActionResult> Index()
        {
            var IListOfFrilansers = await _userManager.GetUsersInRoleAsync("Frilanser");
            List<Frilansers> ListOfFrilansers = new List<Frilansers>();
            for (int i = 0; i < IListOfFrilansers.Count; i++)
            {
                ListOfFrilansers.Add(new Frilansers { Id = IListOfFrilansers[i].Id, UserName = IListOfFrilansers[i].UserName });
            }

            var IListOfClients = await _userManager.GetUsersInRoleAsync("Client");

            List<ClientModel> ListOfClients = new List<ClientModel>();
            for (int i = 0; i < IListOfClients.Count; i++)
            {
                ListOfClients.Add(new ClientModel { Id = IListOfClients[i].Id, UserName = IListOfClients[i].UserName });
            }

            ProjectManagerIndexViewModel projectManagerIndexViewModel = new ProjectManagerIndexViewModel
            {
                Frilansers = ListOfFrilansers,
                Clients = ListOfClients
            };
            return View(projectManagerIndexViewModel);
        }

        // GET: ProjectManger/FrilanserDetails
        public async Task<IActionResult> FrilanserDetails(string id)
        {
            List<IdentityUser> listOfCliens = await _repository.ListOfClientsForFrilanser(id);

            List<ClientSumStringsModel> clientSumStrings = new List<ClientSumStringsModel>();

            foreach (IdentityUser client in listOfCliens)
            {
                clientSumStrings.Add(new ClientSumStringsModel()
                {
                    ClientName = client.UserName,
                    SummOfStrings = await _repository.SumStringFrilanserToClientAsync(client.Id, id)
                }
                    );
            }

            List<DateTime> datesFrilanserWorked = await _repository.DatesFrilanserWorkedAsync(id);
            List<string> datesInStrings = new List<string>();

            foreach (DateTime date in datesFrilanserWorked)
            {
                datesInStrings.Add("01." + date.ToString("MM.yyyy"));
            }

            datesInStrings.Distinct();

            List<FrilanserMonthStringsModel> frilanserMonthStringsModel = new List<FrilanserMonthStringsModel>();

            foreach (string date in datesInStrings)
            {
                frilanserMonthStringsModel.Add(new FrilanserMonthStringsModel()
                {
                    Period = DateTime.Parse(date).ToString("MM.yyyy"),
                    NumberOfStrings = await _repository.SumStringInMonthForFrilanserAsync(id, DateTime.Parse(date)),
                    Status = await _repository.PaymentStatusToFrilanserInPeriode(id, DateTime.Parse(date)),
                    FrilanserId = id,
                    Amount = await _repository.SumStringInMonthForFrilanserAsync(id, DateTime.Parse(date)) * 0.002
                });

            }
            IdentityUser frilanser = await _userManager.FindByIdAsync(id);

            FrilanserDetailsViewModel frilanserDetailsViewModel = new FrilanserDetailsViewModel()
            {
                FrilanserName = frilanser.UserName,
                ClientSumStrings = clientSumStrings,
                FrilanserMonthStrings = frilanserMonthStringsModel
            };

            return View(frilanserDetailsViewModel);
        }

        // GET: ProjectManger/ClientDetails
        public async Task<IActionResult> ClientDetails(string id)
        {
            List<IdentityUser> listOfFrilansers = await _repository.FrilansersOfClientAsync(id);
            List<FrilanserSumStringModel> frilanserSumStrings = new List<FrilanserSumStringModel>();

            foreach (IdentityUser frilanserOfClient in listOfFrilansers)
            {
                frilanserSumStrings.Add(new FrilanserSumStringModel()
                {
                    FrilanserName = frilanserOfClient.UserName,
                    SummOfStrings = await _repository.SumStringFrilanserToClientAsync(id, frilanserOfClient.Id)
                }

                    );
            }

            IdentityUser client = await _userManager.FindByIdAsync(id);

            ClientDetailsViewModel clientDetailsViewModel = new ClientDetailsViewModel()
            {
                ClientName = client.UserName,
                FrilanserSumStrings = frilanserSumStrings

            };

            return View(clientDetailsViewModel);
        }

        //Get Wallet
        public IActionResult Wallet()
        {
            WalletViewModel walletViewModel = new WalletViewModel();

            string address = _usersWalletRepository.getWalletAddressById(3);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay([Bind("FrilanserId, Period, Amount,Content,BlogId")] FrilanserMonthStringsModel frilanserMonthlyPayment)
        {
            IdentityUser frilanser = await _userManager.FindByIdAsync(frilanserMonthlyPayment.FrilanserId);
            if (_usersWalletRepository.isUserHasAddressIdentity(frilanser))
            {
                List<string> FromAddresses = new List<string>();
                List<string> ToAddresses = new List<string>();
                List<string> Amounts = new List<string>();
                FromAddresses.Add(_usersWalletRepository.getWalletAddressById(3));
                ToAddresses.Add(_usersWalletRepository.GetUsersWalletAddressIdentity(frilanser));
                Amounts.Add(frilanserMonthlyPayment.Amount.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")));
                Payments pc = new Payments();
                PaymentsToFrilanserModel paymentsToFrilanser = new PaymentsToFrilanserModel();
                try
                {
                    APIResponse response = _repository.withdrawFromAddresses(pc, FromAddresses, ToAddresses, Amounts);
                    paymentsToFrilanser.Status = true;
                    paymentsToFrilanser.Amount = frilanserMonthlyPayment.Amount;
                    paymentsToFrilanser.Frilanser = frilanser;
                    paymentsToFrilanser.Period = DateTime.Parse(frilanserMonthlyPayment.Period);
                    _repository.CreatePaymentInvoiceToFrilanser(paymentsToFrilanser);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            else return RedirectToAction(nameof(Index));
        }
    }
}