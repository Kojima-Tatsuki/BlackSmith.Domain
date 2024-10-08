﻿using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// プレイヤーのステータスに変更を与える際に用いるユースケース
    /// </summary>
    public class RewritePlayerCommonEntityStatusUsecase
    {
        private readonly IPlayerCommonEntityRepository repository;

        public RewritePlayerCommonEntityStatusUsecase(IPlayerCommonEntityRepository playerRepository)
        {
            repository = playerRepository;
        }

        /// <summary>
        /// プレイヤーの名前を書き換える
        /// </summary>
        /// <param name="id">書き換えるプレイヤーのID</param>
        /// <param name="newName">書き換え先の名前</param>
        public async UniTask RenamePlayer(CharacterID id, string newName)
        {
            var name = new PlayerName(newName);

            var entity = (await repository.FindByID(id)) as ICharacterEntity ?? throw new ArgumentException($"指定したidのキャラクターは存在しません, {id}");

            entity.ChangeName(name);

            await repository.UpdateCharacter((entity as PlayerCommonEntity) ?? throw new InvalidOperationException(nameof(entity)));
        }

        public static bool IsValidPlayerName(string name)
        {
            return PlayerName.IsValid(name);
        }
    }
}