using Microsoft.AspNetCore.Identity;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using ProsjektoppgaveITE1811Gruppe7.Controllers;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{
    public interface IOrderRepository
    {
        
        IEnumerable<Order> GetAll();

        Task<int> CreateNewOrder(Order order, IPrincipal principal);

        Task<IEnumerable<Order>> GetClientsOrdersAsync(IPrincipal principal);

        Task<IEnumerable<Order>> GetFrilansersOrdersAsync(IPrincipal principal);

        Order GetOrdersById(int orderId);

        void saveSolution(Order order);

        FileModel AddFile(FileModel file);

        

        FileModel GetFileById(int id);

        Task<List<Order>> GetFrilanserOrdersAsync(string id);

        //informasjon om hvor mange linjer kode en frilanser har skrevet for en gitt kunde
        Task<int> SumStringFrilanserToClientAsync(string clientId, string frilanserId);

        //hvor mange linjer kode Frilanser har skrevet per måned
        Task<int> SumStringInMonthForFrilanserAsync(string frilanserId, DateTime monthYear);

        //hvor mange linjer kode Frilanser har skrevet per måned for kunder
        Task<int> SumStringInMonthForFrilanserToClientAsync(string clientId, string frilanserId, DateTime monthYear);

        //hvem som har jobbet for en kunde
        Task<List<IdentityUser>> FrilansersOfClientAsync(string clientId);

        //List of Clients for Frilanser
        Task<List<IdentityUser>> ListOfClientsForFrilanser(string frilanserId);

        // Dates when frilanser worked
        Task<List<DateTime>> DatesFrilanserWorkedAsync(string frilanserId);

        void UpdateOrder(Order order);

        void CreateInvoice(InvoiceModel invoice);

        Task<List<InvoiceModel>> GetClientsInvoicesAsync(string clientName);

        string GetClientsNameByOrderId(int id);

        string GetFrilansersNameByOrderId(int id);

        IEnumerable<Order> GetOrdersWithStatus(int status);

        APIResponse getAddress(Payments payments);

        APIResponse getBalance(Payments payments, string type, List<string> addresses);

        APIResponse getTransactions(Payments payments, string type, List<string> addresses);

        Boolean getSucceeded(IdentityResult result);

        IEnumerable<IdentityError> getErrors(IdentityResult result);

        FileStream getFilestream(string path, FileMode filemode);

        int getNumberOfStrings(string path);

        List<string> getSolutionStrings(string path);

        void UpdateInvoice(InvoiceModel invoice);


        InvoiceModel GetInvoice(int orderId);


        Task<bool> PaymentStatusToFrilanserInPeriode(string id, DateTime periode);
        int CreatePaymentInvoiceToFrilanser(PaymentsToFrilanserModel paymentsToFrilanser);

        APIResponse withdrawFromAddresses(Payments payments, List<string> FromAddresses, List<string> ToAddresses,
            List<string> Amounts);
    }
}