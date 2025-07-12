using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Networking.Auth;
using System;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.Auth
{
    public interface IAuthenticationController
    {
        UniTask<AuthPlayerId> SignupForUserNameAndPassword(UserName userName, Password password);

        UniTask<AuthPlayerId> SignInForUserNameAndPassword(UserName userName, Password password);

        UniTask SignOutForAccount();

        bool IsSignedIn();

        AuthPlayerId? GetPlayerId();
    }
}