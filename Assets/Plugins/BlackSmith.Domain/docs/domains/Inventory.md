# Inventory ドメイン

## 概要

Inventory ドメインは、プレイヤーのアイテム所有・管理システムを担当するドメインです。

### 主要な責務
- **アイテム管理**：プレイヤーが所有するアイテムの保存・追加・削除
- **装備管理**：装備アイテムの装着・取り外し管理
- **通貨管理**：ゲーム内通貨の所持・使用・両替
- **インベントリ操作**：アイテムの移動・整理・検索

### 現在の実装状況
- **InfiniteSlotInventory**：無制限容量の一般インベントリを実装
- **EquipmentInventory**：装備専用インベントリを実装
- **Wallet**：通貨管理システムを実装
- **ItemSlot**：アイテム数量管理の値オブジェクトを実装
- **InventoryFactory**：インベントリ作成ファクトリーを実装

### 設計の特徴
- **インターフェースベース設計**：BaseInventory基底クラスを使わず、インターフェースで機能を分離
- **Item ドメイン連携**：IItemインターフェースを通じてアイテム情報を管理
- **通貨システム統合**：Currency recordによる型安全な通貨処理
- **拡張性重視**：将来の容量制限・重量制限・取引履歴に対応可能な基盤設計

このドメインは Item ドメインとの密接な連携により、ゲーム内経済システムの基盤を提供しています。

## ドメインモデル

### 識別子

#### 実装済み識別子

現在のInventoryドメインでは、識別子は他のドメインのIDを参照する形で使用されています：

```csharp
// 実装なし：InventoryID は現在未実装
// 各インベントリクラスは独自の識別子を持たない設計
```

**注意**：現在の実装では以下の識別子が未実装です：
- `InventoryID` - インベントリの識別子
- `WalletID` - ウォレットの識別子

### エンティティ・値オブジェクト

#### 実装済みエンティティ

**重要**：現在の実装では`BaseInventory`基底クラスは存在せず、インターフェースベースの設計を採用しています。

##### ItemSlot (ItemSlot.cs)
```csharp
// 実装済み：アイテム所有数管理の値オブジェクト
internal class ItemSlot
{
    internal IItem Item { get; }
    internal ItemCountNumber Count { get; }
    
    internal ItemSlot(IItem item, ItemCountNumber count);
    internal ItemSlot AddItem(IItem item, ItemCountNumber count = null!);
    internal ItemSlot RemoveItem(IItem item, ItemCountNumber count = null!);
    internal bool IsContaining(IItem item);
}
```

##### ItemCountNumber (ItemSlot.cs)
```csharp
// 実装済み：アイテム数量管理の値オブジェクト
internal class ItemCountNumber : IEquatable<ItemCountNumber>
{
    internal int Value { get; }
    
    internal ItemCountNumber(int value);
    internal ItemCountNumber Add(ItemCountNumber count);
    internal ItemCountNumber Subtract(ItemCountNumber count);
    
    // 比較演算子とEquals実装
    public bool Equals(ItemCountNumber? other);
    public static bool operator ==(ItemCountNumber x, ItemCountNumber y);
    // その他の比較演算子...
}
```

##### ItemHoldableCapacity (ItemSlot.cs)
```csharp
// 実装済み：アイテム保持容量管理
internal class ItemHoldableCapacity
{
    internal ItemCountNumber Value { get; }
    private const int MAXIMUM_CAPACITY = 64;
    
    internal ItemHoldableCapacity(ItemCountNumber capacity);
}
```

#### 実装済みインターフェース

##### インベントリサービスインターフェース (IInventoryService.cs)
```csharp
// 実装済み：インベントリの基本操作インターフェース
public interface IInventoryService : IInventoryService<IItem>, IInventoryStateViewable<IItem> { }

public interface IInventoryService<T> : IInventoryStateViewable<T> where T : IItem
{
    T AddItem(T item, int count = 1!);
    T RemoveItem(T item, int count = 1!);
    bool Contains(T item);
    bool IsAddable(T item, int count = 1!);
}

public interface IOneByInventoryService<T> : IInventoryStateViewable<T> where T : IItem
{
    T AddItem(T item);
    T RemoveItem(T item);
    bool Contains(T item);
    bool IsAddable(T item);
}

public interface IInventoryStateViewable<T> where T : IItem
{
    IReadOnlyDictionary<T, int> GetInventory();
    IReadOnlyCollection<T> GetContainItems();
    int GetContainItemCount(T item);
}

public interface IWallet
{
    void AdditionMoney(Currency money);
    void SubtractMoney(Currency money);
    IReadOnlyCollection<Currency> GetMoney();
    Currency GetMoney(CurrencyType type);
    bool ContainsType(CurrencyType type);
}
```

