using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Item
{
    // ドメインサービス. Usecasekから呼び出すことを想定するため、public
    public class EquipmentEnchanceService
    {
        /// <summary>
        /// 強化を行う
        /// </summary>
        /// <param name="item">強化対象のアイテム</param>
        /// <param name="enchanceType">追加付与を試みるパラメータ</param>
        /// <param name="parameters">強化を試みるプレイヤーのステータス</param>
        /// <returns>強化結果</returns>
        public EnchanceResult Enchance(EquippableItem item, EnchancementParameter.EnchanceType enchanceType, DependentParametersForEnhancement parameters)
        {
            var random = new Random();

            var rand = random.NextDouble(); // 0.0 ~ 1.0 の値を取得する

            var success = rand <= item.GetSuccessProbabilityWhenEnchancement(parameters);

            if (success)
            {
                // 強化を行うロジックはアイテムにある
                var enchance = item.EnchancementParameter.AddEnchance(enchanceType);

                return new EnchanceResult(
                    EnchanceResultType.Success,
                    item.EditEnchancementParam(enchance));
            }

            // 強化に失敗しても、武器の付与パラメータが減少することはない

            return new EnchanceResult(EnchanceResultType.Endure, item);
        }

        public class EnchanceResult
        {
            public EnchanceResultType ResultType { get; }
            public EquippableItem EquippableItem { get; }

            internal EnchanceResult(EnchanceResultType resultType, EquippableItem equippableItem)
            {
                ResultType = resultType;
                EquippableItem = equippableItem;
            }
        }

        public enum EnchanceResultType
        {
            Success, // 成功
            Endure, // 現状維持
            Failure, // 失敗
        }
    }
}
