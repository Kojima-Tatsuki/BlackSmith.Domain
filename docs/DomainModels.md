# BlackSmith.Domain ドメインモデル

## 基盤クラス

### BasicID - 統一ID管理

```csharp
public abstract record BasicID
{
    protected abstract string Prefix { get; }
    public string Value => Prefix + guid;
    private Guid guid { get; }
}
```

全ドメインIDの基底クラス。プレフィックス付きGUIDによる一意性を保証し、JSONシリアライゼーションに対応しています。

## Character ドメイン

### プレイヤーエンティティ

#### PlayerCommonEntity
プレイヤーの基本情報を管理するルートエンティティ。

```csharp
public record PlayerCommonEntity
{
    public PlayerID Id { get; }
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public Experience Experience { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
    public StatusPoint StatusPoint { get; }
    public ImmutableArray<SkillAndProficiency> Skills { get; }
}
```

#### PlayerBattleEntity
戦闘時の状態を管理する戦闘特化エンティティ。

```csharp
public record PlayerBattleEntity
{
    public PlayerID Id { get; }
    public PlayerName Name { get; }
    public CurrentHealth CurrentHealth { get; }
    public CharacterBattleModule BattleModule { get; }
}
```

### 値オブジェクト

#### 基本パラメータ
```csharp
public record PlayerID : BasicID
{
    protected override string Prefix => "PLY_";
}

public record PlayerName
{
    public string Value { get; }
    // 3-20文字の制限
}

public record PlayerLevel
{
    public int Value { get; }
    // 1以上の制限
}
```

#### 能力値システム
```csharp
public record Strength
{
    public int Value { get; }
    // 筋力: 物理攻撃・防御に影響
}

public record Agility
{
    public int Value { get; }
    // 俊敏性: 物理攻撃・防御に影響
}

public record StatusPoint
{
    public int Value { get; }
    // レベルアップ時に3ポイント獲得
}
```

#### 経験値システム
```csharp
public record Experience
{
    public int Value { get; }
    
    // 指数関数的レベル計算
    // 必要経験値 = 100 * (1 - 1.25^(level-1)) / (1 - 1.25)
    private static readonly int InitExpRequirement = 100;
    private static readonly float LevelDifferenceMultiplier = 1.25f;
}
```

#### 戦闘パラメータ
```csharp
public record BattleParameter
{
    public int MaxHealth { get; }      // レベル × 10
    public int AttackValue { get; }    // (STR + AGI) × 2 + 装備補正
    public int DefenseValue { get; }   // (STR + AGI) × 2 + 装備補正
}

public record CurrentHealth
{
    public int Value { get; }
    public CurrentHealth TakeDamage(int damage) => 
        new(Math.Max(0, Value - Math.Max(1, damage)));
}
```

### ファクトリー・コマンド

#### PlayerFactory
```csharp
public static class PlayerFactory
{
    public static PlayerCommonEntity CreateNew(PlayerCommonCreateCommand command);
    public static PlayerCommonEntity Reconstruct(PlayerCommonReconstructCommand command);
    public static PlayerBattleEntity CreateBattleEntity(PlayerCommonEntity common);
}
```

#### コマンドオブジェクト
```csharp
public record PlayerCommonCreateCommand
{
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
}

public record PlayerCommonReconstructCommand
{
    public PlayerID Id { get; }
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public Experience Experience { get; }
    // ... その他プロパティ
}
```

## Item ドメイン

### アイテム階層

```csharp
public interface IItem
{
    string ItemName { get; }
}

public record Item : IItem
{
    protected readonly ItemName itemName;
    public string ItemName => itemName.Value;
}

public record EquippableItem : Item
{
    public EquipmentType EquipmentType { get; }
    public EnhancementParameter Enhancement { get; }
}
```

### 装備システム

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
    public int Sharpness { get; }   // 鋭さ（攻撃力）
    public int Speed { get; }       // 速さ（攻撃力）
    public int Accuracy { get; }    // 正確さ（攻撃力）
    public int Weight { get; }      // 重さ（防御力）
    public int Durability { get; }  // 丈夫さ（防御力）
    
    public int GetAttackValue() => Sharpness + Speed + Accuracy;
    public int GetDefenseValue() => Weight + Durability;
}
```

### クラフトシステム

```csharp
public interface ICraftableItem : IItem
{
    CraftingRecipe Recipe { get; }
}

public interface ICraftMaterialItem : IItem
{
    // 素材アイテムマーカーインターフェース
}

public record CraftingRecipe
{
    public ImmutableArray<ItemAndQuantity> RequiredMaterials { get; }
    public ImmutableArray<ItemAndQuantity> Products { get; }
}
```

## Inventory ドメイン

### インベントリシステム

#### 基本構造
```csharp
public record ItemSlot
{
    public IItem Item { get; }
    public int Quantity { get; }
    
    public ItemSlot AddItem(int quantity) => 
        this with { Quantity = Quantity + quantity };
}

public abstract record BaseInventory
{
    protected ImmutableArray<ItemSlot> slots;
    public abstract bool CanAddItem(IItem item, int quantity);
    public abstract BaseInventory AddItem(IItem item, int quantity);
}
```

#### 装備インベントリ
```csharp
public record EquipmentInventory : BaseInventory
{
    // 各装備種別1つまで装備可能
    public EquippableItem? WeaponSlot { get; }
    public EquippableItem? ArmorSlot { get; }
    public EquippableItem? AccessarySlot { get; }
    
