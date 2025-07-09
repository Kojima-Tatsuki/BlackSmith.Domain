# クラフトシステム (Crafting System)

## 概要

クラフトシステムは、Item、Inventory、Skill の3つのドメインを統合し、アイテムの作成・精錬・修理に関するドメインサービスを提供します。\
クラフトレシピ管理、素材消費処理、成果物生成、スキル連携の各ドメインサービスにより、\
ライブラリ利用者はクラフトロジックを構築できます。

## ドメイン問題の定義

### クラフトドメインが扱う問題領域
- **クラフトレシピ管理**: 素材と成果物の組み合わせ定義
- **素材消費処理**: インベントリからの素材適切な消費
- **成果物生成**: クラフト結果の適切なインベントリ追加
- **スキル連携**: 生産スキルとクラフト処理の統合
- **作成者管理**: クラフトアイテムの作成者記録

### ドメイン間の関係性
- **Item**: クラフトレシピ、素材アイテム、成果アイテムの定義
- **Inventory**: 素材の所有確認と消費、成果物の格納
- **Skill**: 生産スキルの熟練度とアクション連携

## ドメインサービス API

### クラフト実行サービス
```csharp
// クラフト処理の統合サービス
public class ItemCraftUsecase
{
    internal async UniTask Craft(CraftCommand command);
    internal bool IsCraftable(CraftCommand command);
    
    // クラフトコマンドパターン
    internal record CraftCommand(
        InventoryID FromInventoryId,    // 素材元インベントリ
        InventoryID ToInventoryId,      // 成果物格納先インベントリ
        IReadOnlyCollection<IItem> Materials,  // 必要素材
        IItem ResultItem                // 期待する成果物
    );
}
```

### クラフトレシピ管理
```csharp
// レシピ情報のイミュータブル管理
public class CraftingRecipe
{
    public IReadOnlyCollection<ICraftMaterialItem> RequiredMaterials { get; }
    public ICraftableItem ResultItem { get; }
    
    internal CraftingRecipe(
        IReadOnlyCollection<ICraftMaterialItem> requiredMaterials,
        ICraftableItem resultItem);
}
```

### アイテムインターフェース
```csharp
// クラフト可能アイテム
public interface ICraftableItem : IItem
{
    CharacterID CreatedBy { get; }  // 作成者記録
    IReadOnlyCollection<ICraftMaterialItem> GetRequireMaterials();
}

// 素材アイテムマーカー
public interface ICraftMaterialItem : IItem
{
    // マーカーインターフェース
}
```

### 基本的な利用パターン
1. **レシピ確認**: `CraftingRecipe` で必要素材と成果物を確認
2. **作成可能性判定**: `IsCraftable()` で素材所有状況をチェック
3. **クラフト実行**: `Craft()` で素材消費と成果物生成をアトミック実行
4. **結果確認**: 成果物に作成者IDが記録されることを確認

### ドメインサービスの特徴
- **UniTask対応**: クラフト処理の非同期実行
- **アトミック操作**: 素材消費と成果物生成の原子性保証
- **コマンドパターン**: クラフト操作の結果予測可能性
- **ドメイン連携**: Inventory、Skillドメインとのシームレスな統合

## 提供するドメインサービス

### クラフト実行サービス
素材消費と成果物生成のアトミックな処理を提供します。

**提供する操作:**
- クラフトコマンドによるアイテム制作
- 素材のインベントリからの消費
- 成果物のターゲットインベントリへの追加
- 作成可能性の事前判定

**ドメインルール:**
- 素材と成果物のインベントリ操作はアトミックに実行
- 素材不足時はクラフト実行を中止
- 成果物に作成者IDを自動記録

### クラフトレシピ管理
素材と成果物の組み合わせ定義を提供します。

**提供する機能:**
- 必要素材リストの定義
- 成果アイテムの特定
- レシピ情報のイミュータブル管理

### スキル連携サービス
生産スキルとクラフト処理の統合を提供します。

**提供する連携:**
- 生産スキルアクションの定義（Create/Refine/Repair）
- スキルレベルとクラフト品質の連携
- クラフト実行時のスキル経験値獲得

## ビジネスルール・制約

### クラフト実行制約
- **素材有無制約**: 必要素材が不足している場合はクラフト実行不可
- **インベントリ容量制約**: 成果物格納先インベントリに容量が必要
- **アトミック操作**: 素材消費と成果物生成は一体で実行

### アイテム種別制約
- **ICraftMaterialItem**: 素材として使用可能なアイテムのみ
- **ICraftableItem**: クラフト結果として生成されたアイテムのみ
- **作成者記録**: 全てのICraftableItemに作成者IDが記録される

### ドメイン整合性
- **インベントリ同期**: 素材消費と成果物追加の原子性保証
- **スキル連携**: クラフト実行時のスキル状態反映
- **レシピ整合性**: CraftingRecipeと実際のクラフト結果の一致

## 実装状況

### Domain層実装
- ✅ **ICraftableItemインターフェース**: クラフト可能アイテムの定義
- ✅ **ICraftMaterialItemインターフェース**: 素材アイテムのマーカー
- ✅ **CraftingRecipeクラス**: レシピ情報のイミュータブル管理
- ✅ **IProductionSkillインターフェース**: 生産スキルのアクション定義

### Usecase層実装
- ✅ **ItemCraftUsecase**: クラフト実行と判定ロジック
- ✅ **CraftCommand**: クラフト処理のコマンドパターン
- ✅ **インベントリ連携**: 素材消費と成果物格納の統合処理

### スキル連携実装
- ✅ **CreateSkillAction**: アイテム作成アクション
- ✅ **RefineSkillAction**: アイテム精錬アクション
- ✅ **RepairSkillAction**: アイテム修理アクション

## 関連ドキュメント

- [Item.md](../domains/Item.md) - クラフト関連実装詳細
- [Inventory.md](../domains/Inventory.md) - 素材・制作品管理
- [Skill.md](../domains/Skill.md) - 生産スキルシステム
- [InventoryManagementSystem.md](./InventoryManagementSystem.md) - アイテム管理システム
