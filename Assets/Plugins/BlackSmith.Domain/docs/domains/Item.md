# Item ドメイン

## 概要

Item ドメインは、ゲーム内のあらゆるアイテム、装備品、クラフトシステムを管理する基盤ドメインです。

### 主要な責務
- **アイテム管理**：ゲーム内アイテムの識別と基本情報管理
- **装備システム**：装備アイテムの作成、強化、効果管理
- **クラフトシステム**：素材アイテムからのアイテム作成管理
- **NPCショップ**：アイテム購入・販売システム

### 現在の実装状況
- **IItem/Item**：基本アイテムインターフェースとレコードクラスを実装
- **EquippableItem**：装備アイテムの完全な作成・強化システムを実装
- **CraftingRecipe**：素材ベースのクラフトシステムを実装
- **NPCItemShop**：通貨システム統合型ショップを実装
- **EquipmentEnhanceService**：確率ベース強化システムを実装

### 設計の特徴
- **コマンドパターン**：CreateCommandによる安全なアイテム作成
- **ドメインサービス**：強化・クラフト処理の分離
- **JSON対応**：完全なシリアライゼーション対応
- **Character ドメイン連携**：制作者ID・装備要件の管理
- **Currency ドメイン連携**：通貨システムとの統合

このドメインは Inventory、Character、Currency ドメインとの密接な連携により、ゲームの中核システムを支えています。

## ドメインモデル

### 識別子

#### 実装済み識別子

```csharp
// Item.cs の実装済み識別子
public record ItemID : BasicID
{
    protected override string Prefix => "Item-";
}
```

### エンティティ・値オブジェクト

#### 基底インターフェース・クラス

##### IItem (Item.cs)
```csharp
// 実装済み：基本アイテムインターフェース
public interface IItem
{
    string ItemName { get; }
}
```

##### Item (Item.cs)
```csharp
// 実装済み：基底アイテムレコード
public record Item : IItem
{
    public string ItemName => itemName.Value;
    private protected readonly ItemName itemName;
    
    [JsonConstructor]
    internal Item(string itemName);
}
```

##### ItemName (Item.cs)
```csharp
// 実装済み：アイテム名値オブジェクト
internal record ItemName
{
    public string Value { get; }
    
    [JsonConstructor]
    internal ItemName(string value); // 1文字以上の検証実装
}
```

##### ItemType (Item.cs)
```csharp
// 実装済み：アイテム種別
internal enum ItemType
{
    None = 0x0,    // なし
    Weapon = 0x1,  // 武器
    Armor = 0x2,   // 防具
    Consum = 0x4,  // 消費アイテム
}
```

##### ItemComparer (Item.cs)
```csharp
// 実装済み：アイテム比較用クラス
internal class ItemComparer : IEqualityComparer<Item>
{
    public bool Equals(Item? x, Item? y);
    public int GetHashCode(Item obj);
}
```

#### 装備システム

##### IEquipableItem (EquippableItem.cs)
```csharp
// 実装済み：装備アイテムインターフェース
public interface IEquipableItem : IItem
{
    IEquipableItem Enchant(EnhancementParameter parameter);
    IEquipableItem Repair(); // 注意：NotImplementedException実装
}
```

##### EquippableItem (EquippableItem.cs)
```csharp
// 実装済み：装備アイテムレコード
public record EquippableItem : Item, IEquipableItem
{
    public EquipmentType EquipType { get; }
    public EquipmentAttack Attack { get; }
    public EquipmentDefense Defense { get; }
    public EnhancementParameter EnhancementParameter { get; }
    public AdditionalParameter AdditionalParameter { get; }
    public RequireParameter RequireParameter { get; }
    
    // コマンドパターンによる作成
    internal EquippableItem(CreateCommand command);
    
    // JSON用コンストラクタ
    [JsonConstructor]
    private EquippableItem(/* 全プロパティ */);
    
    // 強化メソッド
    internal EquippableItem Enchant(EnhancementParameter parameter);
    
    // コマンドパターン
    internal record CreateCommand { /* 全プロパティ */ }
}
```

##### EquipmentType (EquippableItem.cs)
```csharp
// 実装済み：装備種別
public enum EquipmentType
{
    Weapon,     // 武器
    Armor,      // 防具
    Accessary,  // アクセサリ
}
```

