# Item ドメイン

## 概要

Item ドメインは、ゲーム内のあらゆるアイテム、装備品、クラフトシステムを管理する基盤ドメインです。\
アイテムの種別、特性、強化、クラフトレシピなどを統一的に扱います。

## ドメインモデル

### 基底インターフェース・クラス

#### IItem
```csharp
public interface IItem
{
    string ItemName { get; }
}
```

#### Item（基底クラス）
```csharp
public record Item : IItem
{
    protected readonly ItemName itemName;
    public string ItemName => itemName.Value;
    
    protected Item(ItemName itemName)
    {
        this.itemName = itemName;
    }
}
```

### 装備システム

#### EquippableItem
```csharp
public record EquippableItem : Item
{
    public EquipmentType EquipmentType { get; }
    public EnhancementParameter Enhancement { get; }
    
    public EquippableItem(
        ItemName itemName, 
        EquipmentType equipmentType, 
        EnhancementParameter enhancement) 
        : base(itemName)
    {
        EquipmentType = equipmentType;
        Enhancement = enhancement;
    }
}
```

#### 装備種別
```csharp
public enum EquipmentType
{
    Weapon = 1,     // 武器
    Armor = 2,      // 防具
    Accessary = 3   // アクセサリ
}
```

#### 強化パラメータ
```csharp
public record EnhancementParameter
{
    public int Sharpness { get; }   // 鋭さ（攻撃力に寄与）
    public int Speed { get; }       // 速さ（攻撃力に寄与）
    public int Accuracy { get; }    // 正確さ（攻撃力に寄与）
    public int Weight { get; }      // 重さ（防御力に寄与）
    public int Durability { get; }  // 丈夫さ（防御力に寄与）
    
    public EnhancementParameter(int sharpness, int speed, int accuracy, int weight, int durability)
    {
        Sharpness = Math.Max(0, sharpness);
        Speed = Math.Max(0, speed);
        Accuracy = Math.Max(0, accuracy);
        Weight = Math.Max(0, weight);
        Durability = Math.Max(0, durability);
    }
    
    // 攻撃力計算
    public int GetAttackValue() => Sharpness + Speed + Accuracy;
    
    // 防御力計算
    public int GetDefenseValue() => Weight + Durability;
}
```

### クラフトシステム

#### ICraftableItem
```csharp
public interface ICraftableItem : IItem
{
    CraftingRecipe Recipe { get; }
}
```

#### ICraftMaterialItem
```csharp
public interface ICraftMaterialItem : IItem
{
    // 素材アイテムのマーカーインターフェース
    // 実装クラスで素材特有のプロパティを定義
}
```

#### CraftingRecipe
```csharp
public record CraftingRecipe
{
    public ImmutableArray<ItemAndQuantity> RequiredMaterials { get; }
    public ImmutableArray<ItemAndQuantity> Products { get; }
    
    public CraftingRecipe(
        ImmutableArray<ItemAndQuantity> requiredMaterials,
        ImmutableArray<ItemAndQuantity> products)
    {
        RequiredMaterials = requiredMaterials;
        Products = products;
    }
}

public record ItemAndQuantity
{
    public IItem Item { get; }
    public int Quantity { get; }
    
    public ItemAndQuantity(IItem item, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");
        
        Item = item;
        Quantity = quantity;
    }
}
```

### 値オブジェクト

#### ItemName
```csharp
public record ItemName
{
    public string Value { get; }
    
    public ItemName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Item name cannot be empty");
        
        if (value.Length > 50)
            throw new ArgumentException("Item name cannot exceed 50 characters");
        
        Value = value;
    }
}
```

## ビジネスルール

### 装備制限

#### 装備パラメータ制約
- **強化値下限**: 各パラメータは0以上
- **強化値上限**: 実装上の制限なし（バランス調整は運用で対応）
- **強化回数制限**: なし（各パラメータ個別管理）

#### 装備効果計算
```csharp
public record EnhancementParameter
{
    // 攻撃力寄与度
    public int GetAttackValue() => Sharpness + Speed + Accuracy;
    
    // 防御力寄与度  
    public int GetDefenseValue() => Weight + Durability;
    
    // 強化合計値
    public int GetTotalEnhancement() => GetAttackValue() + GetDefenseValue();
}
```

### クラフト制限

#### クラフト実行条件
```csharp
public bool CanCraft(CraftingRecipe recipe, IInventory inventory)
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

#### クラフト結果
```csharp
public record CraftResult
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public ImmutableArray<ItemAndQuantity> CreatedItems { get; }
    
    public static CraftResult Success(ImmutableArray<ItemAndQuantity> items) =>
        new CraftResult(true, string.Empty, items);
    
    public static CraftResult Failed(string errorMessage) =>
        new CraftResult(false, errorMessage, ImmutableArray<ItemAndQuantity>.Empty);
}
```

### アイテム種別制限

#### スタック制限
```csharp
public static int GetMaxStackSize(IItem item) => item switch
{
    ICraftMaterialItem => 999,      // 素材: 999個まで
    EquippableItem => 1,            // 装備: 1個まで（個別管理）
    _ => 99                         // その他: 99個まで
};
```

## ゲームロジック

### 装備品作成

```csharp
// 基本装備の作成
public static EquippableItem CreateBasicWeapon(string name)
{
    return new EquippableItem(
        new ItemName(name),
        EquipmentType.Weapon,
        new EnhancementParameter(5, 3, 2, 0, 1) // 基本的な武器パラメータ
    );
}

