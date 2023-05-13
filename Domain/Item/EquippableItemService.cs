using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Item
{
    // ドメインサービス. Usecaseから呼び出すことを想定するため、public
    public class EquipmentEnhanceService
    {
        /// <summary>
        /// 強化を行う
        /// </summary>
        /// <param name="item">強化対象のアイテム</param>
        /// <param name="enchanceType">追加付与を試みるパラメータ</param>
        /// <param name="parameters">強化を試みるプレイヤーのステータス</param>
        /// <returns>強化結果</returns>
        public EnhancedResult Enhance(EquippableItem item, EnhancementParameter.EnhanceType enchanceType, DependentParametersForEnhancement parameters)
        {
            var random = new Random();

            var rand = random.NextDouble(); // 0.0 ~ 1.0 の値を取得する

            var success = rand <= item.GetSuccessProbabilityWhenEnhancement(parameters);

            if (success)
            {
                // 強化を行うロジックはアイテムにある
                var enhance = item.EnhancementParameter.AddEnhance(enchanceType);

                return new EnhancedResult(
                    EnhancedResultType.Success,
                    item.Enchant(enhance));
            }

            // 強化に失敗しても、武器の付与パラメータが減少することはない

            return new EnhancedResult(EnhancedResultType.Endure, item);
        }

        public class EnhancedResult
        {
            public EnhancedResultType ResultType { get; }
            public EquippableItem EquippableItem { get; }

            internal EnhancedResult(EnhancedResultType resultType, EquippableItem equippableItem)
            {
                ResultType = resultType;
                EquippableItem = equippableItem;
            }
        }

        public enum EnhancedResultType
        {
            Success, // 成功
            Endure, // 現状維持
            Failure, // 失敗
        }
    }
}
