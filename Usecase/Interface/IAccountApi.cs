using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Player;

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
        public string UserId { get; } // Gs2が作成するID
        public string Password { get; } // Gs2が作成するパスワード

        public PlayerName UserName { get; } // Userが指定する名前
    }
}
