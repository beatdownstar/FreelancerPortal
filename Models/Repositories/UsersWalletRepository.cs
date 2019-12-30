using Microsoft.AspNetCore.Identity;
using ProsjektoppgaveITE1811Gruppe7.Data;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{  
    public class UsersWalletRepository : IUsersWalletRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _manager;

        public UsersWalletRepository(UserManager<IdentityUser> manager, ApplicationDbContext db)
        {
            _db = db;
            _manager = manager;
        }

        public async Task<string> GetUsersWalletAddress(IPrincipal principal)
        {
            IdentityUser user = await _manager.FindByNameAsync(principal.Identity.Name);

            string userAddress = _db.UsersWallets.Where(uw => uw.User == user)
                                   .Select(uw => uw.Address)
                                   .Single();
            return userAddress;
        }
        
        public string GetUsersWalletAddressIdentity(IdentityUser identitiUser)
        {
           string userAddress = _db.UsersWallets.Where(uw => uw.User == identitiUser)
                                   .Select(uw => uw.Address)
                                   .Single();
            return userAddress;
        }
        
        //For HIN Empoyees and all Clients
        public string getWalletAddressById(int id)
        {
            return _db.UsersWallets.Where(uw => uw.Id == id)
                                    .Select(uw => uw.Address)
                                    .Single();
        }

        public void CreateWalletAddress(UsersWalletsModel wallet)
        {
            _db.UsersWallets.Add(wallet);
            _db.SaveChanges();
        }

        public async Task<bool> isUserHasAddress(IPrincipal principal)
        {
            IdentityUser user = await _manager.FindByNameAsync(principal.Identity.Name);
            return _db.UsersWallets.Select(uw => uw.User).Contains(user);
        }

        public bool isUserHasAddressIdentity(IdentityUser identityUser)
        {            
            return _db.UsersWallets.Select(uw => uw.User).Contains(identityUser);
        }
    }
}