#### 実装済みインベントリクラス

##### InfiniteSlotInventory (InfiniteSlotInventory.cs)
```csharp
// 実装済み：無制限容量インベントリ
internal class InfiniteSlotInventory : IInventoryService
{
    private InventoryID ID { get; }
    private Dictionary<IItem, ItemSlot> ItemSlots { get; }
    
    internal InfiniteSlotInventory(InventoryID id);
    
    public IItem AddItem(IItem item, int count = 1!);
    public IItem RemoveItem(IItem item, int count = 1!);
    public bool Contains(IItem item);
    public bool IsAddable(IItem item, int count = 1);
    public ItemCountNumber ContainingItemCount(IItem item);
    
    // IInventoryStateViewable実装
    public IReadOnlyDictionary<IItem, int> GetInventory();
    IReadOnlyCollection<IItem> GetContainItems();
    int GetContainItemCount(IItem item);
}

// 実装済み：インベントリ識別子
public record InventoryID : BasicID
{
    protected override string Prefix => "Inventory-";
}

// 実装済み：インベントリ容量クラス
public class InventoryCapacity
{
    public int Value { get; }
    public InventoryCapacity(int value);
}
```

##### EquipmentInventory (EquipmentInventory.cs)
```csharp
// 実装済み：装備専用インベントリ
internal class EquipmentInventory : IOneByInventoryService<EquippableItem>
{
    internal InventoryID ID { get; }
    private readonly Dictionary<EquipmentType, EquippableItem> Equipments;
    
    internal EquipmentInventory(InventoryID id);
    
    public EquippableItem AddItem(EquippableItem item);
    public EquippableItem RemoveItem(EquippableItem item);
    public bool Contains(EquippableItem item);
    public bool IsAddable(EquippableItem item);
    
    // 装備固有機能
    private bool IsOccupiedType(EquipmentType type);
    
    // IInventoryStateViewable実装
    public IReadOnlyDictionary<EquippableItem, int> GetInventory();
    public IReadOnlyCollection<EquippableItem> GetContainItems();
    public int GetContainItemCount(EquippableItem item);
}
```

#### 実装済み通貨システム

##### Currency (Currency.cs)
```csharp
// 実装済み：通貨レコード
public record Currency
{
    public CurrencyType Type { get; }
    public int Value => value.Value;
    private readonly CurrencyValue value;
    
    internal Currency(CurrencyType type, int value);
    internal Currency Add(Currency other);
    internal Currency Subtract(Currency other);
    internal Currency Exchange(CurrencyType type); // 実装済み
    public bool EqualsType(Currency other);
    
    // 内部値オブジェクト
    private record CurrencyValue
    {
        public int Value { get; }
        internal CurrencyValue(int value);
        internal CurrencyValue Add(CurrencyValue other);
        internal CurrencyValue Subtract(CurrencyValue other);
        public static bool IsValidValue(int value);
    }
}

// 実装済み：通貨タイプ（注意：enum値が両替レートを表す問題あり）
public enum CurrencyType
{
    Sakura = 1, // 桜式通貨
    Aren = 2,   // アレン式通貨
}
```

##### Wallet (Wallet.cs)
```csharp
// 実装済み：ウォレットクラス
public class Wallet : IWallet
{
    private Dictionary<CurrencyType, Currency> Money;
    
    public Wallet();
    public Wallet(IReadOnlyCollection<Currency> money);
    
    public void AdditionMoney(Currency money);
    public void SubtractMoney(Currency money);
    public IReadOnlyCollection<Currency> GetMoney();
    public Currency GetMoney(CurrencyType type);
    public bool ContainsType(CurrencyType type);
}
```

#### 未実装エンティティ

**注意**：ドキュメントに記載されていた以下のクラスは実装されていません：
- `BaseInventory` - 基底クラス（インターフェースベース設計のため不要）
- `LimitedInventory` - 容量制限インベントリ
- `WeightLimitedInventory` - 重量制限インベントリ

## ビジネスルール

### 実装済みルール

