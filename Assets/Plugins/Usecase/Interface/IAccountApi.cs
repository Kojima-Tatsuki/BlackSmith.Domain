using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Usecase.Interface
{
    public interface IAccountApi
    {
        /// <summary>Gs2とGoogleCloudの両方を通った後の状態が帰る</summary>
        /// <returns></returns>
        public UniTask<UserAccount> CreateAccountAsync(PlayerName name);
    }

    public class UserAccount
    {
        public string UserId { get; init; } // Gs2が作成するID
        public string Password { get; init; } // Gs2が作成するパスワード

        public PlayerName UserName { get; init; } // Userが指定する名前

        public UserAccount(string id, string password, string userName)
        {
            UserId = id;
            Password = password;
            UserName = new(userName);
        }
    }
}
