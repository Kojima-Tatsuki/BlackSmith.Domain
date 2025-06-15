# BlackSmith.Domain ビジネスルール

## キャラクターシステム

### レベル・経験値システム

#### 基本ルール
- **最低レベル**: 1
- **経験値下限**: 0
- **レベルアップ条件**: 必要経験値に到達

#### 経験値計算式
```csharp
// 指数関数的成長システム
// 必要経験値 = I * (1 - A^(level-1)) / (1 - A)
// I: 初期値 = 100
// A: 倍率 = 1.25

private static readonly int InitExpRequirement = 100;
private static readonly float LevelDifferenceMultiplier = 1.25f;

public int GetRequiredExperience(int level)
{
    if (level <= 1) return 0;
    
    var multiplier = LevelDifferenceMultiplier;
    var numerator = InitExpRequirement * (1 - Math.Pow(multiplier, level - 1));
    var denominator = 1 - multiplier;
    
    return (int)(numerator / denominator);
}
```

#### レベル別必要経験値表
```
レベル  累計必要経験値  差分経験値
1       0              -
2       100            100
3       225            125  
4       381            156
5       576            195
10      2,441          488
20      47,684         2,384
50      117,737,499    5,898,137
```

### ステータスシステム

#### 基本能力値
- **Strength (STR)**: 筋力
- **Agility (AGI)**: 俊敏性
- **最低値**: 1（作成時・割り振り後も1未満にならない）
- **初期値**: 各1

#### ステータスポイント配分
```csharp
// レベルアップ時のステータスポイント獲得
public StatusPoint GetStatusPointOnLevelUp() => new StatusPoint(3);

// ステータス配分制約
public PlayerCommonEntity AllocateStatus(int strIncrease, int agiIncrease)
{
    if (strIncrease + agiIncrease > StatusPoint.Value)
        throw new InvalidOperationException("Insufficient status points");
    
    if (Strength.Value + strIncrease < 1 || Agility.Value + agiIncrease < 1)
        throw new InvalidOperationException("Status cannot be less than 1");
    
    // 配分実行
}
```

### 体力システム

#### 最大体力計算
```csharp
public int GetMaxHealth(PlayerLevel level) => level.Value * 10;
```

#### 体力回復・ダメージ
```csharp
public record CurrentHealth
{
    public int Value { get; }
    
    // ダメージ処理（最低1ダメージ保証）
    public CurrentHealth TakeDamage(int damage)
    {
        var actualDamage = Math.Max(1, damage);
        return new CurrentHealth(Math.Max(0, Value - actualDamage));
    }
    
    // 回復処理（最大値制限）
    public CurrentHealth Heal(int amount, int maxHealth)
    {
        return new CurrentHealth(Math.Min(maxHealth, Value + amount));
    }
}
```

## 戦闘システム

### 攻撃力・防御力計算

#### 基本計算式
```csharp
// 攻撃力 = レベル依存攻撃力 + 装備攻撃力 + ステータス効果攻撃力
public int CalculateAttackValue()
{
    var levelDependent = (Strength.Value + Agility.Value) * 2;
    var equipment = equipmentModule.GetAttackValue();
    var statusEffect = statusEffectModule.GetAttackValue();
    
    return levelDependent + equipment + statusEffect;
}

// 防御力も同様の計算
public int CalculateDefenseValue()
{
    var levelDependent = (Strength.Value + Agility.Value) * 2;
    var equipment = equipmentModule.GetDefenseValue();
    var statusEffect = statusEffectModule.GetDefenseValue();
    
    return levelDependent + equipment + statusEffect;
}
```

#### 装備による補正
```csharp
// 武器・防具・アクセサリの攻撃力合計
public int GetEquipmentAttackValue()
{
    var weaponAttack = weapon?.Enhancement.GetAttackValue() ?? 0;
    var armorAttack = armor?.Enhancement.GetAttackValue() ?? 0;
    var accessaryAttack = accessary?.Enhancement.GetAttackValue() ?? 0;
    
    return weaponAttack + armorAttack + accessaryAttack;
}
```

### ダメージ計算

#### 基本ダメージ式
```csharp
public int CalculateDamage(int attackValue, int defenseValue)
{
    var baseDamage = attackValue - defenseValue;
    return Math.Max(1, baseDamage);  // 最低1ダメージ保証
}
```

### 敵レベル差による経験値倍率

#### 倍率計算
```csharp
public float CalculateExperienceMultiplier(int playerLevel, int enemyLevel)
{
    var levelDifference = enemyLevel - playerLevel;
    
    // レベル差による倍率調整
    if (levelDifference > 0)
    {
        // 敵が強い場合: 倍率増加
        return 1.0f + (levelDifference * 0.1f);
    }
    else if (levelDifference < 0)
    {
        // 敵が弱い場合: 倍率減少（最低0.1倍）
        return Math.Max(0.1f, 1.0f + (levelDifference * 0.05f));
    }
    
    return 1.0f;  // 同レベル
}
```

