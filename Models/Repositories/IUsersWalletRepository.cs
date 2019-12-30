using Microsoft.AspNetCore.Identity;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProsjektoppgaveITE1811Gruppe7.Models
{
    public interface IUsersWalletRepository
    {
        Task<string> GetUsersWalletAddress(IPrincipal principal);

        string GetUsersWalletAddressIdentity(IdentityUser identitiUser);

        string getWalletAddressById(int id);

        void CreateWalletAddress(UsersWalletsModel wallet);

        Task<bool> isUserHasAddress(IPrincipal principal);

        bool isUserHasAddressIdentity(IdentityUser principal);

    }
}
