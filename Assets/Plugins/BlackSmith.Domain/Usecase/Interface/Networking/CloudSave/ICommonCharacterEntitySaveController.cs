using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.Networking.Auth;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.CloudSave
{
    /// <summary>
    /// CommonCharacterEntityの永続化を管理するインターフェース
    /// </summary>
    public interface ICommonCharacterEntitySaveController
    {
        /// <summary>
        /// CommonCharacterEntity情報を保存する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <param name="reconstructCommand">保存するキャラクター再構築コマンド</param>
        /// <returns>保存処理のタスク</returns>
        UniTask SaveCommonCharacterEntityAsync(AuthPlayerId authPlayerId, CommonCharacterReconstructCommand reconstructCommand);

        /// <summary>
        /// 保存されているCommonCharacterEntity情報を取得する
        /// </summary>
        /// <param name="authPlayerId">認証されたプレイヤーID</param>
        /// <returns>保存されているキャラクター再構築コマンド（存在しない場合はnull）</returns>
        UniTask<CommonCharacterReconstructCommand?> LoadCommonCharacterEntityAsync(AuthPlayerId authPlayerId);
    }
}