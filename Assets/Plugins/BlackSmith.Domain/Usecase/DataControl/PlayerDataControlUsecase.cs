using BlackSmith.Domain.Usecase.Networking.Auth;
using BlackSmith.Usecase.Interface;
using BlackSmith.Usecase.Interface.Networking.Auth;

// AuthPlayerIdからCharacterEntityを取得するユースケース
namespace BlackSmith.Domain.Usecase.DataControl
{
    public class PlayerDataControlUsecase
    {
        private readonly IAuthenticationController authController;
        private readonly ICommonCharacterEntityRepository characterRepository;
        private readonly ISessionPlayerDataRepository sessionRepository;
    }
}