##### 装備パラメータ値オブジェクト群
```csharp
// EquipmentAttack (EquippableItem.cs)
public record EquipmentAttack
{
    public int Value { get; }
    [JsonConstructor]
    internal EquipmentAttack(int value);
}

// EquipmentDefense (EquippableItem.cs)
public record EquipmentDefense
{
    public int Value { get; }
    [JsonConstructor]
    internal EquipmentDefense(int value);
}
```

##### EnhancementParameter (EquippableItem.cs)
```csharp
// 実装済み：強化パラメータ
public record EnhancementParameter
{
    public int Sharpness { get; }  // 鋭さ
    public int Quickness { get; }  // 速さ（注意：Speedではない）
    public int Accuracy { get; }   // 正確さ
    public int Heaviness { get; }  // 重さ（注意：Weightではない）
    public int Durability { get; } // 丈夫さ
    
    internal EnhancementParameter(); // 全て0で初期化
    [JsonConstructor]
    internal EnhancementParameter(int sharpness, int quickness, int accuracy, int heaviness, int durability);
    
    // 実装済み：強化回数計算
    public int GetEnchancedCount => Sharpness + Quickness + Accuracy + Heaviness + Durability;
    
    // 実装済み：強化実行
    internal EnhancementParameter AddEnhance(EnhanceType type);
    
    public enum EnhanceType
    {
        Sharpness, Quickness, Accuracy, Heaviness, Durability,
    }
}
```

##### AdditionalParameter (EquippableItem.cs)
```csharp
// 実装済み（部分）：追加パラメータ（注意：コンストラクタ未実装）
public record AdditionalParameter
{
    public int Attack { get; }
    public int Defense { get; }
    public int STR { get; }
    public int AGI { get; }
}
```

##### RequireParameter (EquippableItem.cs)
```csharp
// 実装済み：装備要求パラメータ
public record RequireParameter
{
    public PlayerLevel Level { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
    
    [JsonConstructor]
    internal RequireParameter(PlayerLevel level, Strength strength, Agility agility);
}
```

#### クラフトシステム

##### ICraftMaterialItem (CraftableItem.cs)
```csharp
// 実装済み：素材アイテムマーカーインターフェース
public interface ICraftMaterialItem : IItem
{
    // マーカーインターフェース（実装は空）
}
```

##### ICraftableItem (CraftableItem.cs)
```csharp
// 実装済み：クラフト可能アイテムインターフェース
public interface ICraftableItem : IItem
{
    CharacterID CreatedBy { get; } // 制作者ID
    IReadOnlyCollection<ICraftMaterialItem> GetRequireMaterials();
}
```

##### CraftingRecipe (CraftableItem.cs)
```csharp
// 実装済み：クラフトレシピクラス
public class CraftingRecipe
{
    public readonly ICraftableItem Craftable; // 完成アイテム
    public readonly IReadOnlyList<ICraftMaterialItem> Materials; // 必要素材（順序あり）
    
    internal CraftingRecipe(ICraftableItem craftable, IReadOnlyList<ICraftMaterialItem> materials);
    
    // 実装済み：素材チェック機能
    public bool IsCraftable(IReadOnlyCollection<ICraftMaterialItem> materials);
}
```

#### NPCショップシステム

```csharp
// NPCItemShop.cs の実装済み機能
public interface IItemPurchasable
{
    IItem Purchase(IItem item, Currency.Currency money);
}

public interface IItemExhibitable
{
    Currency.Currency GetPrice(IItem item);
    bool IsExisting(IItem item);
}

public class NPCItemShop : IItemPurchasable, IItemExhibitable
{
    // 無限在庫ショップ
    private readonly Dictionary<IItem, Currency.Currency> items;
    
    internal NPCItemShop(Dictionary<IItem, Currency.Currency> items);
    
    // アイテム購入
    public IItem Purchase(IItem item, Currency.Currency money);
    
    // 価格取得・在庫確認
    public Currency.Currency GetPrice(IItem item);
    public bool IsExisting(IItem item);
}
```

#### ドメインサービス

##### EquipmentEnhanceService (EquippableItemService.cs)
```csharp
// 実装済み：装備強化サービス
public static class EquipmentEnhanceService
{
    // 強化成功確率計算
    public static EnhanceSuccessRatio CalcSuccessRatio(EquippableItem item, PlayerLevel playerLevel);
    
    public enum EnhanceSuccessRatio
    {
        Success, // 成功
        Endure,  // 変化なし
        Failure, // 失敗
    }
}
```

