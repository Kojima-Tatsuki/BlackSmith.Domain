﻿using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using System;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// プレイヤーのステータスに変更を与える際に用いるユースケース
    /// </summary>
    public class RewritePlayerStatusUsecase
    {
        private readonly PlayerRepositoryInstructor instructor;

        public RewritePlayerStatusUsecase(IPlayerRepository playerRepository)
        {
            var repository = playerRepository;

            instructor = new PlayerRepositoryInstructor(repository);
        }

        /// <summary>
        /// プレイヤーの名前を書き換える
        /// </summary>
        /// <param name="id">書き換えるプレイヤーのID</param>
        /// <param name="newName">書き換え先の名前</param>
        public void RenamePlayer(CharacterID id, string newName)
        {
            var name = new PlayerName(newName);

            var entity = instructor.GetPlayerEntity(id) as ICharacterEntity;

            entity.ChangeName(name);

            instructor.UpdatePlayerEntity((entity as PlayerEntity) ?? throw new InvalidOperationException(nameof(entity)));
        }

        public static bool IsValidPlayerName(string name)
        {
            return PlayerName.IsValid(name);
        }
    }
}