# Inventory ドメイン

## 概要

Inventory ドメインは、プレイヤーのアイテム所有・管理システムを担当します。\
装備インベントリ、一般インベントリ、通貨システム、アイテム取引システムを統合的に管理します。

## ドメインモデル

### 基底クラス

#### BaseInventory
```csharp
// 【未実装】統一されたインベントリ基底クラス
public abstract record BaseInventory
{
    protected readonly ImmutableArray<ItemSlot> slots;
    
    public ImmutableArray<ItemSlot> Slots => slots;
    
    public abstract bool CanAddItem(IItem item, int quantity);
    public abstract BaseInventory AddItem(IItem item, int quantity);
    public abstract BaseInventory RemoveItem(IItem item, int quantity);
    public abstract int GetItemQuantity(IItem item);
}
```

#### ItemSlot
```csharp
// 【部分実装】基本的なスロット機能は実装済みだが、record型ではない
public record ItemSlot
{
    public IItem Item { get; }
    public int Quantity { get; }
    
    public ItemSlot(IItem item, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive");
        
        Item = item;
        Quantity = quantity;
    }
    
    public ItemSlot AddQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");
        
        return this with { Quantity = Quantity + amount };
    }
    
    public ItemSlot RemoveQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");
        
        if (amount > Quantity)
            throw new InvalidOperationException("Cannot remove more items than available");
        
        return this with { Quantity = Quantity - amount };
    }
}
```

### 装備インベントリ

#### EquipmentInventory
```csharp
// 【部分実装】基本的な装備管理は実装済みだが、record型ではなくclass型で実装
public record EquipmentInventory : BaseInventory
{
    public EquippableItem? WeaponSlot { get; }
    public EquippableItem? ArmorSlot { get; }
    public EquippableItem? AccessarySlot { get; }
    
    public EquipmentInventory() : this(null, null, null) { }
    
    public EquipmentInventory(
        EquippableItem? weaponSlot,
        EquippableItem? armorSlot,
        EquippableItem? accessarySlot)
    {
        WeaponSlot = weaponSlot;
        ArmorSlot = armorSlot;
        AccessarySlot = accessarySlot;
    }
    
    public override bool CanAddItem(IItem item, int quantity)
    {
        if (item is not EquippableItem equipItem)
            return false;
        
        if (quantity != 1)
            return false; // 装備は1つまで
        
        return equipItem.EquipmentType switch
        {
            EquipmentType.Weapon => WeaponSlot == null,
            EquipmentType.Armor => ArmorSlot == null,
            EquipmentType.Accessary => AccessarySlot == null,
            _ => false
        };
    }
    
    public EquipmentInventory EquipItem(EquippableItem item)
    {
        if (!CanAddItem(item, 1))
            throw new InvalidOperationException("Cannot equip item");
        
        return item.EquipmentType switch
        {
            EquipmentType.Weapon => this with { WeaponSlot = item },
            EquipmentType.Armor => this with { ArmorSlot = item },
            EquipmentType.Accessary => this with { AccessarySlot = item },
            _ => throw new ArgumentException("Invalid equipment type")
        };
    }
    
    public EquipmentInventory UnequipItem(EquipmentType equipmentType)
    {
        return equipmentType switch
        {
            EquipmentType.Weapon => this with { WeaponSlot = null },
            EquipmentType.Armor => this with { ArmorSlot = null },
            EquipmentType.Accessary => this with { AccessarySlot = null },
            _ => throw new ArgumentException("Invalid equipment type")
        };
    }
}
```

### 無制限インベントリ

#### InfiniteSlotInventory
```csharp
// 【部分実装】基本的な無制限インベントリは実装済み
public record InfiniteSlotInventory : BaseInventory
{
    public InfiniteSlotInventory() : this(ImmutableArray<ItemSlot>.Empty) { }
    
    public InfiniteSlotInventory(ImmutableArray<ItemSlot> slots)
    {
        this.slots = slots;
    }
    
    public override bool CanAddItem(IItem item, int quantity)
    {
        if (quantity <= 0) return false;
        
        var existingSlot = FindItemSlot(item);
        if (existingSlot != null)
        {
            var maxStack = GetMaxStackSize(item);
            return existingSlot.Quantity + quantity <= maxStack;
        }
        
        return true; // 新規スロット作成可能
    }
    
    public override InfiniteSlotInventory AddItem(IItem item, int quantity)
    {
        if (!CanAddItem(item, quantity))
            throw new InvalidOperationException("Cannot add item");
        
        var existingSlotIndex = FindItemSlotIndex(item);
        if (existingSlotIndex >= 0)
        {
            // 既存スロットに追加
            var updatedSlot = slots[existingSlotIndex].AddQuantity(quantity);
            return this with { slots = slots.SetItem(existingSlotIndex, updatedSlot) };
        }
        else
        {
            // 新規スロット作成
            var newSlot = new ItemSlot(item, quantity);
            return this with { slots = slots.Add(newSlot) };
        }
    }
    
    public override InfiniteSlotInventory RemoveItem(IItem item, int quantity)
    {
        var slotIndex = FindItemSlotIndex(item);
        if (slotIndex < 0)
            throw new InvalidOperationException("Item not found");
        
        var currentSlot = slots[slotIndex];
        if (currentSlot.Quantity < quantity)
            throw new InvalidOperationException("Insufficient quantity");
        
        if (currentSlot.Quantity == quantity)
        {
            // スロット削除
            return this with { slots = slots.RemoveAt(slotIndex) };
        }
        else
        {
            // 数量減少
            var updatedSlot = currentSlot.RemoveQuantity(quantity);
            return this with { slots = slots.SetItem(slotIndex, updatedSlot) };
        }
    }
    
    private static int GetMaxStackSize(IItem item) => item switch
    {
        ICraftMaterialItem => 999,
        EquippableItem => 1,
        _ => 99
    };
}
```

