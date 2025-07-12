# レベリングシステム (Leveling System)

## 概要

レベリングシステムは、Character、Skill、PassiveEffect の3つのドメインを統合し、キャラクター成長・熟練度管理に関するドメインサービスを提供します。\
キャラクター成長管理、スキル熟練度管理、成長効果適用、プレイヤーエンティティ管理の各ドメインサービスにより、\
ライブラリ利用者は成長ロジックを構築できます。

## ドメイン問題の定義

### レベリングドメインが扱う問題領域
- **キャラクター成長状態管理**: 経験値蓄積とレベル変換の管理
- **レベル依存制約管理**: レベルに基づくスキル数制限等の制約
- **スキル熟練度管理**: スキル個別の経験値と熟練度計算
- **成長効果適用**: PassiveEffectによる成長促進効果の管理
- **プレイヤーエンティティ管理**: キャラクターのライフサイクル管理

### ドメイン間の関係性
- **Character**: 基本経験値・レベル・ステータス管理の定義
- **Skill**: スキル経験値・熟練度計算ロジックの定義
- **PassiveEffect**: 成長促進効果の定義と適用

## 提供するドメインサービス

### キャラクター成長管理サービス
経験値蓄積とレベル管理のドメインサービスを提供します。

**提供する操作:**
- 経験値の蓄積と累積管理
- 累積経験値からの現在レベル算出
- レベルアップ処理とレベル依存制約更新
- レベルに基づくスキル数制限の適用

**ドメインルール:**
- 経験値は累積加算方式で管理
- レベルは累積経験値に基づき自動決定
- レベルアップ時はスキル数制限を更新
- 経験値の減算は不可

### スキル熟練度管理サービス
個別スキルの経験値と熟練度計算を提供します。

**提供する機能:**
- スキル個別の経験値蓄積
- 熟練度1-1000の段階的成長
- 経験値要求量の指数関数的増加
- 熟練度レベルに応じた次レベル必要経験値計算

**熟練度ルール:**
- 初期必要経験値100、必要回数15から開始
- レベル差倍率1.1による指数関数的増加
- 熟練度1-1000の範囲で管理
- 累積経験値からの熟練度自動算出

### 成長効果適用サービス
PassiveEffectとの連携による成長促進効果を提供します。

**提供する効果管理:**
- ステータス効果の追加・削除
- 経験値倍率効果の適用（基盤準備済み）
- 成長効果の重複管理
- 効果適用状態の管理

**効果適用ルール:**
- 同種効果の重複は上書き
- 効果削除時は元の状態に復元
- 効果適用は成長計算に即座反映

### プレイヤーエンティティ管理サービス
キャラクターエンティティのライフサイクル管理を提供します。

**提供する操作:**
- 新規プレイヤーエンティティの作成
- プレイヤーエンティティの復元・再構築
- プレイヤー情報の取得・更新・削除
- プレイヤー名の変更と妥当性検証

**エンティティ管理ルール:**
- CharacterIDによる一意識別
- エンティティ作成時の初期値設定
- 復元時の整合性保証
- 名前変更時の妥当性チェック

## ビジネスルール・制約

### 経験値・レベル制約
- **非負制約**: 経験値は0以上のみ有効
- **累積制約**: 経験値は累積加算のみ可能（減算不可）
- **レベル算出**: 累積経験値からレベルを自動決定
- **スキル数制限**: レベルに応じたスキル数上限を適用

### スキル熟練度制約
- **熟練度範囲**: 1-1000の範囲で熟練度を管理
- **指数関数成長**: レベル差倍率1.1による要求経験値増加
- **累積経験値管理**: スキル個別の累積経験値を永続管理
- **熟練度算出**: 累積経験値から熟練度を自動算出

### 成長効果制約
- **効果重複**: 同種BattleStatusEffectは上書き適用
- **効果期間**: 基盤は整備済み、具体的期間管理は未実装
- **効果適用**: 成長計算への即座反映

### ドメイン整合性
- **エンティティ同期**: 基本エンティティと戦闘エンティティの整合性
- **レベル連動**: レベル変更時の制約・能力の自動更新
- **熟練度整合性**: スキル熟練度と効果・成功率の連動

## 実装状況

### Domain層実装
- ✅ **Experienceクラス**: 経験値の累積管理
- ✅ **PlayerLevelレコード**: レベル管理とスキル数制限
- ✅ **SkillProficiencyクラス**: スキル熟練度管理
- ✅ **SkillExpCalculatorクラス**: 熟練度経験値計算ロジック
- ✅ **BattleStatusEffectModuleレコード**: ステータス効果管理

