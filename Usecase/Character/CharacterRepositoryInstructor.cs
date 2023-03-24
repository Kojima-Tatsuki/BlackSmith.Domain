using System;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.NonPlayer;
using BlackSmith.Repository.Interface;

#nullable enable

namespace BlackSmith.InterfaceAdapter.GateWay
{
    /// <summary>
    /// キャラクターをキャラクターリポジトリに登録する
    /// </summary>
    internal class CharacterRepositoryInstructor
    {
        private readonly ICharacterRepositoty characterRepositoty;

        public CharacterRepositoryInstructor(ICharacterRepositoty repositoty)
        {
            characterRepositoty = repositoty;
        }

        public void Register(NonPlayerEntity character)
        {
            if (character is null)
                throw new ArgumentNullException(nameof(character));

            characterRepositoty.Register(character);
        }

        public NonPlayerEntity? GetEnitty(CharacterID id)
        {
            var character = characterRepositoty.FindByID(id);
            if (character is null)
                return null;

            return character;
        }

        public void UpdateCharacterStatus(NonPlayerEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var character = characterRepositoty.FindByID(entity.ID);
            if (character is null)
                throw new ArgumentException("そのIDのキャラクターは存在しません");

            characterRepositoty.UpdateCharacter(character);
        }

        public bool IsExist(CharacterID id) => characterRepositoty.IsExist(id);

        public void Delete(CharacterID id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            if (characterRepositoty.IsExist(id))
                characterRepositoty.Delete(id);
        }
    }
}