#### InfiniteSlotInventoryの制限
```csharp
// InfiniteSlotInventory.cs の実装済み機能
internal class InfiniteSlotInventory : IInventoryService
{
    // アイテム追加制限
    public bool IsAddable(IItem item, int count = 1)
    {
        if (item is null) return false;
        return true; // 現在は容量制限なし
    }
    
    // 基本的な追加・削除
    public IItem AddItem(IItem item, int count = 1!);
    public IItem RemoveItem(IItem item, int count = 1!);
}
```

#### EquipmentInventoryの制限
```csharp
// EquipmentInventory.cs の実装済み機能
internal class EquipmentInventory : IOneByInventoryService<EquippableItem>
{
    // 装備制限：各装備タイプ1つまで
    public bool IsAddable(EquippableItem item)
    {
        if (item is null) return false;
        return !IsOccupiedType(item.EquipType); // 同タイプが装備されていないかチェック
    }
    
    // 装備タイプ別管理
    private readonly Dictionary<EquipmentType, EquippableItem> Equipments;
}
```

#### 通貨システムの制限
```csharp
// Currency.cs の実装済み機能
public record Currency
{
    // 通貨値検証
    private record CurrencyValue
    {
        public static bool IsValidValue(int value)
        {
            return value >= 0; // 負の値は無効
        }
    }
    
    // 同種通貨のみ計算可能
    internal Currency Add(Currency other)
    {
        if (!Type.Equals(other.Type))
            throw new ArgumentException("通貨単位が一致していません");
        // ...
    }
}

// Wallet.cs の実装済み機能
public class Wallet : IWallet
{
    // 通貨タイプ別管理
    private Dictionary<CurrencyType, Currency> Money;
    
    // 残高不足チェック
    public void SubtractMoney(Currency money)
    {
        if (!ContainsType(money.Type))
            throw new ArgumentException("指定型のお金を所持していません");
        // 減算処理でCurrency内部で残高チェック
    }
}
```

#### ItemSlotの制限
```csharp
// ItemSlot.cs の実装済み機能
internal class ItemCountNumber
{
    internal ItemCountNumber(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value)); // 負の値は無効
    }
    
    internal ItemCountNumber Subtract(ItemCountNumber count)
    {
        var result = Value - count.Value;
        if (result < 0)
            throw new Exception("計算後の値が規定値を超えました。"); // 負の結果は無効
    }
}

internal class ItemHoldableCapacity
{
    private const int MAXIMUM_CAPACITY = 64; // 最大容量制限
}
```

### 未実装ルール

**注意**：以下のビジネスルールはドキュメントに記載されていますが、現在未実装です：

#### スタック制限
- **アイテム種別による制限**: 素材999個、装備1個、その他99個
- **現在の実装**: 制限なし（InfiniteSlotInventoryは無制限）

#### インベントリ間アイテム移動
- **アイテム移動システム**: 異なるインベントリ間での移動機能
- **移動可能条件チェック**: 移動元・移動先の容量チェック

#### 通貨両替システム
- **両替レート管理**: 現在はenumの値を直接使用（問題のある実装）
- **両替手数料**: 未実装

## ゲームロジック

### 実装済み機能

#### インベントリ作成
```csharp
// InventoryFactory.cs の実装済み機能
public class InventoryFactory
{
    // 一般インベントリ作成
    public IInventoryService Create()
    {
        InventoryID id = new InventoryID();
        var inventory = new InfiniteSlotInventory(id);
        return inventory;
    }
    
    // 装備インベントリ作成
    public IOneByInventoryService<EquippableItem> CreateEquipInventory()
    {
        InventoryID id = new InventoryID();
        var inventory = new EquipmentInventory(id);
        return inventory;
    }
}
```

#### アイテム操作
```csharp
// InfiniteSlotInventory.cs の実装済み機能
var factory = new InventoryFactory();
var inventory = factory.Create();

// アイテム追加
IItem addedItem = inventory.AddItem(item, 5);

// アイテム削除
IItem removedItem = inventory.RemoveItem(item, 2);

// アイテム存在確認
bool hasItem = inventory.Contains(item);

// 追加可能チェック
bool canAdd = inventory.IsAddable(item, 3);

// 所持数確認
int count = inventory.GetContainItemCount(item);
```

#### 装備管理
```csharp
// EquipmentInventory.cs の実装済み機能
var factory = new InventoryFactory();
var equipInventory = factory.CreateEquipInventory();

// 装備追加
EquippableItem equipped = equipInventory.AddItem(weapon);

// 装備削除
EquippableItem unequipped = equipInventory.RemoveItem(weapon);

// 装備可能チェック
bool canEquip = equipInventory.IsAddable(armor);

// 装備確認
bool isEquipped = equipInventory.Contains(weapon);
```