## アイテムシステム

### 装備制限

#### 装備スロット制限
```csharp
public record EquipmentInventory
{
    // 各装備種別は1つまで
    public EquippableItem? WeaponSlot { get; }    // 武器スロット
    public EquippableItem? ArmorSlot { get; }     // 防具スロット  
    public EquippableItem? AccessarySlot { get; } // アクセサリスロット
    
    public EquipmentInventory EquipItem(EquippableItem item)
    {
        return item.EquipmentType switch
        {
            EquipmentType.Weapon => this with { WeaponSlot = item },
            EquipmentType.Armor => this with { ArmorSlot = item },
            EquipmentType.Accessary => this with { AccessarySlot = item },
            _ => throw new ArgumentException("Invalid equipment type")
        };
    }
}
```

### 強化システム

#### 強化パラメータ制限
```csharp
public record EnhancementParameter
{
    // 各パラメータは0以上
    public int Sharpness { get; }   // 鋭さ
    public int Speed { get; }       // 速さ  
    public int Accuracy { get; }    // 正確さ
    public int Weight { get; }      // 重さ
    public int Durability { get; }  // 丈夫さ
    
    public EnhancementParameter(int sharpness, int speed, int accuracy, int weight, int durability)
    {
        Sharpness = Math.Max(0, sharpness);
        Speed = Math.Max(0, speed);
        Accuracy = Math.Max(0, accuracy);
        Weight = Math.Max(0, weight);
        Durability = Math.Max(0, durability);
    }
    
    // 攻撃力計算（鋭さ + 速さ + 正確さ）
    public int GetAttackValue() => Sharpness + Speed + Accuracy;
    
    // 防御力計算（重さ + 丈夫さ）
    public int GetDefenseValue() => Weight + Durability;
}
```

#### 強化制限
- **強化回数制限**: なし（各パラメータ個別管理）
- **強化値下限**: 0（負の値は不可）
- **強化値上限**: 実装上の制限なし

### クラフトシステム

#### クラフト条件
```csharp
public bool CanCraft(CraftingRecipe recipe, InfiniteSlotInventory inventory)
{
    // 必要素材の完全一致チェック
    foreach (var required in recipe.RequiredMaterials)
    {
        var availableQuantity = inventory.GetItemQuantity(required.Item);
        if (availableQuantity < required.Quantity)
            return false;
    }
    
    return true;
}
```

#### クラフト実行
```csharp
public CraftResult ExecuteCraft(CraftingRecipe recipe, InfiniteSlotInventory inventory)
{
    if (!CanCraft(recipe, inventory))
        return CraftResult.Failed("Insufficient materials");
    
    // 素材消費
    var updatedInventory = inventory;
    foreach (var material in recipe.RequiredMaterials)
    {
        updatedInventory = updatedInventory.RemoveItem(material.Item, material.Quantity);
    }
    
    // 生成物追加
    foreach (var product in recipe.Products)
    {
        updatedInventory = updatedInventory.AddItem(product.Item, product.Quantity);
    }
    
    return CraftResult.Success(updatedInventory);
}
```

## インベントリシステム

### 容量制限

#### 装備インベントリ
```csharp
// 各装備種別1つまで固定
public class EquipmentInventory
{
    public int GetMaxCapacity(EquipmentType type) => 1;
}
```

#### 無制限インベントリ
```csharp
// アイテムスタック制限のみ
public class InfiniteSlotInventory
{
    public int GetMaxStackSize(IItem item) => item switch
    {
        ICraftMaterialItem => 999,      // 素材: 999個まで
        EquippableItem => 1,            // 装備: 1個まで
        _ => 99                         // その他: 99個まで
    };
}
```

### アイテム移動制限

#### 移動可能条件
```csharp
public bool CanMoveItem(IItem item, BaseInventory from, BaseInventory to, int quantity)
{
    // 移動元に十分な数量があるか
    if (from.GetItemQuantity(item) < quantity)
        return false;
    
    // 移動先に追加可能か
    if (!to.CanAddItem(item, quantity))
        return false;
    
    return true;
}
```

## 通貨システム

### 通貨種別・両替

#### 対応通貨
```csharp
public enum CurrencyType
{
    Sakura = 1,  // 桜式通貨（基準通貨）
    Aren = 2,    // アレン式通貨
}
```

#### 両替レート
```csharp
public Currency Exchange(CurrencyType targetType)
{
    if (Type == targetType) return this;
    
    var rate = GetExchangeRate(Type, targetType);
    var convertedValue = (int)(Value * rate);
    
    return new Currency(targetType, convertedValue);
}

private static float GetExchangeRate(CurrencyType from, CurrencyType to)
{
    return (from, to) switch
    {
        (CurrencyType.Sakura, CurrencyType.Aren) => 0.8f,   // 桜→アレン: 0.8倍
        (CurrencyType.Aren, CurrencyType.Sakura) => 1.25f,  // アレン→桜: 1.25倍
        _ => 1.0f
    };
}
```

