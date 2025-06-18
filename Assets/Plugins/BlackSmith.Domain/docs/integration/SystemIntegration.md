# システム統合パターン

## 概要

BlackSmith.Domain における技術的なシステム統合の現状と将来的な拡張方針を説明します。\
現在は基本的なドメイン分離とインターフェース設計による統合基盤が構築されています。

**実装状況**: 基本的なドメイン分離とインターフェース統合が実装済み。高度な統合パターンは将来的な拡張です。

## 現在の統合基盤

### ドメイン分離アーキテクチャ

**実装済み**: 各ドメインは独立したディレクトリ構造で分離されており、明確な責務境界を持っています。

```
Domain/
├── Character/     # プレイヤー・戦闘関連
├── Item/         # アイテム・装備関連
├── Inventory/    # 所有・通貨管理関連
├── Field/        # 世界・位置関連
├── Skill/        # スキル・能力関連
├── PassiveEffect/ # 効果・ステータス関連
└── Quest/        # クエスト・依頼関連
```

### インターフェース統合パターン

**実装済み**: 型制約とインターフェースによるドメイン間連携

#### 1. 共通インターフェース活用
```csharp
// IItem インターフェースによる統合
internal class InfiniteSlotInventory : IInventoryService
{
    private Dictionary<IItem, ItemSlot> ItemSlots; // 全アイテム種別を統一管理
}
```

#### 2. 型制約による分離
```csharp
// 型制約による専用管理
internal class EquipmentInventory : IOneByInventoryService<EquippableItem>
{
    private readonly Dictionary<EquipmentType, EquippableItem> Equipments;
}
```

### ID参照による疎結合

**実装済み**: 強い依存関係を避けたID参照システム

#### CharacterID参照パターン
```csharp
// Item ドメイン: 制作者として参照
public interface ICraftableItem : IItem
{
    CharacterID CreatedBy { get; }
}

// Field ドメイン: 位置管理で参照
public class Field
{
    public IReadOnlyCollection<CharacterID> CharacterIds { get; }
}

// Quest ドメイン: 依頼人として参照
internal class QuestModel
{
    private CharacterID ClientId { get; }
}
```

### 内包による統合パターン

**実装済み**: 関連度の高いコンポーネントの内包管理

```csharp
// Character ドメイン内でのPassiveEffect統合
public record PlayerBattleEntity
{
    public BattleStatusEffectModule StatusEffects { get; }
}

public record BattleStatusEffectModule
{
    public IReadOnlyCollection<BattleStatusEffect> StatusEffects { get; }
}
```

## 将来的な統合拡張ポイント

現在の基本的な統合基盤により、以下の高度な統合パターンを段階的に実装していくことが可能です：

### 高度なアーキテクチャパターン
- **ファサードパターン**: 複数ドメインの複雑な相互作用を単一インターフェースで統合
- **メディエーターパターン**: ドメイン間通信の中央集権的管理
- **イベント駆動アーキテクチャ**: ドメインイベントによる疎結合な連携

### データ整合性パターン
- **トランザクション境界**: ドメイン横断操作の一貫性保証
- **補償トランザクション**: 分散システムでの整合性回復
- **イベントソーシング**: 状態変更の完全な履歴管理

### パフォーマンス最適化
- **非同期処理パイプライン**: 重い処理の背景実行
- **キャッシュ統合**: ドメイン間の効率的なデータ共有
- **バッチ処理**: 複数操作の効率的な一括実行

現在の基本的な連携基盤により、これらの高度な機能を必要に応じて段階的に導入していくことができます。

## 関連ドキュメント

- [DomainInteractions.md](./DomainInteractions.md) - ドメイン間の基本的な連携関係
- [domains/](../domains/) - 各ドメインの詳細実装
- [systems/](../systems/) - 具体的なゲーム機能システム
