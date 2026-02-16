#nullable enable

namespace BlackSmith.Domain.Usecase.Interface.Networking.Connection
{
    using Domain.Networking.Connection;
    using Domain.Networking.Auth;
    using Domain.Character;

    /// <summary>
    /// 接続承認を処理するインターフェース
    /// </summary>
    public interface IConnectionApprovalHandler
    {
        /// <summary>
        /// 接続承認を実行
        /// </summary>
        ConnectionApprovalResult ApproveConnection(
            AuthPlayerId playerId,
            CharacterName characterName);
    }
}
