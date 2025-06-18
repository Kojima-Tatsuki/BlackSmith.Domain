# 装備システム (Equipment System)

## 概要

装備システムは、Character、Item、Inventory の3つのドメインを統合し、プレイヤーの装備管理を行うシステムです。\
装備品の着脱、強化、制限チェック、インベントリ管理が充実して実装されており、包括的な装備管理体験を提供します。

## システム構成

### 関連ドメイン

- **[Character](../domains/Character.md)**: 装備条件、戦闘パラメータへの効果適用
- **[Item](../domains/Item.md)**: 装備品、強化パラメータ、装備種別
- **[Inventory](../domains/Inventory.md)**: 装備インベントリ、一般インベントリ

### 実装済み機能

#### 装備品管理
Item ドメインの装備アイテムシステム。

```csharp
// 実装済み：装備可能アイテム
public class EquippableItem : IItem
{
    public ItemID ID { get; }
    public ItemName ItemName { get; }
    public EnhancementParameter Enhancement { get; }
    public IReadOnlyDictionary<EquipmentSlotType, int> EquipmentSlots { get; }
    public RequireParameter RequireParameter { get; }
    
    // コマンドパターンによる生成
    public record CreateCommand(ItemID ItemID, ItemName ItemName, 
        EnhancementParameter EnhancementParameter, 
        IReadOnlyDictionary<EquipmentSlotType, int> EquipmentSlots) : ICommand;
}
```

#### 装備インベントリ管理
Inventory ドメインの装備専用管理。

```csharp
// 実装済み：装備インベントリ
internal class EquipmentInventory : IOneByInventoryService<EquippableItem>
{
    private readonly Dictionary<EquipmentType, EquippableItem> Equipments;
    
    public bool IsAddable(EquippableItem item);
    public EquippableItem AddItem(EquippableItem item);
    public EquippableItem RemoveItem(EquippableItem item);
    public bool Contains(EquippableItem item);
}
```

#### 装備強化システム
装備品の性能向上機能。

```csharp
// 実装済み：装備強化サービス
internal class EquipmentEnhanceService
{
    internal EquippableItem Execute(EquippableItem item, EnhancementMaterial material);
}

// 実装済み：強化パラメータ
public record EnhancementParameter
{
    public int AttackValue { get; }
    public int DefenseValue { get; }
    public AdditionalParameter AdditionalParameter { get; }
}
```

#### 装備制限システム
Character ドメインとの連携による装備制限。

```csharp
// 実装済み：装備要求パラメータ
public record RequireParameter
{
    public PlayerLevel Level { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
}

// 実装済み：追加パラメータ
public record AdditionalParameter
{
    public int Attack { get; }
    public int Defense { get; }
    public int STR { get; }
    public int AGI { get; }
}
```

#### 戦闘中装備管理（Usecase層）
戦闘中の装備変更とインベントリ整合性保証。

```csharp
// 実装済み：戦闘中装備変更
internal class BattleEquipmentUsecase
{
    // 装備交換処理
    internal async UniTask ChengeEquipment(CharacterID playerId, InventoryID inventoryId, 
        EquippableItem equipment, EquippableItem remove);
    
    // 装備解除処理
    internal async UniTask RemoveEquipment(CharacterID playerId, InventoryID inventoryId, 
        EquippableItem remove);
}
```

## 提供するドメインサービス

### 装備管理サービス
装備品の着脱と状態管理を提供します。

**提供する操作:**
- 装備アイテムの着用、取り外し
- 装備種別ごとの状態管理
- 装備インベントリと一般インベントリ間の移動
- 戦闘中の安全な装備変更

**ドメインルール:**
- 同種類装備は1つまでのみ装備可能
- 装備変更時のインベントリ操作はアトミックに実行
- 装備制限チェックの事前実行

### 装備制限チェックサービス
プレイヤーのステータスに基づく装備可否判定を提供します。

**提供するチェック:**
- プレイヤーレベルの要件確認
- Strength、Agilityステータスの要件確認
- 装備種別の重複チェック

**制限ルール:**
- RequireParameterで指定された最低レベル以上が必要
- 必要ステータスを満たさない場合は装備不可

### 装備強化サービス
装備品の性能向上処理を提供します。

**提供する機能:**
- 強化素材を使用した装備性能向上
- 攻撃力、防御力、追加パラメータの強化
- 強化結果の予測可能性

**強化ルール:**
- 強化素材と装備品の組み合わせにより結果が決定
- 強化後のパラメータは強化前に加算
- 強化は非可逆操作

## ビジネスルール・制約

### 装備管理制約
- **種別重複制約**: 同じEquipmentTypeの装備は1つまでのみ装備可能
- **装備制限**: RequireParameterの条件を満たさない装備は着用不可
- **インベントリ連携**: 装備変更時のアイテム移動はアトミックに実行

### 装備効果適用ルール
- **パラメータ加算**: 装備のEnhancementParameterがプレイヤーのパラメータに加算
- **効果結合**: 複数装備の効果は累積適用
- **即座反映**: 装備変更時のパラメータ変更は即座反映