### 通貨システム

#### Currency
```csharp
// 【部分実装】基本通貨機能は実装済みだが、両替機能は未実装
public record Currency
{
    public CurrencyType Type { get; }
    public int Value { get; }
    
    public Currency(CurrencyType type, int value)
    {
        if (value < 0)
            throw new ArgumentException("Currency value cannot be negative");
        
        Type = type;
        Value = value;
    }
    
    public Currency Add(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        return this with { Value = Value + amount };
    }
    
    public Currency Subtract(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        if (amount > Value)
            throw new InvalidOperationException("Insufficient funds");
        
        return this with { Value = Value - amount };
    }
    
    // 【未実装】両替機能
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
            (CurrencyType.Sakura, CurrencyType.Aren) => 0.8f,
            (CurrencyType.Aren, CurrencyType.Sakura) => 1.25f,
            _ => 1.0f
        };
    }
}

public enum CurrencyType
{
    Sakura = 1,  // 桜式通貨（基準通貨）
    Aren = 2,    // アレン式通貨
}
```

#### Wallet
```csharp
// 【部分実装】基本的なウォレット機能は実装済み
public record Wallet
{
    private readonly ImmutableDictionary<CurrencyType, Currency> currencies;
    
    public Wallet() : this(ImmutableDictionary<CurrencyType, Currency>.Empty) { }
    
    public Wallet(ImmutableDictionary<CurrencyType, Currency> currencies)
    {
        this.currencies = currencies;
    }
    
    public int GetCurrencyAmount(CurrencyType type)
    {
        return currencies.TryGetValue(type, out var currency) ? currency.Value : 0;
    }
    
    public bool CanAfford(CurrencyType type, int amount)
    {
        return GetCurrencyAmount(type) >= amount;
    }
    
    public Wallet AddCurrency(Currency currency)
    {
        var currentAmount = GetCurrencyAmount(currency.Type);
        var newCurrency = new Currency(currency.Type, currentAmount + currency.Value);
        
        return this with { currencies = currencies.SetItem(currency.Type, newCurrency) };
    }
    
    public Wallet SubtractCurrency(CurrencyType type, int amount)
    {
        if (!CanAfford(type, amount))
            throw new InvalidOperationException("Insufficient funds");
        
        var current = GetCurrencyAmount(type);
        var newCurrency = new Currency(type, current - amount);
        
        return this with { currencies = currencies.SetItem(type, newCurrency) };
    }
}
```

## ビジネスルール

### 容量制限

#### 装備インベントリ制限
- **各装備種別**: 1つまで固定
- **装備変更**: 同種別の装備は自動的に置き換え

#### 無制限インベントリ制限
- **スタック制限**: アイテム種別により異なる
  - 素材アイテム: 999個まで
  - 装備アイテム: 1個まで
  - その他アイテム: 99個まで

### アイテム移動制限

#### 移動可能条件
```csharp
// 【未実装】インベントリ間のアイテム移動システム
public static bool CanMoveItem(IItem item, BaseInventory from, BaseInventory to, int quantity)
{
    // 移動元チェック
    if (from.GetItemQuantity(item) < quantity)
        return false;
    
    // 移動先チェック
    if (!to.CanAddItem(item, quantity))
        return false;
    
    return true;
}
```

### 通貨両替制限

#### 両替レート
- **桜式通貨 → アレン式通貨**: 0.8倍
- **アレン式通貨 → 桜式通貨**: 1.25倍
- **同一通貨**: 1.0倍（変換なし）

#### 両替手数料
現在の実装では手数料なし（将来拡張可能）

## ゲームロジック

### インベントリ操作

