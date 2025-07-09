# インベントリ管理システム (Inventory Management System)

## 概要

インベントリ管理システムは、Inventory と Item ドメインを統合し、アイテム所有・管理に関するドメインサービスを提供します。\
アイテム所有管理、インベントリ種別管理、アイテム移動処理、アイテム検索の各ドメインサービスにより、\
ライブラリ利用者はアイテム所有ロジックを構築できます。

## ドメイン問題の定義

### インベントリドメインが扱う問題領域
- **アイテム所有状態管理**: プレイヤーのアイテム所有状況とその変更
- **アイテム数量管理**: 同種アイテムのスタック数と数量増減
- **インベントリ種別管理**: 汎用・装備等の用途別アイテム分類
- **アイテム移動処理**: インベントリ間の安全なアイテム移動
- **アイテム検索・確認**: 所有アイテムの検索と数量確認

### ドメイン間の関係性
- **Inventory**: インベントリサービスとスロット管理の定義
- **Item**: アイテム種別とインベントリ制約の定義

## 提供するドメインサービス

### アイテム所有管理サービス
アイテムの追加、削除、数量変更のドメインサービスを提供します。

**提供する操作:**
- アイテムの所有状態への追加・削除
- 同種アイテムの数量増減管理
- アイテム追加可能性の事前判定
- 所有アイテムの数量確認

**ドメインルール:**
- アイテム数量は0以下にならない
- 同種アイテムは自動的にスタック管理
- 数量変更操作はアトミックに実行

### インベントリ種別管理サービス
用途別インベントリによる分類管理を提供します。

**提供する機能:**
- 汎用インベントリでの無制限アイテム管理
- 装備インベントリでの種別制限管理
- インベントリ種別による制約適用
- 動的なインベントリ作成

**種別ルール:**
- 汎用インベントリは容量無制限
- 装備インベントリは同種装備1つまで
- アイテム種別により管理方式を決定

### アイテム移動サービス
インベントリ間の安全なアイテム移動処理を提供します。

**提供する移動操作:**
- インベントリ間のアイテム転送
- 移動前の制約チェック
- 移動処理のアトミック実行
- 移動失敗時のロールバック

**移動制約:**
- 移動先インベントリの制約確認
- 移動元からの削除と移動先への追加をアトミック実行
- 制約違反時は操作を中止

### アイテム検索・確認サービス
所有アイテムの検索と状態確認を提供します。

**提供する検索機能:**
- 特定アイテムの所有確認
- アイテム数量の取得
- インベントリ内アイテム一覧取得
- アイテム種別による絞り込み

## ビジネスルール・制約

### アイテム数量制約
- **非負制約**: アイテム数量は0以上のみ有効
- **スタック制約**: 同種アイテムは自動的にスタック管理
- **減算制約**: 減算時は十分な数量が必要

### インベントリ種別制約
- **汎用制約**: InfiniteSlotInventoryは容量無制限
- **装備制約**: EquipmentInventoryは同種装備1つまで
- **型制約**: アイテム種別により管理インベントリを決定

### アイテム移動制約
- **制約確認**: 移動前の移動先制約チェック
- **アトミック移動**: 削除と追加の原子性保証
- **失敗時保証**: 移動失敗時の元状態維持

### ドメイン整合性
- **スロット状態**: ItemSlotの数量とアイテムの整合性
- **インベントリ同期**: 複数インベントリ間の状態一貫性
- **型安全性**: アイテム種別とインベントリ種別の整合性

## 実装状況

### Domain層実装
- ✅ **InfiniteSlotInventoryクラス**: 汎用インベントリの完全実装
- ✅ **EquipmentInventoryクラス**: 装備専用インベントリ
- ✅ **ItemSlotクラス**: アイテムスロットの数量管理
- ✅ **IInventoryServiceインターフェース**: インベントリ操作の抽象化
- ✅ **IOneByInventoryServiceインターフェース**: 単品管理の抽象化

### Usecase層実装
- ✅ **InventoryUseCaseクラス**: インベントリ作成とアイテム操作
- ✅ **ItemTradeUsecaseクラス**: インベントリ間アイテム移動
- ✅ **IInventoryCreateUsecaseインターフェース**: インベントリ作成の抽象化

### インターフェース定義
- ✅ **IInventoryService**: 汎用インベントリ操作
- ✅ **IOneByInventoryService**: 単品インベントリ操作
- ✅ **IInventoryCreateUsecase**: インベントリ作成操作

## ドメインサービス API

### アイテム所有管理サービス
```csharp
// 汎用インベントリでのアイテム管理
internal class InfiniteSlotInventory : IInventoryService
{
    public bool IsAddable(IItem item, int count = 1);        // 追加可能性判定
    public IItem AddItem(IItem item, int count = 1);         // アイテム追加
    public IItem RemoveItem(IItem item, int count = 1);      // アイテム削除
    public bool Contains(IItem item);                        // 所有確認
    public int GetContainItemCount(IItem item);              // 数量取得
}

// アイテムスロットでの数量管理
internal class ItemSlot
{
    public IItem Item { get; }                               // スロットアイテム
    public ItemCountNumber Count { get; }                    // 所有数量
    
    internal ItemSlot AddCount(ItemCountNumber count);       // 数量増加
    internal ItemSlot SubtractCount(ItemCountNumber count);  // 数量減少
    internal bool IsContaining(IItem item);                  // アイテム一致確認
}
```

### インベントリ種別管理サービス
```csharp
// 装備専用インベントリ管理
internal class EquipmentInventory : IOneByInventoryService<EquippableItem>
{
    public bool IsAddable(EquippableItem item);             // 装備可能性判定
    public EquippableItem AddItem(EquippableItem item);     // 装備追加
    public EquippableItem RemoveItem(EquippableItem item);  // 装備削除
    public bool Contains(EquippableItem item);              // 装備確認
}

// インベントリ作成サービス
public class InventoryUseCase : IInventoryCreateUsecase
{
    internal async UniTask<IInventoryService> CreateInventory();  // インベントリ作成
    internal async UniTask AddItemToInventory(IInventoryService inventory, 
        Item item, int count);                                    // アイテム追加
}
```

### アイテム移動サービス
```csharp
// インベントリ間アイテム移動
public class ItemTradeUsecase
{
    // ジェネリック型による安全なアイテム移動
    internal async UniTask Transfer<T>(IInventoryService<T> fromInventory, 
        IInventoryService<T> toInventory, T item) where T : IItem;
}
```

### 基本的な利用パターン
1. **インベントリ作成**: `CreateInventory()` で用途別インベントリを作成
2. **アイテム管理**: `AddItem()`, `RemoveItem()` でアイテム所有状態を変更
3. **移動処理**: `Transfer()` でインベントリ間の安全なアイテム移動
4. **状態確認**: `Contains()`, `GetContainItemCount()` で所有状況を確認

### ドメインサービスの特徴
- **UniTask対応**: インベントリ操作の非同期実行
- **型安全性**: ジェネリック型によるタイプセーフなアイテム管理
- **アトミック操作**: アイテム操作の原子性保証
- **制約適用**: インベントリ種別による自動制約チェック

## 関連ドキュメント

- [Inventory.md](../domains/Inventory.md) - インベントリ実装詳細
- [Item.md](../domains/Item.md) - アイテム種別詳細
- [CurrencySystem.md](./CurrencySystem.md) - 通貨・売買システム
- [EquipmentSystem.md](./EquipmentSystem.md) - 装備管理システム