using System;
using System.Collections.Generic;
using BlackSmith.Domain.CharacterObjects;

namespace BlackSmith.Domain.Player
{
    internal class PlayerCharacterService
    {
        // キャラクターに対して対象の装備を装備させる
        // 既に装備していた場合には、エラー処理をする
        internal void Equip(PlayerEntity player, Equipment equip)
        {
            /*
            プレイヤーの装備欄に指定の装備が装備できるなら、装備する
            この時、装備できるかの条件はプレイヤーのドメインオブジェクト内に記載すべき
            -> ここでは、装備できるか という bool を返す関数を実行すべき

            装備した時、プレイヤーのステータスは、その装備の影響を受ける

            また、装備した際に新たにプレイヤーに加えられる重量によって
            プレイヤーにデバフが生じるかの確認を行う

            ドメインサービスの形で実装しようとしているのは、
            プレイヤーの装備がリポジトリに入るのか、
            プレイヤーのエンティティに入るのか未決定だからである

            プレイヤーのエンティティが自らの状態異常やインベントリ情報、装備している装備など、
            すべての情報を保持していればいいのだが、それはデータを分けるということからも、
            あまり望ましくない可能性が高い
            */
        }
    }
}
