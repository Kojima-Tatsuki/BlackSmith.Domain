using BlackSmith.Domain.Usecase.Networking.Auth;
using BlackSmith.Usecase.Interface;
using BlackSmith.Usecase.Interface.Networking.Auth;

#nullable enable

// AuthPlayerIdからCharacterEntityを取得するユースケース
namespace BlackSmith.Domain.Usecase.DataControl
{
    public class PlayerDataControlUsecase
    {
        private readonly IAuthenticationController authController;
        private readonly ICommonCharacterEntityRepository characterRepository;
        private readonly ISessionPlayerDataRepository sessionRepository;

        public PlayerDataControlUsecase(IAuthenticationController authController, ICommonCharacterEntityRepository characterRepository, ISessionPlayerDataRepository sessionRepository)
        {
            this.authController = authController;
            this.characterRepository = characterRepository;
            this.sessionRepository = sessionRepository;
        }
    }
}