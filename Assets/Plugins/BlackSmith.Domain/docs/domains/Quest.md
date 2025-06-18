# Quest ドメイン

## 概要

Quest ドメインは、ゲーム内のクエスト・依頼システムを管理するドメインです。

### 主要な責務
- **クエスト基本情報管理**：クエスト名・説明・依頼人の管理
- **依頼人情報管理**：CharacterID による依頼人追跡

### 現在の実装状況
- **QuestModel**：基本的なクエストデータモデルを実装（最小限）

**注意**：Quest ドメインは現在プレースホルダー状態で、基本的なデータモデル以外の機能は未実装です。

### 設計の特徴
- **Character ドメイン連携**：CharacterID による依頼人管理
- **ミニマル設計**：必要最小限の基本情報のみ実装

このドメインは現在 Character ドメインとの基本的な連携のみ実装されており、将来的な拡張のための基盤として位置づけられています。

## ドメインモデル

### 識別子

#### 未実装識別子

**注意**：現在の実装では以下の識別子が未実装です：
- `QuestID` - クエスト識別子（BasicID 継承）
- `ObjectiveID` - 目標識別子

### エンティティ・値オブジェクト

#### 実装済みエンティティ

##### QuestModel (QuestModel.cs)
```csharp
// 実装済み：基本的なクエストモデル
internal class QuestModel
{
    public string QuestName { get; }
    public string Discription { get; } // 注意：Descriptionのtypo
    private CharacterID ClientId { get; } // 依頼人ID
    
    internal QuestModel(string questName, string discription, CharacterID clientId);
}
```

**実装の特徴**：
- **internal クラス**: 外部からの直接アクセスを制限
- **プロパティベース**: 基本的な読み取り専用プロパティ
- **CharacterID 連携**: Character ドメインとの依存関係
- **typo**: "Discription" は "Description" のスペルミス

#### 未実装エンティティ

**注意**：以下のエンティティはドキュメントに記載されていますが、現在未実装です：

##### 値オブジェクト
- `QuestTitle` - クエストタイトル値オブジェクト
- `QuestDescription` - クエスト説明値オブジェクト
- `ObjectiveDescription` - 目標説明値オブジェクト

##### エンティティ
- `QuestObjective` - クエスト目標管理
- `QuestReward` - クエスト報酬システム

##### 列挙型
- `QuestStatus` - クエスト状態（Available/InProgress/Completed/Failed/Expired）
- `QuestType` - クエスト種別（Main/Side/Daily/Weekly/Event/Guild）
- `QuestDifficulty` - クエスト難易度（Easy/Normal/Hard/Expert/Master）
- `ObjectiveType` - 目標種別（KillEnemies/CollectItems/CraftItems等）
- `RewardType` - 報酬種別（Item/Currency/Experience）

## ビジネスルール

### 実装済みルール

現在、ビジネスルール層の実装は存在しません。QuestModel は基本的なデータコンテナとしてのみ機能しています。

#### 基本的なデータ制約
```csharp
// QuestModel.cs の実装済み制約（最小限）
internal QuestModel(string questName, string discription, CharacterID clientId)
{
    QuestName = questName;
    Discription = discription;
    ClientId = clientId;
    // 現在は引数の null チェック等も未実装
}
```

### 未実装ルール

**注意**：以下のビジネスルールは将来実装予定ですが、現在未実装です：

#### クエスト状態管理
- **状態遷移制御**：Available → InProgress → Completed/Failed
- **期限切れ処理**：時間ベースの期限切れ判定
- **放棄処理**：進行中クエストの放棄機能

#### 目標管理システム
- **進行状況追跡**：各目標の達成進度管理
- **完了判定**：全目標完了による自動クエスト完了
- **進行条件**：目標更新の可能性チェック

#### 報酬システム
- **報酬受取制限**：完了済みクエストのみ報酬受取可能
- **インベントリ制限**：アイテム報酬のインベントリ容量チェック
- **重複受取防止**：同一クエストの報酬重複受取防止

## ゲームロジック

