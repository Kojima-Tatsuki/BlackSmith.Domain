# 戦闘システム (Battle System)

## 概要

戦闘システムは、Character、Item、PassiveEffect の3つのドメインを統合し、戦闘に関するドメインサービスを提供します。\
戦闘エンティティ管理、戦闘パラメータ計算、ダメージ処理、装備効果適用、ステータス効果管理の各ドメインサービスにより、\
ライブラリ利用者は戦闘ロジックを構築できます。

## ドメイン問題の定義

### 戦闘ドメインが扱う問題領域
- **戦闘エンティティの状態管理**: プレイヤーの戦闘時の状態とパラメータ
- **戦闘力の算出**: レベル、装備、ステータス効果を総合した戦闘能力
- **ダメージ計算**: 攻撃者と防御者のパラメータに基づく適正ダメージ
- **装備による戦闘力変動**: 戦闘中の装備変更とその効果適用
- **一時的効果の管理**: バフ・デバフによる戦闘パラメータの変動

### ドメイン間の関係性
- **Character**: 基本戦闘パラメータと戦闘エンティティの提供
- **Item**: 装備品による戦闘力強化と装備制限
- **PassiveEffect**: 一時的なステータス変動効果

## 提供するドメインサービス

### 戦闘エンティティ管理
プレイヤーの戦闘専用エンティティの生成と管理を提供します。

**提供する操作:**
- 基本エンティティから戦闘エンティティへの変換
- レベル依存パラメータの自動初期化
- 戦闘エンティティの永続化・復元

**ドメインルール:**
- 戦闘エンティティは基本エンティティと整合性を保つ
- レベル変更時は戦闘パラメータを再計算する
- 装備モジュールとステータス効果モジュールを含む

### 戦闘パラメータ計算
戦闘に必要な各種パラメータの算出を提供します。

**提供するパラメータ:**
- **MaxHealth**: 最大HP（基本値 + 装備補正）
- **AttackValue**: 攻撃力（基本値 + 装備補正 + ステータス効果）
- **DefenseValue**: 防御力（基本値 + 装備補正 + ステータス効果）

**計算ルール:**
- 基本パラメータはレベルとステータスから算出
- 装備効果は加算方式で適用
- ステータス効果は乗算・加算の組み合わせ

### ダメージ計算サービス
プレイヤー間戦闘における適正ダメージ計算を提供します。

**計算要素:**
- 攻撃者の攻撃力
- 防御者の防御力  
- レベル差による補正
- ランダム要素の適用

**ビジネスルール:**
- レベル差が大きいほど補正が強く働く
- 最小ダメージは1以上を保証
- 防御力がダメージを完全に無効化することはない

### 戦闘中装備管理
戦闘状態での装備変更とその効果適用を提供します。

**提供する操作:**
- 戦闘中の安全な装備交換
- 装備解除による戦闘力変動
- インベントリとの整合性保証

**整合性保証:**
- 装備変更時の戦闘パラメータ即座反映
- インベントリとの同期維持
- 装備制限チェックの実行

### ステータス効果管理
一時的なバフ・デバフ効果の適用と管理を提供します。

**提供する操作:**
- ステータス効果の追加・削除
- 効果の重複管理
- 効果期間の管理（基盤のみ）

**管理ルール:**
- 同種効果の重複は上書き
- 効果適用は戦闘パラメータに即座反映
- 効果削除時は元の値に復元

## ビジネスルール・制約

### 装備制限ルール
- **レベル制限**: 装備のRequireParameterで指定されたレベル以上が必要
- **ステータス制限**: Strength、Agility の最低値要件
- **装備種別制限**: 同じEquipmentTypeは同時に1つまで

### 戦闘計算の制約
- **非負制約**: HP、攻撃力、防御力は0以下にならない
- **レベル差上限**: レベル差による補正は一定範囲内に制限
- **ダメージ下限**: 最低1ダメージは必ず発生

### ドメイン整合性
- **戦闘エンティティ**: 基本エンティティとの同期必須
- **装備効果**: 装備変更時の即座な戦闘力反映
- **ステータス効果**: 効果適用・解除の原子性保証

## 実装状況

### Domain層実装
- ✅ **PlayerBattleEntity**: 戦闘用エンティティの完全実装
- ✅ **BattleParameter**: HP・攻撃力・防御力の計算ロジック
- ✅ **BattleEquipmentModule**: 装備効果のリアルタイム反映
- ✅ **BattleStatusEffectModule**: ステータス効果の管理モジュール

### Usecase層実装
- ✅ **AdjustPlayerBattleEntityUsecase**: 戦闘エンティティのCRUD操作
- ✅ **CharacterDamageUsecase**: プレイヤー間ダメージ計算
- ✅ **BattleEquipmentUsecase**: 戦闘中装備変更サービス
- ✅ **PlayerBattleEntityProvideUsecase**: エンティティ提供サービス

### リポジトリインターフェース
- ✅ **IPlayerBattleEntityRepository**: 戦闘エンティティの永続化


## ドメインサービス API

### 戦闘エンティティ管理サービス
```csharp
// 戦闘エンティティの生成・管理
public class AdjustPlayerBattleEntityUsecase
{
    internal async UniTask<PlayerBattleEntity> CreateCharacter(PlayerCommonEntity entity);
    internal async UniTask<PlayerBattleEntity> ReconstructPlayer(PlayerBattleReconstructCommand command);
    internal async UniTask<PlayerBattleEntity> GetPlayer(CharacterID id);
    internal async UniTask DeletePlayer(CharacterID id);
}
```

### ダメージ計算サービス
```csharp
// プレイヤー間戦闘のダメージ計算
public class CharacterDamageUsecase
{
    internal async UniTask<int> CalculateDamage(PlayerBattleEntity attacker, PlayerBattleEntity defender);
}
```

### 装備管理サービス
```csharp
// 戦闘中の装備変更
public class BattleEquipmentUsecase
{
    internal async UniTask<PlayerBattleEntity> EquipItem(PlayerBattleEntity entity, EquippableItem item);
    internal async UniTask<PlayerBattleEntity> UnequipItem(PlayerBattleEntity entity, EquipmentType type);
}
```

### 基本的な利用パターン
1. **戦闘エンティティ生成**: `CreateCharacter()` で基本エンティティから戦闘エンティティを生成
2. **装備状態同期**: `BattleEquipmentUsecase` で装備状態を戦闘パラメータに反映
3. **ダメージ計算**: `CalculateDamage()` でプレイヤー間戦闘のダメージを算出
4. **結果保存**: リポジトリ経由で戦闘結果を永続化

### ドメインサービスの特徴
- **UniTask対応**: 非同期処理による高パフォーマンス
- **イミュータブル設計**: エンティティの状態不変性保証
- **型安全性**: 強型付きドメインモデルによる安全性
- **テスタビリティ**: ユニットテスト可能な設計

## 関連ドキュメント

- [Character.md](../domains/Character.md) - 戦闘エンティティの基盤
- [Item.md](../domains/Item.md) - 装備システムの詳細
- [PassiveEffect.md](../domains/PassiveEffect.md) - ステータス効果の仕様
- [EquipmentSystem.md](./EquipmentSystem.md) - 装備管理システム
- [LevelingSystem.md](./LevelingSystem.md) - レベリングとの連携