### 取引制限

#### 通貨不足チェック
```csharp
public bool CanAfford(CurrencyType type, int amount)
{
    return GetCurrencyAmount(type) >= amount;
}

public Wallet SubtractCurrency(CurrencyType type, int amount)
{
    if (!CanAfford(type, amount))
        throw new InvalidOperationException("Insufficient funds");
    
    var current = GetCurrencyAmount(type);
    var updated = currencies.SetItem(type, new Currency(type, current - amount));
    
    return new Wallet(updated);
}
```

## スキルシステム

### スキル習得条件

#### 条件チェック
```csharp
public bool CanAcquireSkill(Skill skill, PlayerCommonEntity player)
{
    var conditions = skill.AcquisitionConditions;
    
    // レベル条件
    if (player.Level.Value < conditions.Level.Value)
        return false;
    
    // ステータス条件
    if (player.Strength.Value < conditions.Strength.Value ||
        player.Agility.Value < conditions.Agility.Value)
        return false;
    
    // 前提スキル条件
    foreach (var required in conditions.RequiredSkills)
    {
        var playerSkill = player.GetSkill(required.Skill);
        if (playerSkill == null || playerSkill.Proficiency.Value < required.Proficiency.Value)
            return false;
    }
    
    return true;
}
```

### スキル熟練度システム

#### 熟練度範囲・経験値変換
```csharp
public record SkillProficiency
{
    public int Value { get; }
    
    public SkillProficiency(int value)
    {
        if (value < 1 || value > 1000)
            throw new ArgumentOutOfRangeException("Proficiency must be between 1 and 1000");
        
        Value = value;
    }
}

public record SkillExperience
{
    public SkillProficiency CalculateProficiency()
    {
        // 経験値100につき熟練度1上昇
        var proficiency = Math.Min(1000, Math.Max(1, Value / 100));
        return new SkillProficiency(proficiency);
    }
}
```

## フィールドシステム

### 階層制限

#### 4層階層の制約
```csharp
// World → Map → Field → Chunk の順序厳守
public record MapModel
{
    public MapModel(MapID id, WorldID worldId, ...)
    {
        // Mapは必ずWorldに属する
        if (worldId == null)
            throw new ArgumentNullException("Map must belong to a World");
    }
}

public record FieldModel  
{
    public FieldModel(FieldID id, MapID mapId, ...)
    {
        // Fieldは必ずMapに属する
        if (mapId == null)
            throw new ArgumentNullException("Field must belong to a Map");
    }
}
```

## ステータス効果システム

### 効果累積ルール

#### 複数効果の統合
```csharp
public static BattleStatusEffectModel Combine(IEnumerable<BattleStatusEffectModel> effects)
{
    // 加算方式での累積（乗算は行わない）
    return effects.Aggregate(new BattleStatusEffectModel(0, 0, 0, 0), 
        (acc, effect) => new BattleStatusEffectModel
        {
            MaxHealth = acc.MaxHealth + effect.MaxHealth,
            Attack = acc.Attack + effect.Attack,
            Defense = acc.Defense + effect.Defense,
            MovementSpeed = acc.MovementSpeed + effect.MovementSpeed
        });
}
```

### 効果持続時間

#### ターン制管理
```csharp
public record BattleStatusEffect
{
    public int Duration { get; }  // 残り持続ターン数
    
    public BattleStatusEffect DecreaseDuration()
    {
        return this with { Duration = Math.Max(0, Duration - 1) };
    }
    
    public bool IsExpired => Duration <= 0;
}
```

## クエストシステム

### クエスト状態遷移

#### 状態遷移ルール
```csharp
public enum QuestStatus
{
    Available = 1,   // 受注可能
    InProgress = 2,  // 進行中  
    Completed = 3,   // 完了
    Failed = 4       // 失敗
}

public QuestModel AcceptQuest()
{
    if (Status != QuestStatus.Available)
        throw new InvalidOperationException("Quest is not available");
    
    return this with { Status = QuestStatus.InProgress };
}

public QuestModel CompleteQuest()
{
    if (Status != QuestStatus.InProgress)
        throw new InvalidOperationException("Quest is not in progress");
    
    return this with { Status = QuestStatus.Completed };
}
```

#### 締切チェック
```csharp
public bool IsOverdue(DateTime currentTime)
{
    return Deadline.HasValue && currentTime > Deadline.Value && Status == QuestStatus.InProgress;
}

public QuestModel CheckDeadline(DateTime currentTime)
{
    if (IsOverdue(currentTime))
        return this with { Status = QuestStatus.Failed };
    
    return this;
}
```

これらのビジネスルールにより、ゲームシステムの整合性と公平性が保たれています。