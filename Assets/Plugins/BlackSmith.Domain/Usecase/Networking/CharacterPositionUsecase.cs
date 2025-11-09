using System;
using BlackSmith.Usecase.Interface.Networking.CloudSave;
using Cysharp.Threading.Tasks;

namespace BlackSmith.Usecase.Networking
{
    // このユースケースはあまり意味がない..
    // が、キャラクタの位置情報をどのようなロジックで取得するのか、
    // という観点は間違いなくドメインに属するため、一応ユースケースとして実装しておく
    public class CharacterPositionUsecase
    {
        private readonly IPositionSaveController positionSaveController;

        public CharacterPositionUsecase(IPositionSaveController positionSaveController)
        {
            this.positionSaveController = positionSaveController;
        }

        public async UniTask SavePositionAsync(PositionModel position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            await positionSaveController.SaveAsync(position);
        }

        public async UniTask<PositionModel> LoadPositionAsync()
        {
            var position = await positionSaveController.LoadAsync();
            if (position == null)
                return PositionModel.Zero;

            return position;
        }
    }
}