### Usecase層実装
- ✅ **AdjustPlayerCommonEntityUsecase**: プレイヤーエンティティ管理
- ✅ **RewritePlayerCommonEntityStatusUsecase**: プレイヤー情報変更
- ✅ **PlayerCommonEntityProvideUsecase**: エンティティ提供サービス

### リポジトリインターフェース
- ✅ **IPlayerCommonEntityRepository**: エンティティ永続化の抽象化

## ドメインサービス API

### キャラクター成長管理サービス
```csharp
// 経験値管理
public class Experience
{
    public int Value { get; }
    
    internal Experience Add(Experience experience);              // 経験値追加
    internal static PlayerLevel CurrentLevel(Experience cumulativeExp); // レベル算出
}

// レベル管理
public record PlayerLevel
{
    public int Value { get; }
    
    internal int GetNumberOfSkillsAvailable();                   // スキル数制限取得
    internal PlayerLevel LevelUp();                             // レベルアップ
}
```

### スキル熟練度管理サービス
```csharp
// スキル熟練度管理
public class SkillProficiency
{
    public int Value { get; }                                    // 熟練度1-1000
    public SkillExperience CumulativeExp { get; }               // 累積経験値
    
    internal SkillProficiency AddExp(SkillExperience exp);      // 経験値追加
}

// スキル経験値計算
internal class SkillExpCalculator
{
    internal SkillExperience NeedToNextLevel(int level);        // 次レベル必要経験値
    internal SkillExperience ReceveExp(int level);              // 受取経験値計算
    internal int CurrentProficiency(SkillExperience cumExp);    // 現在熟練度算出
}
```

### プレイヤーエンティティ管理サービス
```csharp
// プレイヤーエンティティ管理
public class AdjustPlayerCommonEntityUsecase
{
    internal async UniTask<PlayerCommonEntity> CreateCharacter(CharacterName characterName);           // プレイヤー新規作成
    internal async UniTask<PlayerCommonEntity> CreateCharacter(CharacterName characterName, CharacterLevel level); // NPC新規作成
    internal async UniTask<PlayerCommonEntity> ReconstructPlayer(PlayerCommonReconstructCommand command); // 復元
    internal async UniTask<PlayerCommonEntity> GetCharacter(CharacterID id);          // 取得
    internal async UniTask DeletePlayer(CharacterID id);                              // 削除
}

// プレイヤー情報変更
public class RewritePlayerCommonEntityStatusUsecase
{
    internal async UniTask RenamePlayer(CharacterID id, string newName);  // 名前変更
    internal bool IsValidPlayerName(string name);                         // 名前妥当性検証
}
```

### 成長効果管理サービス
```csharp
// 成長効果の管理
public record BattleStatusEffectModule
{
    public IReadOnlyCollection<BattleStatusEffect> StatusEffects { get; }
    
    internal BattleStatusEffectModule AddStatusEffect(BattleStatusEffect statusEffect);      // 効果追加
    internal BattleStatusEffectModule RemoveStatusEffect(BattleStatusEffect statusEffect);   // 効果削除
}
```

### 基本的な利用パターン
1. **キャラクター作成**: `CreateCharacter()` で新規プレイヤーエンティティを作成
2. **経験値管理**: `Experience.Add()` で経験値を蓄積し、`CurrentLevel()` でレベル取得
3. **スキル成長**: `SkillProficiency.AddExp()` でスキル経験値を追加
4. **効果適用**: `BattleStatusEffectModule` で成長促進効果を管理

### ドメインサービスの特徴
- **UniTask対応**: 非同期でのエンティティ管理処理
- **イミュータブル設計**: 経験値・レベル・熟練度の不変性保証
- **自動計算**: 累積値からの現在値自動算出
- **型安全性**: 強型付きによる計算ミス防止

## 関連ドキュメント

- [Character.md](../domains/Character.md) - 経験値・レベル実装詳細
- [Skill.md](../domains/Skill.md) - スキル経験値・熟練度詳細
- [PassiveEffect.md](../domains/PassiveEffect.md) - ステータス効果管理
- [BattleSystem.md](./BattleSystem.md) - 戦闘による成長連携
- [CraftingSystem.md](./CraftingSystem.md) - 制作による成長連携