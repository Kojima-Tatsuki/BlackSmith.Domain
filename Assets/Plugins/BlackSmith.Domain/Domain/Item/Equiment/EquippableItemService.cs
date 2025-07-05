using BlackSmith.Domain.Character;
using System;

namespace BlackSmith.Domain.Item.Equipment
{
    // ドメインサービス. Usecaseから呼び出すことを想定するため、public
    public class EquipmentEnhanceService
    {
        /// <summary>
        /// 強化を行う
        /// </summary>
        /// <param name="item">強化対象のアイテム</param>
        /// <param name="enchanceType">追加付与を試みるパラメータ</param>
        /// <param name="parameters">強化を試みるキャラクターのステータス</param>
        /// <returns>強化結果</returns>
        internal EnhancedResult Enhance(EquippableItem item, EnhancementParameter.EnhanceType enchanceType, DependentParametersForEnhancement parameters)
        {
            var random = new Random();

            var rand = random.NextDouble(); // 0.0 ~ 1.0 の値を取得する

            var success = rand <= GetSuccessProbabilityWhenEnhancement(item, parameters);

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

        /// <summary>強化時の成功確率</summary>
        /// <returns>確率は 0.0 ~ 1.0 で返される</returns>
        internal float GetSuccessProbabilityWhenEnhancement(EquippableItem item, DependentParametersForEnhancement parameters)
        {
            var baseLevel = Math.Max(item.RequireParameter.Level.Value - 5, 0);

            var diff = Math.Min(parameters.CharacterLevel.Value - baseLevel, 5);

            var result = MathF.Min(90 - 10 * item.EnhancementParameter.GetEnhancedCount + diff, 100) / 100;

            return result;
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

        /// <summary>強化の際に依存するアイテム以外のパラメータ</summary>
        public class DependentParametersForEnhancement
        {
            public CharacterLevel CharacterLevel { get; }

            internal DependentParametersForEnhancement(CharacterLevel characterLevel)
            {
                CharacterLevel = characterLevel;
            }
        }
    }
}
