using BlackSmith.Domain.Input;

namespace BlackSmith.Usecase.Character.Player.Move
{
    public interface IPlayerMoveUsecase
    {
        /// <summary>
        /// Playerを動かす
        /// </summary>
        /// <param name="actionCode"></param>
        void MovePlayer(MoveActionCode actionCode);
    }

    public interface PlayerMoveUsecaseOutput
    {
    }

    public class PlayerMoveUsecase : IPlayerMoveUsecase
    {
        // private readonly PlayerMoveUsecaseOutput moveOutput;

        public PlayerMoveUsecase()
        {
        }

        public void MovePlayer(MoveActionCode actionCode)
        {
        }
    }
}