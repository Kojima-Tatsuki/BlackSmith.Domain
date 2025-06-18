# 通貨システム (Currency System)

## 概要

通貨システムは、Inventory と Item ドメインを統合し、通貨に関するドメインサービスを提供します。\
通貨管理、売買取引、両替処理の各ドメインサービスにより、\
ライブラリ利用者は経済ロジックを構築できます。

## ドメイン問題の定義

### 通貨ドメインが扱う問題領域
- **通貨管理**: 複数種類の通貨の所有状態管理
- **通貨計算**: 同種通貨の加減算処理
- **両替処理**: 異なる通貨種別間の変換
- **売買取引**: アイテムと通貨の交換処理
- **価格管理**: アイテムの売買価格算定

### ドメイン間の関係性
- **Inventory**: ウォレット機能と通貨オブジェクトの定義
- **Item**: NPCショップ機能と売買インターフェースの定義

## 実装状況

### Domain層実装
- ✅ **Currencyレコード**: 通貨値と種別のイミュータブル管理
- ✅ **CurrencyType列挙**: 通貨種別の定義（Sakura、Aren）
- ✅ **Walletクラス**: 複数通貨の統合管理
- ✅ **NPCItemShopクラス**: アイテム売買機能
- ✅ **両替機能**: Currency.Exchange()メソッド

### Usecase層実装
- ❌ **統合通貨サービス**: 未実装（Domain層のみで完結）
- ❌ **売買取引Usecase**: 未実装（NPCItemShopで直接対応）

### インターフェース定義
- ✅ **IWalletインターフェース**: ウォレット機能の抽象化
- ✅ **IItemPurchasableインターフェース**: アイテム購入機能
- ✅ **IItemExhibitableインターフェース**: アイテム展示機能

## ドメインサービス API

### 通貨管理サービス
```csharp
// 通貨オブジェクトの操作
public record Currency
{
    public CurrencyType Type { get; }
    public int Value { get; }
    
    internal Currency Add(Currency other);        // 加算
    internal Currency Subtract(Currency other);   // 減算
    internal Currency Exchange(CurrencyType type); // 両替
}

// ウォレット管理
public class Wallet : IWallet
{
    public void AdditionMoney(Currency money);              // 通貨追加
    public void SubtractMoney(Currency money);              // 通貨減算
    public IReadOnlyCollection<Currency> GetMoney();        // 全通貨取得
    public Currency GetMoney(CurrencyType type);            // 種別指定取得
    public bool ContainsType(CurrencyType type);            // 種別存在確認
}
```

### 売買取引サービス
```csharp
// NPCショップ機能
public class NPCItemShop : IItemPurchasable, IItemExhibitable
{
    public IItem Purchase(IItem item, Currency.Currency money);  // アイテム購入
    public Currency.Currency GetPrice(IItem item);               // 価格取得
}
```

### 基本的な利用パターン
1. **通貨管理**: `Wallet` で複数種類の通貨を統合管理
2. **両替処理**: `Currency.Exchange()` で異なる通貨種別に変換
3. **売買取引**: `NPCItemShop` でアイテムと通貨を交換
4. **残高管理**: 取引前に十分な通貨所有を確認

### ドメインサービスの特徴
- **イミュータブル設計**: Currencyレコードによる安全性
- **固定レート**: enum値ベースの予測可能な両替
- **型安全性**: 強型付き通貨システム
- **Domain層完結**: Usecase層なしでも完全な機能提供

## 提供するドメインサービス

### 通貨管理サービス
複数種類の通貨の統合管理を提供します。

**提供する操作:**
- 通貨の追加、減算処理
- 種別別通貨所有状態管理
- 通貨種別の存在確認
- 所有通貨一覧の取得

**ドメインルール:**
- 同種通貨のみ加減算可能
- 通貨値は非負制約を遵守
- 不足時は減算操作を禁止

### 両替処理サービス
異なる通貨種別間の変換処理を提供します。

**提供する機能:**
- enum値ベースの固定レート両替
- 両替結果の予測可能性
- 通貨稭別間の自動変換

**両替ルール:**
- 通貨タイプのenum値が交換レートを決定
- 固定レートによる安定した交換

### 売買取引サービス
NPCショップでのアイテム売買処理を提供します。

**提供する操作:**
- アイテムの購入処理
- アイテム価格の取得
- 購入可能性の判定

**取引ルール:**
- 十分な通貨を所有している場合のみ購入可能
- 購入時に通貨を自動減算
- アイテムと通貨のアトミック交換

## 技術仕様

### 通貨種別
現在実装されている通貨タイプ：

```csharp
public enum CurrencyType
{
    Sakura = 1, // 桜式通貨（基本通貨）
    Aren = 2,   // アレン式通貨（交換レート2倍）
}
```

### ビジネスルール
- **非負制約**: 通貨値は0以上のみ有効
- **同種通貨制約**: 計算は同じ通貨種別間のみ可能
- **残高制約**: 支払い時は十分な残高が必要
- **両替制約**: enum値に基づく固定レート両替

## ビジネスルール・制約

### 通貨計算制約
- **同種通貨制約**: 加減算は同じCurrencyType間のみ可能
- **非負制約**: 通貨値は0以下にならない
- **減算制約**: 減算時は十分な残高が必要

### 両替ルール
- **固定レート**: 通貨タイプのenum値が交換レートを決定
- **可逆性**: 両替は双方向で実行可能
- **精度保証**: 両替結果の数値精度を保証

### 売買取引制約
- **残高確認**: 購入前の十分な通貨所有確認
- **アトミック取引**: アイテム取得と通貨支払いの原子性
- **価格不変**: 取引中の価格変更は無し

### ドメイン整合性
- **ウォレット状態一貫性**: 所有通貨の正確な管理
- **取引完全性**: 部分的な取引は禁止
- **通貨種別整合性**: 定義されたCurrencyTypeのみ有効

## 関連ドキュメント

- [Inventory.md](../domains/Inventory.md) - 通貨・ウォレット実装詳細
- [Item.md](../domains/Item.md) - NPCショップ実装詳細
- [InventoryManagementSystem.md](./InventoryManagementSystem.md) - アイテム管理システム