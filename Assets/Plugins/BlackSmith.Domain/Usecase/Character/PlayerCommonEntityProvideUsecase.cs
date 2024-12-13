using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class PlayerCommonEntityProvideUsecase
    {
        /// <summary>
        /// Commandを用いて再構築を行う, リポジトリへの登録は行わない
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static PlayerCommonEntity BuildCommonEntity(PlayerCommonReconstructCommand command)
        {
            return PlayerFactory.Reconstruct(command);
        }
    }
}