##### CraftableItemService (CraftableItemService.cs)
```csharp
// 実装済み：クラフトサービス
public class CraftableItemService
{
    public CraftingResult CreateItem(CraftingRecipe recipe, CharacterID createrId);
    
    public record CraftingResult
    {
        public bool IsSuccess { get; }
        public ICraftableItem? CreatedItem { get; }
        public CharacterID CreatedBy { get; }
    }
}
```

## ビジネスルール

### 実装済みルール

#### アイテム名制限
```csharp
// ItemName.cs の実装済み制限
internal record ItemName
{
    internal ItemName(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        if (value.Length == 0) throw new ArgumentOutOfRangeException("nameは1文字以上です。");
        // 注意：上限制限は現在未実装
    }
}
```

#### 装備強化ルール
```csharp
// EnhancementParameter.cs の実装済み機能
public record EnhancementParameter
{
    // 強化値制限なし（負値も許可）
    internal EnhancementParameter(int sharpness, int quickness, int accuracy, int heaviness, int durability);
    
    // 実装済み：強化回数計算
    public int GetEnchancedCount => Sharpness + Quickness + Accuracy + Heaviness + Durability;
    
    // 実装済み：個別パラメータ強化
    internal EnhancementParameter AddEnhance(EnhanceType type);
}
```

#### 装備成功確率ルール
```csharp
// EquipmentEnhanceService.cs の実装済み機能
public static class EquipmentEnhanceService
{
    // プレイヤーレベルと装備要求レベルに基づく成功確率計算
    public static EnhanceSuccessRatio CalcSuccessRatio(EquippableItem item, PlayerLevel playerLevel);
}
```

#### クラフト成功ルール
```csharp
// CraftingRecipe.cs の実装済み機能
public class CraftingRecipe
{
    // 素材の必要性チェック（完全一致ベース）
    public bool IsCraftable(IReadOnlyCollection<ICraftMaterialItem> materials)
    {
        return Materials.All(item => materials.Contains(item));
    }
}

// CraftableItemService.cs の実装済み機能
public class CraftableItemService
{
    // クラフトは素材があれば必ず成功
    public CraftingResult CreateItem(CraftingRecipe recipe, CharacterID createrId);
}
```

#### NPCショップルール
```csharp
// NPCItemShop.cs の実装済み機能
public class NPCItemShop
{
    // 無限在庫システム
    // 固定価格システム
    // 通貨交換レート対応
    public IItem Purchase(IItem item, Currency.Currency money);
}
```

### 未実装ルール

**注意**：以下のビジネスルールはドキュメントに記載されていますが、現在未実装です：

#### アイテムスタック制限
- **種別別制限**：素材999個、装備1個、その他99個
- **現在の実装**：Inventory ドメインで管理

#### 装備制限システム
- **レベル制限**：RequireParameter は定義済みだが制限チェック未実装
- **ステータス制限**：Strength/Agility 要件チェック未実装

#### 品質・耐久度システム
- **アイテム品質**：Common/Rare/Epic等の品質分類
- **耐久度**：装備の使用による劣化システム

## ゲームロジック

### 実装済み機能

#### 装備品作成
```csharp
// EquippableItem.cs の実装済み機能
var command = new EquippableItem.CreateCommand(
    "アイアンソード",
    EquipmentType.Weapon,
    new EquipmentAttack(10),
    new EquipmentDefense(2),
    new EnhancementParameter(0, 0, 0, 0, 0),
    new AdditionalParameter(), // 注意：コンストラクタ未実装
    new RequireParameter(new PlayerLevel(5), new Strength(10), new Agility(5))
);

var weapon = new EquippableItem(command);
```

#### 装備強化
```csharp
// EnhancementParameter.cs の実装済み機能
var enhancedParameter = weapon.EnhancementParameter.AddEnhance(EnhanceType.Sharpness);
var enhancedWeapon = weapon.Enchant(enhancedParameter);

// 成功確率計算
var successRatio = EquipmentEnhanceService.CalcSuccessRatio(weapon, playerLevel);
```

