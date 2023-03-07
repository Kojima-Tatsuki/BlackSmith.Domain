using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Item.Service
{
    class EquipmentEnchanceService
    {
        // 強化を行う
        public EnchanceResult Enchance(EquippableItem item, EnchancementParameter.EnchanceType enchanceType, DependentParametersForEnhancement parameters)
        {
            var random = new Random();

            var rand = random.NextDouble(); // 0.0 ~ 1.0 の値を取得する

            var success = rand <= item.GetSuccessProbabilityWhenEnchancement(parameters);

            if (success)
            {
                var enchance = item.EnchancementParameter.AddEnchance(enchanceType);

                return new EnchanceResult(
                    EnchanceResultType.Success,
                    item.EditEnchancementParam(enchance));
            }

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