### 強化システム制約
- **素材消費**: 強化実行時に強化素材を消費
- **非可逆性**: 強化結果は元に戻すこと不可
- **パラメータ結合**: 強化パラメータは元のパラメータと加算結合

### ドメイン整合性
- **戦闘連携**: 装備変更時の戦闘パラメータ自動更新
- **インベントリ同期**: 装備とインベントリ状態の一貫性保証
- **ステータス連携**: プレイヤーステータスと装備制限の連動

## 実装状況

### Domain層実装
- ✅ **EquippableItemクラス**: 装備可能アイテムの完全定義
- ✅ **EquipmentInventoryクラス**: 装備品専用インベントリ
- ✅ **EquipmentEnhanceService**: 装備強化処理サービス
- ✅ **EnhancementParameter**: 強化パラメータのイミュータブル管理
- ✅ **RequireParameter**: 装備要件パラメータ
- ✅ **AdditionalParameter**: 装備追加効果パラメータ

### Usecase層実装
- ✅ **BattleEquipmentUsecase**: 戦闘中装備変更サービス
- ✅ **装備交換処理**: ChengeEquipment()メソッド
- ✅ **装備解除処理**: RemoveEquipment()メソッド

### インターフェース定義
- ✅ **IOneByInventoryService**: 装備インベントリの抽象化
- ✅ **ICommandパターン**: 装備アイテム生成コマンド

## ドメインサービス API

### 装備管理サービス
```csharp
// 装備アイテムの定義
public class EquippableItem : IItem
{
    public ItemID ID { get; }
    public ItemName ItemName { get; }
    public EnhancementParameter Enhancement { get; }        // 強化パラメータ
    public RequireParameter RequireParameter { get; }       // 装備要件
    
    // コマンドパターンによる生成
    public record CreateCommand(ItemID ItemID, ItemName ItemName, 
        EnhancementParameter EnhancementParameter, 
        IReadOnlyDictionary<EquipmentSlotType, int> EquipmentSlots) : ICommand;
}

// 装備インベントリ管理
internal class EquipmentInventory : IOneByInventoryService<EquippableItem>
{
    public bool IsAddable(EquippableItem item);            // 装備可否判定
    public EquippableItem AddItem(EquippableItem item);    // 装備着用
    public EquippableItem RemoveItem(EquippableItem item); // 装備解除
    public bool Contains(EquippableItem item);             // 装備状態確認
}
```

### 装備強化サービス
```csharp
// 装備強化処理
internal class EquipmentEnhanceService
{
    internal EquippableItem Execute(EquippableItem item, EnhancementMaterial material);
}

// 強化パラメータの定義
public record EnhancementParameter
{
    public int AttackValue { get; }                        // 攻撃力加算値
    public int DefenseValue { get; }                       // 防御力加算値
    public AdditionalParameter AdditionalParameter { get; } // 追加パラメータ
}
```

### 戦闘中装備管理サービス
```csharp
// 戦闘中の安全な装備変更
internal class BattleEquipmentUsecase
{
    // 装備交換（新装備着用+旧装備解除）
    internal async UniTask ChengeEquipment(CharacterID playerId, InventoryID inventoryId, 
        EquippableItem equipment, EquippableItem remove);
    
    // 装備解除（装備をインベントリに戻す）
    internal async UniTask RemoveEquipment(CharacterID playerId, InventoryID inventoryId, 
        EquippableItem remove);
}
```

### 装備制限チェック
```csharp
// 装備要件パラメータ
public record RequireParameter
{
    public PlayerLevel Level { get; }      // 必要レベル
    public Strength Strength { get; }      // 必要筋力
    public Agility Agility { get; }        // 必要敏捷性
}

// 装備追加効果
public record AdditionalParameter
{
    public int Attack { get; }   // 攻撃力ボーナス
    public int Defense { get; }  // 防御力ボーナス
    public int STR { get; }      // 筋力ボーナス
    public int AGI { get; }      // 敏捷性ボーナス
}
```

### 基本的な利用パターン
1. **装備制限チェック**: `RequireParameter` でプレイヤーの装備可否を判定
2. **装備着脱**: `EquipmentInventory` で装備品の着脱管理
3. **装備強化**: `EquipmentEnhanceService` で装備性能向上
4. **戦闘中変更**: `BattleEquipmentUsecase` で安全な装備交換

### ドメインサービスの特徴
- **UniTask対応**: 非同期での装備変更処理
- **アトミック操作**: 装備変更とインベントリ操作の原子性
- **コマンドパターン**: 装備アイテム生成の結果予測性
- **強型付き装備管理**: タイプセーフな装備システム

## 関連ドキュメント

- [Item.md](../domains/Item.md) - 装備アイテム実装詳細
- [Inventory.md](../domains/Inventory.md) - 装備インベントリ管理
- [Character.md](../domains/Character.md) - 装備制限・効果適用
- [BattleSystem.md](./BattleSystem.md) - 戦闘パラメータ連携
- [InventoryManagementSystem.md](./InventoryManagementSystem.md) - インベントリ管理
