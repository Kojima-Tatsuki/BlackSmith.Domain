using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Networking.Auth;
using BlackSmith.Usecase.Interface.Networking.Auth;
using System;
using System.Collections.Generic;
using BlackSmith.Usecase.Character;
using BlackSmith.Domain.Character;
using BlackSmith.Usecase.Interface;
using BlackSmith.Domain.Usecase.Networking.Auth;

#nullable enable

namespace BlackSmith.Usecase.Networking.Auth
{
    /// <summary>
    /// 認証に関するユースケース
    /// ユーザー名とパスワードによるサインイン/サインアップ処理を提供する
    /// </summary>
    public class AuthenticationUsecase
    {
        private readonly IAuthenticationController authController;
        private readonly ICommonCharacterEntityRepository characterRepository;
        private readonly ISessionPlayerDataRepository sessionRepository;
        private readonly IPlayerCharacterIdResolver playerCharacterIdResolver;

        public AuthenticationUsecase(IAuthenticationController authController, ICommonCharacterEntityRepository characterRepository, ISessionPlayerDataRepository sessionRepository, IPlayerCharacterIdResolver playerCharacterIdResolver)
        {
            this.authController = authController ?? throw new ArgumentNullException(nameof(authController));
            this.characterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            this.playerCharacterIdResolver = playerCharacterIdResolver ?? throw new ArgumentNullException(nameof(playerCharacterIdResolver));
        }

        /// <summary>
        /// ユーザー名とパスワードでサインアップする
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証されたプレイヤーID</returns>
        /// <exception cref="ArgumentNullException">引数がnullの場合</exception>
        /// <exception cref="InvalidOperationException">すでに認証済みの場合、またはサインアップに失敗した場合</exception>
        public async UniTask<AuthPlayerId> SignUpAsync(UserName userName, Password password)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));

            if (authController.IsSignedIn())
                throw new InvalidOperationException("既に認証済みです。サインアップ前にサインアウトしてください。");

            try
            {
                var playerId = await authController.SignUpForUserNameAndPassword(userName, password);

                var adjustUsecase = new AdjustCommonCharacterEntityUsecase(characterRepository);
                var characterEntity = await adjustUsecase.CreateCharacter(new CharacterName(userName.Value));

                sessionRepository.Update(new SessionPlayerData(playerId, characterEntity.ID, characterEntity.Name));
                return playerId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"サインアップに失敗しました: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ユーザー名とパスワードでサインインする
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証されたプレイヤーID</returns>
        /// <exception cref="ArgumentNullException">引数がnullの場合</exception>
        /// <exception cref="InvalidOperationException">すでに認証済みの場合、またはサインインに失敗した場合</exception>
        public async UniTask<AuthPlayerId> SignInAsync(UserName userName, Password password)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));

            if (authController.IsSignedIn())
                throw new InvalidOperationException("既に認証済みです。サインイン前にサインアウトしてください。");

            try
            {
                var playerId = await authController.SignInForUserNameAndPassword(userName, password);

                var characterId = await playerCharacterIdResolver.GetCharacterIdByPlayerAuthId(playerId);
                if (characterId == null)
                    throw new InvalidOperationException("サインインに失敗しました。対応するキャラクターが見つかりません。");
                
                var characterEntity = await characterRepository.FindByID(characterId);
                if (characterEntity == null)
                    throw new InvalidOperationException("サインインに失敗しました。対応するキャラクターが見つかりません。");

                sessionRepository.Update(new SessionPlayerData(playerId, characterEntity.ID, characterEntity.Name));

                return playerId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"サインインに失敗しました: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// サインアウトする
        /// </summary>
        /// <exception cref="InvalidOperationException">認証されていない場合</exception>
        public async UniTask SignOutAsync()
        {
            if (!authController.IsSignedIn())
                throw new InvalidOperationException("認証されていません。サインアウトできません。");

            try
            {
                await authController.SignOutForAccount();
                sessionRepository.Logout();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"サインアウトに失敗しました: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 現在サインイン中かどうかを取得する
        /// </summary>
        public bool IsSignedIn => authController.IsSignedIn();

        /// <summary>
        /// 現在のプレイヤーIDを取得する
        /// </summary>
        public AuthPlayerId? GetCurrentPlayerId() => authController.GetPlayerId();

        /// <summary>
        /// ユーザー名のバリデーション結果を取得する
        /// </summary>
        /// <param name="userName">検証するユーザー名</param>
        /// <returns>バリデーションエラーのリスト</returns>
        public static IReadOnlyList<UserName.Validator.ValidationError> ValidateUserName(string userName)
        {
            return UserName.Validator.ValidateUserName(userName);
        }

        /// <summary>
        /// パスワードのバリデーション結果を取得する
        /// </summary>
        /// <param name="password">検証するパスワード</param>
        /// <returns>バリデーションエラーのリスト</returns>
        public static IReadOnlyList<Password.Validator.ValidationError> ValidatePassword(string password)
        {
            return Password.Validator.ValidatePassword(password);
        }
    }
}