```csharp
// 【未実装】統一されたアイテム追加システム
public static InfiniteSlotInventory AddItemSafely(
    InfiniteSlotInventory inventory, 
    IItem item, 
    int quantity)
{
    if (!inventory.CanAddItem(item, quantity))
        throw new InvalidOperationException($"Cannot add {quantity} of {item.ItemName}");
    
    return inventory.AddItem(item, quantity);
}

// 【未実装】インベントリ間のアイテム移動システム
public static (BaseInventory updatedFrom, BaseInventory updatedTo) MoveItem(
    BaseInventory from, 
    BaseInventory to, 
    IItem item, 
    int quantity)
{
    if (!CanMoveItem(item, from, to, quantity))
        throw new InvalidOperationException("Cannot move item");
    
    var newFrom = from.RemoveItem(item, quantity);
    var newTo = to.AddItem(item, quantity);
    
    return (newFrom, newTo);
}
```

### 装備管理

```csharp
// 【未実装】統合された装備変更システム
public static (EquipmentInventory equipment, InfiniteSlotInventory general) 
    ChangeEquipment(
        EquipmentInventory equipmentInventory,
        InfiniteSlotInventory generalInventory,
        EquippableItem newEquipment)
{
    var equipmentType = newEquipment.EquipmentType;
    
    // 既存装備を一般インベントリに移動
    var currentEquipment = equipmentType switch
    {
        EquipmentType.Weapon => equipmentInventory.WeaponSlot,
        EquipmentType.Armor => equipmentInventory.ArmorSlot,
        EquipmentType.Accessary => equipmentInventory.AccessarySlot,
        _ => null
    };
    
    var updatedGeneral = generalInventory;
    if (currentEquipment != null)
    {
        updatedGeneral = updatedGeneral.AddItem(currentEquipment, 1);
    }
    
    // 新装備を一般インベントリから削除
    updatedGeneral = updatedGeneral.RemoveItem(newEquipment, 1);
    
    // 新装備を装備
    var updatedEquipment = equipmentInventory.EquipItem(newEquipment);
    
    return (updatedEquipment, updatedGeneral);
}
```

### 通貨管理

```csharp
// 【未実装】購入処理システム
public static Wallet ProcessPurchase(Wallet wallet, CurrencyType currencyType, int price)
{
    if (!wallet.CanAfford(currencyType, price))
        throw new InvalidOperationException("Insufficient funds");
    
    return wallet.SubtractCurrency(currencyType, price);
}

// 【未実装】売却処理システム
public static Wallet ProcessSale(Wallet wallet, CurrencyType currencyType, int revenue)
{
    var currency = new Currency(currencyType, revenue);
    return wallet.AddCurrency(currency);
}

// 【未実装】通貨両替システム
public static Wallet ExchangeCurrency(Wallet wallet, CurrencyType from, CurrencyType to, int amount)
{
    if (!wallet.CanAfford(from, amount))
        throw new InvalidOperationException("Insufficient funds for exchange");
    
    var sourceCurrency = new Currency(from, amount);
    var targetCurrency = sourceCurrency.Exchange(to);
    
    return wallet
        .SubtractCurrency(from, amount)
        .AddCurrency(targetCurrency);
}
```

## 他ドメインとの連携

### Item ドメインとの連携
- **アイテム管理**: 全てのアイテムの所有・保存
- **装備効果**: 装備中のアイテム効果の適用
- 詳細: [Item.md](./Item.md)

### Character ドメインとの連携
- **装備システム**: 戦闘パラメータへの装備効果反映
- **レベル制限**: 装備可能条件チェック（将来拡張）
- 詳細: [EquipmentSystem.md](../systems/EquipmentSystem.md)

### Skill ドメインとの連携
- **クラフト素材**: 生産スキル使用時の素材管理
- **スキル制限**: 特定アイテム使用のスキル要件（将来拡張）
- 詳細: [CraftingSystem.md](../systems/CraftingSystem.md)

## 拡張ポイント

### インベントリ容量制限
```csharp
// 【未実装】容量制限システム
public record LimitedInventory : BaseInventory
{
    public int MaxSlots { get; }
    public int CurrentSlots => slots.Length;
    
    public override bool CanAddItem(IItem item, int quantity)
    {
        if (FindItemSlot(item) != null)
            return true; // 既存スロットに追加
        
        return CurrentSlots < MaxSlots; // 新規スロット作成可能
    }
}
```

### アイテム重量システム
```csharp
// 【未実装】重量制限システム
public record Weight
{
    public float Value { get; }
}

public interface IItem
{
    string ItemName { get; }
    Weight Weight { get; } // 追加
}

public record WeightLimitedInventory : BaseInventory
{
    public Weight MaxWeight { get; }
    public Weight CurrentWeight => CalculateCurrentWeight();
}
```

### 取引履歴システム
```csharp
// 【未実装】取引履歴の記録システム
public record TradeHistory
{
    public DateTime Timestamp { get; }
    public TradeType Type { get; }
    public IItem Item { get; }
    public int Quantity { get; }
    public Currency Price { get; }
}

public enum TradeType
{
    Purchase,
    Sale,
    Craft,
    Drop
}
```

Inventory ドメインは、プレイヤーの所有アイテムとリソースを一元管理する重要なドメインです。  
他のドメインと密接に連携し、ゲームの経済システムを支えています。