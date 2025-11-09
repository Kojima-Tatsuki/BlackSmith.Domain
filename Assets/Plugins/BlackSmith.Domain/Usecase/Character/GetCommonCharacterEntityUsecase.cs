using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;

#nullable enable

namespace BlackSmith.Domain.Usecase.Character
{
    public class GetCommonCharacterEntityUsecase
    {
        private readonly ICommonCharacterEntityRepository repository;

        public GetCommonCharacterEntityUsecase(ICommonCharacterEntityRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// CharacterIdからCommonCharacterEntityを取得する
        /// </summary>
        /// <param name="characterId">キャラクターID</param>
        /// <returns>CommonCharacterEntity</returns>
        public async UniTask<CommonCharacterEntity?> FindCommonCharacterEntityAsync(CharacterID characterId)
        {
            

            return await repository.FindByID(characterId);
        }
    }
}