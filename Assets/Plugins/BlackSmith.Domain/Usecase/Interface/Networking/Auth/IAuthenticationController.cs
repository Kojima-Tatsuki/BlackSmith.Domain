using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Networking.Auth;
using System;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Auth
{
    public interface IAuthenticationController
    {
                /// <summary>
        /// ユーザー名とパスワードでサインアップを実行
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <returns>サインアップ処理の完了</returns>
        UniTask<AuthPlayerId> SignUpForUserNameAndPassword(UserName userName, Password password);

        /// <summary>
        /// ユーザー名とパスワードでサインインを実行
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <returns>サインイン処理の完了</returns>
        UniTask<AuthPlayerId> SignInForUserNameAndPassword(UserName userName, Password password);

        /// <summary>
        /// アカウントからサインアウトを実行
        /// </summary>
        UniTask SignOutForAccount();

        bool IsSignedIn();

        /// <summary>
        /// 現在ログイン中のプレイヤーIDを取得
        /// </summary>
        /// <returns>プレイヤーID（未ログインの場合はnull）</returns>
        AuthPlayerId? GetPlayerId();
    }
}