using Cysharp.Threading.Tasks;
using BlackSmith.Domain.Networking.Auth;

#nullable enable

namespace BlackSmith.Usecase.Interface.Networking.CloudSave
{
    /// <summary>
    /// 位置情報を表すモデル
    /// </summary>
    public record PositionModel
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float Z { get; init; }
    }

    /// <summary>
    /// 位置情報の永続化を管理するインターフェース
    /// </summary>
    public interface IPositionSaveController
    {
        /// <summary>
        /// 位置情報を保存する
        /// </summary>
        /// <param name="position">保存する位置情報</param>
        /// <returns>保存処理のタスク</returns>
        UniTask SaveAsync(PositionModel position);

        /// <summary>
        /// 保存されている位置情報を取得する
        /// </summary>
        /// <returns>保存されている位置情報（存在しない場合はnull）</returns>
        UniTask<PositionModel?> LoadAsync();
    }
}