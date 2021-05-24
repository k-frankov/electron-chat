using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;

namespace ElectronChatCosmosDB.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity> CreateUserAsync(UserEntity userEntity);
        Task<UserEntity> GetUserByUserNameAsync(string userName);
    }
}