// 強化装備の作成
public static EquippableItem EnhanceEquipment(
    EquippableItem baseEquipment,
    int sharpnessBonus = 0,
    int speedBonus = 0,
    int accuracyBonus = 0,
    int weightBonus = 0,
    int durabilityBonus = 0)
{
    var currentEnhancement = baseEquipment.Enhancement;
    var newEnhancement = new EnhancementParameter(
        currentEnhancement.Sharpness + sharpnessBonus,
        currentEnhancement.Speed + speedBonus,
        currentEnhancement.Accuracy + accuracyBonus,
        currentEnhancement.Weight + weightBonus,
        currentEnhancement.Durability + durabilityBonus
    );
    
    return baseEquipment with { Enhancement = newEnhancement };
}
```

### クラフト実行

```csharp
public static CraftResult ExecuteCraft(CraftingRecipe recipe, IInventory inventory)
{
    // 1. クラフト可能性チェック
    if (!CanCraft(recipe, inventory))
        return CraftResult.Failed("Insufficient materials");
    
    // 2. 素材消費（実際のインベントリ操作は Inventory ドメインで実行）
    var materialConsumption = recipe.RequiredMaterials;
    
    // 3. 生成物作成
    var products = recipe.Products;
    
    return CraftResult.Success(products);
}
```

### 装備比較

```csharp
public static class EquipmentComparer
{
    public static int CompareAttackPower(EquippableItem item1, EquippableItem item2)
    {
        return item1.Enhancement.GetAttackValue().CompareTo(item2.Enhancement.GetAttackValue());
    }
    
    public static int CompareDefensePower(EquippableItem item1, EquippableItem item2)
    {
        return item1.Enhancement.GetDefenseValue().CompareTo(item2.Enhancement.GetDefenseValue());
    }
    
    public static int CompareTotalPower(EquippableItem item1, EquippableItem item2)
    {
        var total1 = item1.Enhancement.GetTotalEnhancement();
        var total2 = item2.Enhancement.GetTotalEnhancement();
        return total1.CompareTo(total2);
    }
}
```

## 他ドメインとの連携

### Character ドメインとの連携
- **装備効果**: 戦闘パラメータ計算への寄与
- **装備条件**: レベル・ステータス要件（将来拡張）
- 詳細: [EquipmentSystem.md](../systems/EquipmentSystem.md)

### Inventory ドメインとの連携
- **アイテム所有**: インベントリでの管理
- **アイテム移動**: スロット間の移動制御
- 詳細: [Inventory.md](./Inventory.md)

### Skill ドメインとの連携
- **クラフト条件**: 生産スキル熟練度要件
- **装備条件**: 装備可能スキル要件（将来拡張）
- 詳細: [CraftingSystem.md](../systems/CraftingSystem.md)

## 拡張ポイント

### 装備条件システム
```csharp
// 将来的な拡張例：装備条件の追加
public record EquipmentRequirements
{
    public PlayerLevel RequiredLevel { get; }
    public Strength RequiredStrength { get; }
    public Agility RequiredAgility { get; }
    public ImmutableArray<SkillAndProficiency> RequiredSkills { get; }
}

public record EquippableItem : Item
{
    // 既存プロパティ...
    public EquipmentRequirements? Requirements { get; }
}
```

### アイテム品質システム
```csharp
// 品質システムの追加
public enum ItemQuality
{
    Common = 1,    // 一般
    Uncommon = 2,  // 上級
    Rare = 3,      // 希少
    Epic = 4,      // 史詩
    Legendary = 5  // 伝説
}

public record Item : IItem
{
    // 既存プロパティ...
    public ItemQuality Quality { get; }
}
```

### 耐久度システム
```csharp
// 装備の耐久度システム
public record Durability
{
    public int Current { get; }
    public int Maximum { get; }
    
    public float DurabilityRatio => (float)Current / Maximum;
    public bool IsBroken => Current <= 0;
}

public record EquippableItem : Item
{
    // 既存プロパティ...
    public Durability Durability { get; }
}
```

### 特殊効果システム
```csharp
// アイテム固有の特殊効果
public interface IItemEffect
{
    string EffectName { get; }
    string Description { get; }
}

public record EquippableItem : Item
{
    // 既存プロパティ...
    public ImmutableArray<IItemEffect> SpecialEffects { get; }
}
```

Item ドメインは装備システムとクラフトシステムの基盤となる重要なドメインです。\
他のドメインとの連携により、豊富なゲームコンテンツを提供します。