#### クラフト実行
```csharp
// CraftableItemService.cs の実装済み機能
var service = new CraftableItemService();
var result = service.CreateItem(recipe, creatorId);

if (result.IsSuccess)
{
    var createdItem = result.CreatedItem;
    var creator = result.CreatedBy;
}
```

#### クラフトレシピ検証
```csharp
// CraftingRecipe.cs の実装済み機能
var recipe = new CraftingRecipe(craftableItem, materials);
bool canCraft = recipe.IsCraftable(availableMaterials);
```

#### NPCショップ利用
```csharp
// NPCItemShop.cs の実装済み機能
var shop = new NPCItemShop(itemPriceDict);

// アイテム価格確認
var price = shop.GetPrice(item);
bool available = shop.IsExisting(item);

// アイテム購入
var purchasedItem = shop.Purchase(item, currency);
```

### 未実装機能

**注意**：以下の機能はドキュメントに記載されていますが、現在未実装です：

#### 装備比較システム
- **パフォーマンス比較**：攻撃力・防御力の比較機能
- **装備評価**：総合評価値の計算

#### 高度なクラフトシステム
- **クラフト失敗**：現在は必ず成功
- **品質ランダム**：生成アイテムの品質ランダム化
- **レシピ習得**：プレイヤーのレシピ管理

#### アイテム修理システム
- **耐久度修理**：IEquipableItem.Repair()は未実装
- **修理コスト**：修理に必要な素材・費用


## 拡張ポイント

### 実装可能な拡張

#### アイテム品質システム
```csharp
// 将来実装可能：品質システム
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
    public ItemQuality Quality { get; }
    // 品質による価格倍率、ドロップ確率等に影響
}
```

#### 耐久度システム
```csharp
// 将来実装可能：装備耐久度管理
public record Durability
{
    public int Current { get; }
    public int Maximum { get; }
    
    public float DurabilityRatio => (float)Current / Maximum;
    public bool IsBroken => Current <= 0;
    public bool NeedsRepair => DurabilityRatio < 0.5f;
}

public record EquippableItem : Item
{
    public Durability Durability { get; }
    
    // IEquipableItem.Repair()の実装で使用
    public EquippableItem RepairDurability(int repairAmount);
}
```

#### 特殊効果・アビリティシステム
```csharp
// 将来実装可能：アイテム固有効果
public interface IItemEffect
{
    string EffectName { get; }
    string Description { get; }
    EffectType Type { get; }
}

public enum EffectType
{
    PassiveBonus,    // パッシブ効果
    ActiveSkill,     // アクティブスキル
    SetBonus,        // セット効果
    Enchantment,     // エンチャント効果
}

public record EquippableItem : Item
{
    public IReadOnlyList<IItemEffect> SpecialEffects { get; }
}
```

#### 高度な装備要件システム
```csharp
// 将来実装可能：詳細装備条件
public record EquipmentRequirements
{
    public PlayerLevel RequiredLevel { get; }
    public Strength RequiredStrength { get; }
    public Agility RequiredAgility { get; }
    public IReadOnlyList<SkillRequirement> RequiredSkills { get; }
    public IReadOnlyList<QuestRequirement> RequiredQuests { get; }
    
    public bool CanEquip(Character.Player player);
}

public record SkillRequirement
{
    public SkillType Skill { get; }
    public int RequiredLevel { get; }
}
```

#### アイテムセットシステム
```csharp
// 将来実装可能：装備セット効果
public record ItemSet
{
    public string SetName { get; }
    public IReadOnlyList<EquippableItem> SetItems { get; }
    public IReadOnlyList<SetBonus> SetBonuses { get; }
}

public record SetBonus
{
    public int RequiredPieces { get; }
    public IItemEffect BonusEffect { get; }
}
```

### 設計基盤の特徴

現在の実装は将来の拡張に対応できる堅牢な設計基盤を提供：

- **コマンドパターン**: CreateCommandによる安全なオブジェクト作成
- **ドメインサービス**: ビジネスロジックの適切な分離
- **JSON互換性**: 完全なシリアライゼーション対応
- **型安全性**: recordによるイミュータブル設計
- **拡張性**: インターフェース分離による機能追加容易性

Item ドメインは装備システムとクラフトシステムの中核として、Character、Inventory、Currency ドメインとの統合により、豊富なゲームプレイ体験を支える基盤を提供しています。