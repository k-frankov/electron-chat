using System.Threading.Tasks;
using ElectronChatCosmosDB.Entities;

namespace ElectronChatCosmosDB.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity> CreateUser(UserEntity userEntity);
        Task<UserEntity> GetUserByUserName(string userName);
    }
}