    public EquipmentInventory EquipItem(EquippableItem item);
    public EquipmentInventory UnequipItem(EquipmentType type);
}
```

#### 無制限インベントリ
```csharp
public record InfiniteSlotInventory : BaseInventory
{
    // スタック制限のみ（アイテム種別毎）
    public override bool CanAddItem(IItem item, int quantity) => true;
}
```

### 通貨システム

```csharp
public record Currency
{
    public CurrencyType Type { get; }
    public int Value { get; }
    
    // 両替機能
    internal Currency Exchange(CurrencyType targetType)
    {
        // 為替レート適用
    }
}

public enum CurrencyType
{
    Sakura = 1,  // 桜式通貨
    Aren = 2,    // アレン式通貨
}

public record Wallet
{
    private readonly ImmutableDictionary<CurrencyType, Currency> currencies;
    
    public Wallet AddCurrency(Currency currency);
    public Wallet SubtractCurrency(CurrencyType type, int amount);
}
```

## Skill ドメイン

### スキル階層

```csharp
public abstract record Skill
{
    public SkillName Name { get; }
    public abstract SkillType Type { get; }
    public SkillAcquisitionConditions AcquisitionConditions { get; }
}

public record BattleSkill : Skill
{
    public override SkillType Type => SkillType.Battle;
}

public record ProductionSkill : Skill
{
    public override SkillType Type => SkillType.Production;
    public ProductionType ProductionType { get; }
}
```

### スキル種別

```csharp
public enum SkillType
{
    Battle = 1,
    Production = 2
}

public enum ProductionType
{
    Creation = 1,  // 作成
    Refining = 2,  // 精錬
    Repair = 3     // 修理
}
```

### スキル習得・熟練度

```csharp
public record SkillAcquisitionConditions
{
    public PlayerLevel Level { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
    public IReadOnlyCollection<SkillAndProficiency> RequiredSkills { get; }
}

public record SkillProficiency
{
    public int Value { get; }  // 1-1000
}

public record SkillExperience
{
    public int Value { get; }
    
    public SkillProficiency CalculateProficiency()
    {
        // 経験値 → 熟練度変換
        return new SkillProficiency(Math.Min(1000, Value / 100));
    }
}

public record SkillAndProficiency
{
    public Skill Skill { get; }
    public SkillProficiency Proficiency { get; }
}
```

## Field ドメイン

### 4層階層システム

```csharp
public record WorldID : BasicID
{
    protected override string Prefix => "WLD_";
}

public record MapID : BasicID  
{
    protected override string Prefix => "MAP_";
}

public record FieldID : BasicID
{
    protected override string Prefix => "FLD_";
}

public record ChunkID : BasicID
{
    protected override string Prefix => "CHK_";
}
```

### 階層関係

```csharp
public record WorldModel
{
    public WorldID Id { get; }
    public string Name { get; }
    public ImmutableArray<MapID> Maps { get; }
}

public record MapModel
{
    public MapID Id { get; }
    public WorldID WorldId { get; }
    public string Name { get; }
    public ImmutableArray<FieldID> Fields { get; }
}

public record FieldModel
{
    public FieldID Id { get; }
    public MapID MapId { get; }
    public string Name { get; }
    public ImmutableArray<ChunkID> Chunks { get; }
}

public record ChunkModel
{
    public ChunkID Id { get; }
    public FieldID FieldId { get; }
    // レンダリング・処理用メタデータ
}
```

## PassiveEffect ドメイン

### ステータス効果

```csharp
public record EffectID : BasicID
{
    protected override string Prefix => "EFF_";
}

public record BattleStatusEffect
{
    public EffectID Id { get; }
    public BattleStatusEffectModel StatusModel { get; }
    public int Duration { get; }  // 持続ターン数
}

public record BattleStatusEffectModel
{
    public int MaxHealth { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int MovementSpeed { get; }
    
    // 複数効果の累積計算
    public static BattleStatusEffectModel Combine(
        IEnumerable<BattleStatusEffectModel> effects)
    {
        return effects.Aggregate((a, b) => new BattleStatusEffectModel
        {
            MaxHealth = a.MaxHealth + b.MaxHealth,
            Attack = a.Attack + b.Attack,
            Defense = a.Defense + b.Defense,
            MovementSpeed = a.MovementSpeed + b.MovementSpeed
        });
    }
}
```

## Quest ドメイン

```csharp
public record QuestID : BasicID
{
    protected override string Prefix => "QST_";
}

public record QuestModel
{
    public QuestID Id { get; }
    public string Title { get; }
    public string Description { get; }
    public PlayerID? ClientId { get; }     // 依頼人
    public ImmutableArray<IItem> Rewards { get; }
    public DateTime? Deadline { get; }     // 締切
    public QuestStatus Status { get; }
}

public enum QuestStatus
{
    Available = 1,   // 受注可能
    InProgress = 2,  // 進行中
    Completed = 3,   // 完了
    Failed = 4       // 失敗
}
```

## 統合モジュール

### CharacterBattleModule
複数のドメインを統合した戦闘計算モジュール。

```csharp
public class CharacterBattleModule
{
    private readonly BattleEquipmentModule equipmentModule;
    private readonly BattleStatusEffectModule statusEffectModule;
    
    public BattleParameter GetBattleParameter()
    {
        var baseParam = GetLevelDependentParameter();
        var equipmentParam = equipmentModule.GetEquipmentParameter();
        var effectParam = statusEffectModule.GetStatusEffectParameter();
        
        return BattleParameter.Combine(baseParam, equipmentParam, effectParam);
    }
}
```

このドメインモデルにより、ゲームの複雑なビジネスロジックを型安全で拡張可能な形で表現しています。