#### 通貨管理
```csharp
// Wallet.cs の実装済み機能
var wallet = new Wallet();
var sakuraCurrency = new Currency(CurrencyType.Sakura, 1000);

// 通貨追加
wallet.AdditionMoney(sakuraCurrency);

// 通貨使用
wallet.SubtractMoney(new Currency(CurrencyType.Sakura, 500));

// 残高確認
Currency balance = wallet.GetMoney(CurrencyType.Sakura);
bool hasEnough = wallet.ContainsType(CurrencyType.Sakura);

// 通貨両替
Currency exchanged = sakuraCurrency.Exchange(CurrencyType.Aren);
```

### 未実装機能

**注意**：以下の機能はドキュメントに記載されていますが、現在未実装です：

#### インベントリ間アイテム移動
- **アイテム移動システム**: 異なるインベントリ間での移動機能
- **移動可能条件チェック**: 移動元・移動先の容量チェック
- **統合された装備変更**: 装備・一般インベントリ間の自動移動

#### 高度な通貨操作
- **購入処理システム**: アイテム購入時の通貨処理
- **売却処理システム**: アイテム売却時の通貨処理
- **通貨両替手数料**: 両替時の手数料計算

#### アイテムスタック制限
- **種別別制限**: 素材999個、装備1個、その他99個の制限機能


## 拡張ポイント

### 実装可能な拡張

#### 容量制限システム
```csharp
// 将来実装可能：容量制限付きインベントリ
public class LimitedInventory : IInventoryService
{
    private readonly int maxSlots;
    private readonly Dictionary<IItem, ItemSlot> ItemSlots;
    
    public bool CanAddItem(IItem item, int count = 1)
    {
        if (ItemSlots.ContainsKey(item))
            return true; // 既存スロットに追加
        
        return ItemSlots.Count < maxSlots; // 新規スロット作成可能
    }
}
```

#### アイテム重量システム  
```csharp
// 将来実装可能：重量制限システム
public interface IWeightedItem : IItem
{
    float Weight { get; }
}

public class WeightLimitedInventory : IInventoryService
{
    private readonly float maxWeight;
    
    public bool CanAddItem(IItem item, int count = 1)
    {
        if (item is IWeightedItem weighted)
        {
            var currentWeight = CalculateCurrentWeight();
            return currentWeight + (weighted.Weight * count) <= maxWeight;
        }
        return true;
    }
}
```

#### アイテムスタック制限システム
```csharp
// 将来実装可能：アイテム種別によるスタック制限
public interface IStackableItem : IItem
{
    int MaxStackSize { get; }
}

public class StackLimitedInventory : IInventoryService
{
    public bool CanAddItem(IItem item, int count = 1)
    {
        if (item is IStackableItem stackable && Contains(item))
        {
            var currentCount = GetContainItemCount(item);
            return currentCount + count <= stackable.MaxStackSize;
        }
        return true;
    }
}
```

#### 取引履歴システム
```csharp
// 将来実装可能：取引履歴記録システム
public record TradeHistory
{
    public DateTime Timestamp { get; }
    public TradeType Type { get; }
    public IItem Item { get; }
    public int Quantity { get; }
    public Currency? Price { get; }
}

public enum TradeType
{
    Add,      // アイテム取得
    Remove,   // アイテム使用/削除
    Purchase, // 購入
    Sale,     // 売却
    Craft,    // クラフト
    Drop      // ドロップ
}

public class TrackableInventory : IInventoryService
{
    private readonly List<TradeHistory> history = new();
    
    public IItem AddItem(IItem item, int count = 1)
    {
        var result = baseInventory.AddItem(item, count);
        history.Add(new TradeHistory 
        { 
            Timestamp = DateTime.Now, 
            Type = TradeType.Add, 
            Item = item, 
            Quantity = count 
        });
        return result;
    }
}
```

### 設計基盤の特徴

現在の実装は将来の拡張に対応できる柔軟な設計基盤を提供：

- **インターフェースベース設計**: 新しいインベントリタイプの追加が容易
- **ファクトリーパターン**: インベントリ作成ロジックの一元化
- **値オブジェクト**: ItemSlot、ItemCountNumberによる安全な数量管理
- **型安全性**: ジェネリクスによるコンパイル時型チェック