### 実装済み機能

#### 基本的なクエスト作成
```csharp
// QuestModel の基本使用例
var clientId = new CharacterID(); // Character ドメインから
var quest = new QuestModel(
    "アイアンソードを作ろう",
    "アイアンソードを1本作成してください",
    clientId
);

// アクセス
string name = quest.QuestName;
string description = quest.Discription; // typo注意
// ClientId は private のためアクセス不可
```

### 未実装機能

**注意**：以下の機能は将来実装予定ですが、現在未実装です：

#### クエスト管理システム
- **クエスト受注**：プレイヤーによるクエスト受注処理
- **クエスト放棄**：進行中クエストの放棄処理
- **クエスト完了**：目標達成による自動完了判定
- **クエスト失敗**：期限切れ・条件不達による失敗処理

#### 目標追跡システム
- **進行状況更新**：ゲームイベントによる目標進捗更新
- **達成通知**：目標達成時の通知システム
- **複数目標管理**：並行する複数目標の同時追跡

#### 報酬処理システム
- **報酬算出**：クエスト完了時の報酬計算
- **報酬付与**：プレイヤーへの報酬アイテム・経験値付与
- **インベントリ統合**：Inventory ドメインとの報酬アイテム連携

#### クエスト生成システム
- **ファクトリーパターン**：標準的なクエスト作成機能
- **動的生成**：プレイヤーレベルに応じた自動クエスト生成
- **テンプレート**：再利用可能なクエストテンプレート


## 拡張ポイント

### 実装可能な拡張

#### 基本的なクエスト状態管理
```csharp
// 将来実装可能：クエスト状態追加
public enum QuestStatus
{
    Available,  // 受注可能
    InProgress, // 進行中
    Completed,  // 完了
    Failed,     // 失敗
    Expired     // 期限切れ
}

internal class EnhancedQuestModel : QuestModel
{
    public QuestStatus Status { get; }
    public DateTime CreatedAt { get; }
    public DateTime? AcceptedAt { get; }
    public DateTime? CompletedAt { get; }
    
    // 状態遷移メソッド
    public EnhancedQuestModel Accept();
    public EnhancedQuestModel Complete();
    public EnhancedQuestModel Fail();
}
```

#### QuestID システム
```csharp
// 将来実装可能：クエスト識別子
public record QuestID : BasicID
{
    protected override string Prefix => "QST-";
}

internal class IdentifiableQuestModel
{
    public QuestID Id { get; }
    public string QuestName { get; }
    public string Description { get; } // typo修正
    public CharacterID ClientId { get; } // public化
}
```

#### 目標システム
```csharp
// 将来実装可能：クエスト目標
public record QuestObjective
{
    public string Description { get; }
    public int CurrentProgress { get; }
    public int RequiredProgress { get; }
    public bool IsCompleted => CurrentProgress >= RequiredProgress;
    
    public QuestObjective UpdateProgress(int progress);
}

internal class ObjectiveQuestModel
{
    public IReadOnlyCollection<QuestObjective> Objectives { get; }
    public bool AllObjectivesCompleted => Objectives.All(o => o.IsCompleted);
}
```

#### 報酬システム
```csharp
// 将来実装可能：クエスト報酬
public record QuestReward
{
    public RewardType Type { get; }
    public object RewardData { get; } // Item, Currency, Experience
    public int Quantity { get; }
}

public enum RewardType
{
    Item,       // アイテム報酬
    Currency,   // 通貨報酬
    Experience  // 経験値報酬
}
```

### 設計基盤の特徴

現在の実装は最小限ですが、将来の拡張に対応できる基盤を提供：

- **Domain 分離**: 独立したQuest名前空間
- **Character 連携**: 既存ドメインとの適切な依存関係
- **内部実装**: 適切なカプセル化による外部API制御
- **拡張準備**: 将来的な機能追加に対応可能な基本構造

Quest ドメインは現在プレースホルダー状態ですが、Character ドメインとの基本連携により、将来的な本格的なクエストシステム実装のための基盤を提供しています。