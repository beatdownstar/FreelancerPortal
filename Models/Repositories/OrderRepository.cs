using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProsjektoppgaveITE1811Gruppe7.Data;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using ProsjektoppgaveITE1811Gruppe7.Controllers;


namespace ProsjektoppgaveITE1811Gruppe7.Models
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _manager;

        public OrderRepository(UserManager<IdentityUser> manager, ApplicationDbContext db)
        {
            _db = db;
            _manager = manager;
        }
       

        public IEnumerable<Order> GetAll()
        {

            IEnumerable<Order> orders = _db.Orders.ToList();

            return orders;
        }

        public async Task<IEnumerable<Order>> GetClientsOrdersAsync(IPrincipal principal)
        {

            IdentityUser currentClient = await _manager.FindByNameAsync(principal.Identity.Name);

            IEnumerable<Order> orders = _db.Orders.Where(c => c.Client == currentClient).Include(fl => fl.Frilanser).ToList();

            return orders;
        }

        public async Task<List<Order>> GetFrilanserOrdersAsync(string id)
        {
            IdentityUser currentFrilanser = await _manager.FindByIdAsync(id);

            List<Order> orders = _db.Orders.Where(c => c.Frilanser == currentFrilanser).ToList();

            return orders;
        }

        public Order GetOrdersById(int orderId)
        {

            Order order = _db.Orders.Include(sf => sf.SolutionFile)
                                    .Include(c => c.Client)
                                    .Include(su => su.SeniorUtvikler)
                                    .Include(fl => fl.Frilanser)
                                    .Include(r => r.SpecializationRole)
                                    .FirstOrDefault(o => o.OrderId == orderId);


            return order;
        }

        public async Task<int> CreateNewOrder(Order order, IPrincipal principal)
        {
            IdentityUser currentUser = await _manager.FindByNameAsync(principal.Identity.Name);
            order.DateCreated = DateTime.Now;
            order.Client = currentUser;
            //_manager.GetUsersInRoleAsync
            _db.Orders.Add(order);
            _db.SaveChanges();
            return order.OrderId;
        }

        public async Task<IEnumerable<Order>> GetFrilansersOrdersAsync(IPrincipal principal)
        {
            
            IdentityUser currentUser = await _manager.FindByNameAsync(principal.Identity.Name);
            var userRoles = await _manager.GetRolesAsync(currentUser);
            userRoles.Remove("Frilanser");
            
            IEnumerable<Order> orders = _db.Orders.Where(o => o.Frilanser == currentUser || (o.Frilanser==null && ( o.SpecializationRole.Name == "Universal (Specialization)" || userRoles.Contains(o.SpecializationRole.Name))))
             //   IEnumerable<Order> orders = _db.Orders.Where(o => o.Frilanser == currentUser || (o.Frilanser == null && (_manager.IsInRoleAsync(currentUser, o.SpecializationRole.Name).Result))) // || o.SpecializationRole.Name == "Universal (Secialization)")))
                                                  .Include(o => o.SpecializationRole)
                                                  .Include(o => o.Frilanser)
                                                  .Include(o => o.Client)
                                                  .ToList();
            
            return orders;
        }

        

        public void saveSolution(Order order)
        {
            _db.Orders.Update(order);
            _db.SaveChanges();
        }
        public void UpdateOrder(Order order)
        {
            _db.Orders.Update(order);
            _db.SaveChanges();
        }

        public FileModel AddFile(FileModel file)
        {
            
            _db.Files.Add(file);
            _db.SaveChanges();

            return file;
        }


        public FileModel GetFileById(int id)
        {
            FileModel file = _db.Files.FirstOrDefault(f => f.Id == id);
            return file;

        }

        //informasjon om hvor mange linjer kode en frilanser har skrevet for en gitt kunde
        public async Task<int> SumStringFrilanserToClientAsync(string clientId, string frilanserId)
        {
            int sum = 0;
            IdentityUser client = await _manager.FindByIdAsync(clientId);
            IdentityUser frilanser = await _manager.FindByIdAsync(frilanserId);
            if (client != null || frilanser != null)
            {
                sum = _db.Orders.Where(cl => cl.Client == client)
                                .Where(status => status.Status == 4)
                                .Where(fr => fr.Frilanser == frilanser)
                                .Select(str => str.NumberOfStrings)
                                .Distinct()
                                .Sum();
            }

            return sum;
        }

        //hvor mange linjer kode Frilanser har skrevet per måned 
        public async Task<int> SumStringInMonthForFrilanserAsync(string frilanserId, DateTime monthYear)
        {
            int sum = 0;
            IdentityUser frilanser = await _manager.FindByIdAsync(frilanserId);
            if (frilanser != null)
            {
                sum = _db.Orders.Where(frl => frl.Frilanser == frilanser)
                                .Where(status => status.Status == 4)
                                .Where(datofdeadline => datofdeadline.DateOfDeadline.ToString("MM.yyyy") == monthYear.ToString("MM.yyyy"))
                                .Select(str => str.NumberOfStrings)
                                .Distinct()
                                .Sum();
            }
            return sum;
        }

        //hvor mange linjer kode Frilanser har skrevet per måned for kunder
        public async Task<int> SumStringInMonthForFrilanserToClientAsync(string clientId, string frilanserId, DateTime monthYear)
        {
            int sum = 0;
            IdentityUser client = await _manager.FindByIdAsync(clientId);
            IdentityUser frilanser = await _manager.FindByIdAsync(frilanserId);
            if (client != null || frilanser != null)
            {
                sum = _db.Orders.Where(frl => frl.Frilanser == frilanser)
                                .Where(cl => cl.Client == client)
                                .Where(status => status.Status == 4)
                                .Where(datofdeadline => datofdeadline.DateOfDeadline.ToString("MM.yyyy") == monthYear.ToString("MM.yyyy"))
                                .Select(str => str.NumberOfStrings)
                                .Distinct()
                                .Sum();
            }
            return sum;
        }

        //hvem som har jobbet for en kunde
        public async Task<List<IdentityUser>> FrilansersOfClientAsync(string clientId)
        {
            List<IdentityUser> frilansersOfClient = new List<IdentityUser>();
            IdentityUser client = await _manager.FindByIdAsync(clientId);

            if (client != null)
            {
                frilansersOfClient = _db.Orders.Where(cl => cl.Client == client)
                                               .Where(f => f.Frilanser != null)
                                               .Select(frl => frl.Frilanser)
                                               .Distinct()
                                               .ToList();
            }
            return frilansersOfClient;
        }

        //List of Clients for Frilanser
        public async Task<List<IdentityUser>> ListOfClientsForFrilanser(string frilanserId)
        {
            List<IdentityUser> ClientsOfFrilanser = new List<IdentityUser>();
            IdentityUser frilanser = await _manager.FindByIdAsync(frilanserId);

            if (frilanser != null)
            {
                ClientsOfFrilanser = _db.Orders.Where(o => o.Frilanser == frilanser)
                                               .Select(o => o.Client)
                                               .Distinct()
                                               .ToList();
            }
            return ClientsOfFrilanser;
        }

        // Dates when frilanser worked
        public async Task<List<DateTime>> DatesFrilanserWorkedAsync(string frilanserId)
        {
            List<DateTime> datesFrilanserWorked = new List<DateTime>();
            IdentityUser frilanser = await _manager.FindByIdAsync(frilanserId);
            if (frilanser != null)
            {
                datesFrilanserWorked = _db.Orders.Where(frl => frl.Frilanser == frilanser)
                                                 .Where(status => status.Status == 4)
                                                 .Select(datesofdeadline => datesofdeadline.DateOfDeadline)
                                                 .Distinct()
                                                 .ToList();
            }

            return datesFrilanserWorked;
        }

        public void CreateInvoice(InvoiceModel invoice)
        {
            _db.Invoice.Add(invoice);
            _db.SaveChanges();
        }

        public void UpdateInvoice(InvoiceModel invoice)
        {            
            _db.Invoice.Update(invoice);
            _db.SaveChanges();
        }

        public InvoiceModel GetInvoice(int orderId)
        {
            Order order = _db.Orders.FirstOrDefault(o => o.OrderId == orderId);
            
            return _db.Invoice.FirstOrDefault(i => i.order == order);
        }

        public async Task<List<InvoiceModel>> GetClientsInvoicesAsync(string clientName)
        {
            IdentityUser client = await _manager.FindByNameAsync(clientName);
            List<InvoiceModel> invoices = _db.Invoice.Where(inv => inv.Status == false)
                                                     .Where(inv => inv.order.Client == client)
                                                     .Include(inv => inv.order)
                                                     .Include(inv => inv.order.Frilanser)
                                                     .ToList();

            return invoices;
        }

        public string GetClientsNameByOrderId (int id)
        {
            string clientsName = _db.Orders.Where(o => o.OrderId == id)
                                           .Select(o => o.Client)
                                           .Select(cl => cl.UserName)
                                           .ToString();           

            return clientsName;
        }

        public string GetFrilansersNameByOrderId(int id)
        {
            string frilansersName = _db.Orders.Where(o => o.OrderId == id)
                                           .Select(o => o.Frilanser)
                                           .Select(cl => cl.UserName)
                                           .ToString();

            return frilansersName;
        }

        public IEnumerable<Order> GetOrdersWithStatus(int status)
        {
            return _db.Orders.Where(o => o.Status == status)
                             .Include(o => o.Frilanser)
                             .Include(o => o.Client)
                             .ToList();
                                                 
        }

        public APIResponse getAddress(Payments payments)
        {
            return payments.getNewAddress();
        }

        public APIResponse getBalance(Payments payments, string type, List<string> addresses)
        {
            return payments.getAddressBalance(type, addresses);
        }

        public APIResponse getTransactions(Payments payments, string type, List<string> addresses)
        {
            return payments.getTransactions(type, addresses);
        }

        public Boolean getSucceeded(IdentityResult result)
        {
            return result.Succeeded;
        }

        public IEnumerable<IdentityError> getErrors(IdentityResult result)
        {
            return result.Errors;
        }

        public FileStream getFilestream(string path, FileMode filemode)
        {
            
            return new FileStream(path, filemode);

        }

        public int getNumberOfStrings(string path)
        {

            return System.IO.File.ReadAllLines(path).Length;

        }

        public List<string> getSolutionStrings(string path)
        {

            return System.IO.File.ReadAllLines(path).ToList();

        }
        public async Task<bool> PaymentStatusToFrilanserInPeriode(string id, DateTime periode)
        {
            IdentityUser frilancer = await _manager.FindByIdAsync(id);
           return _db.PaymentsToFrilansers.Where(p => p.Frilanser == frilancer && (p.Period.ToString("MM.yyyy") == periode.ToString("MM.yyyy"))).Count()>0;
                                    
        }

        public int CreatePaymentInvoiceToFrilanser(PaymentsToFrilanserModel paymentsToFrilanser)
        {
            _db.PaymentsToFrilansers.Add(paymentsToFrilanser);
            _db.SaveChanges();
            return paymentsToFrilanser.PaymentId;

        }

        public APIResponse withdrawFromAddresses(Payments payments, List<string> FromAddresses, List<string> ToAddresses, List<string> Amounts)
        {
            return payments.withdrawFromAddresses(FromAddresses, ToAddresses, Amounts);
        }

    }

}