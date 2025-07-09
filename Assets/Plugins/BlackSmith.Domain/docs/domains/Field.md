# Field ドメイン

## 概要

Field ドメインは、ゲーム世界の空間管理とキャラクター配置を担当するドメインです。

### 主要な責務
- **空間管理**：フィールド（エリア）単位でのゲーム空間の識別と管理
- **キャラクター配置**：各フィールド内に存在するキャラクターの追跡と管理
- **エリア分類**：Village/Abandoned/Dungeonによるフィールドタイプの分類

### 現在の実装状況
- **Field**：FieldIDによる識別、名前管理、キャラクター配置機能を実装
- **Chunk**：フィールド内でのキャラクター存在管理を実装（Guidベース）
- **FieldType**：Village（街・安全圏）、Abandoned（安全圏外）、Dungeon の3種類を定義

### 設計の特徴
- **Character ドメイン連携**：CharacterIDを使用したキャラクター配置管理
- **イミュータブル設計**：Chunkクラスでの状態変更時に新インスタンス生成
- **拡張性**：将来的なWorld/Map階層やPosition座標システムに対応可能な基盤設計

このドメインは Character ドメインとの連携により、ゲーム世界における基本的な空間管理を提供しています。

## ドメインモデル

### 階層構造

#### 実装済み階層
```
Field（フィールド）
  └── Chunk（チャンク）
```

#### 現在の実装状況
- **Field**: ✅ 実装済み - FieldIDによる識別、名前管理、キャラクター配置
- **Chunk**: ✅ 実装済み - Guidベース、キャラクター管理（注意：ChunkIDは未実装）

#### 未実装階層
```
World（世界）     ← 未実装（空のクラスのみ）
  └── Map（マップ）  ← 未実装（空のクラスのみ）
      └── Field（フィールド）  ← 実装済み
          └── Chunk（チャンク）  ← 実装済み
```

### 各階層の役割

#### 実装済み階層の役割
- **Field**: 街やダンジョンを含むマップ単位のエリア管理、キャラクター配置追跡
- **Chunk**: フィールド内でのキャラクター存在管理、データの分割単位

#### 未実装階層の計画（ドキュメント記載のみ）
- **World**: 最上位エリア（大陸、次元など）
- **Map**: 世界内の国、山、平原レベル

### 識別子

#### 実装済み識別子

```csharp
// 実装済み：Field.cs
public record FieldID : BasicID
{
    protected override string Prefix => "Field-";
}
```

**注意**：現在の実装は限定的で、以下の識別子は未実装です：
- `WorldID` - World.csは空のクラス 
- `MapID` - Map.csは空のクラス
- `ChunkID` - Chunk（Chank.cs）はGuid ChunkIdを使用

### エンティティ・値オブジェクト

#### 実装済みエンティティ

##### Field (Field.cs)
```csharp
public class Field
{
    public FieldID ID { get; }
    public string Name { get; }
    public IReadOnlyCollection<CharacterID> CharacterIds => Chunk.CharacterIds;
    internal Chunk Chunk { get; private set; }
    
    // ファクトリーメソッド
    internal static Field ValueOf(FieldID id, string name, IReadOnlyCollection<CharacterID> ids);
    internal static Field NameOf(string name);
    
    // キャラクター管理
    internal void AddCharacter(CharacterID id);
    internal void RemoveCharacter(CharacterID id);
}
```

##### Chunk (Chank.cs)
```csharp
internal class Chunk
{
    public Guid ChunkId { get; }  // 注意：ChunkIDではなくGuidを使用
    public IReadOnlyCollection<CharacterID> CharacterIds { get; }
    
    internal Chunk RemoveCharacter(CharacterID id);
    internal Chunk AddCharacter(CharacterID id);
}
```

##### FieldType (Field.cs)
```csharp
public enum FieldType
{
    Village,   // 街、安全圏内
    Abandoned, // 安全圏外
    Dungeon,
}
```

#### 未実装エンティティ

以下のエンティティはドキュメントに記載されていますが、実装されていません：

##### World (World.cs - 空のクラス)
```csharp
internal class World
{
    // 実装なし
}
```

##### Map (Map.cs - 空のクラス)
```csharp
public class Map
{
    // 実装なし
}
```

**注意**：ドキュメントに記載されている`WorldModel`、`MapModel`、`FieldModel`、`ChunkModel`、`Position`、`ChunkSize`などの詳細なモデルクラスは現在未実装です。

## ビジネスルール

### 実装済みルール

#### Field内のキャラクター管理
```csharp
// Field.cs の実装済み機能
public class Field
{
    // キャラクターの追加・削除
    internal void AddCharacter(CharacterID id);
    internal void RemoveCharacter(CharacterID id);
    
    // 現在のキャラクター一覧取得
    public IReadOnlyCollection<CharacterID> CharacterIds => Chunk.CharacterIds;
}
```

#### FieldType による分類
```csharp
public enum FieldType
{
    Village,   // 街、安全圏内
    Abandoned, // 安全圏外  
    Dungeon,   // ダンジョン
}
```

#### Chunk内のキャラクター管理
```csharp
internal class Chunk
{
    // キャラクターの追加・削除（新しいChunkインスタンスを返す）
    internal Chunk AddCharacter(CharacterID id);
    internal Chunk RemoveCharacter(CharacterID id);
}
```

### 未実装ルール

**注意**：以下のビジネスルールはドキュメントに記載されていますが、現在未実装です：

- **階層制約**：World/Map/Field/Chunkの親子関係検証
- **命名規則**：各階層の名前制約（100文字制限等）
- **チャンク管理**：位置ベースの動的読み込み/アンロード
- **循環参照防止**：階層間の参照制限

## ゲームロジック

### 実装済み機能

#### Field作成と管理
```csharp
// Field.cs の実装済み機能
public class Field
{
    // ファクトリーメソッドでField作成
    internal static Field ValueOf(FieldID id, string name, IReadOnlyCollection<CharacterID> ids);
    internal static Field NameOf(string name);  // 新規Field作成
    
    // キャラクター管理
    internal void AddCharacter(CharacterID id);
    internal void RemoveCharacter(CharacterID id);
}

// 使用例
var field = Field.NameOf("スタート村");
field.AddCharacter(characterId);
```

#### Chunk管理
```csharp
// Chank.cs の実装済み機能  
internal class Chunk
{
    public Guid ChunkId { get; }  // 自動生成
    public IReadOnlyCollection<CharacterID> CharacterIds { get; }
    
    // イミュータブルなキャラクター操作
    internal Chunk AddCharacter(CharacterID id);
    internal Chunk RemoveCharacter(CharacterID id);
}
```

### 未実装機能

**注意**：以下の機能はドキュメントに記載されていますが、現在未実装です：

#### 世界構築システム
- `WorldBuilder`クラス
- `WorldModel`、`MapModel`、`FieldModel`、`ChunkModel`の作成
- 階層的な世界構造の管理

#### プレイヤー位置管理
- `PlayerLocation`レコード
- `Position`システム
- 階層間の位置追跡

#### エリア遷移システム
- `AreaTransition`クラス
- フィールド間/マップ間/世界間移動
- 位置ベースの遷移処理


## まとめ

Field ドメインは現在基本的なフィールドとチャンクによるキャラクター配置管理が実装されており、Character ドメインとの連携によってゲーム世界の基礎的な空間管理を提供しています。

### 現在の実装範囲
- **Field**: FieldIDによる識別、名前管理、キャラクター配置
- **Chunk**: キャラクターの追加・削除によるフィールド内管理
- **FieldType**: Village/Abandoned/Dungeonによる分類

### 今後の拡張可能性
> **⚠️ 注意**: 以下は将来の拡張案であり、現在は未実装です

- **階層構造**: World→Map→Field→Chunkの完全な階層管理
- **位置システム**: 座標ベースの位置管理とエリア遷移
- **エリア属性**: フィールド固有の効果やルール
- **動的生成**: 手続き的なエリア生成システム
- **アクセス制限**: レベルやクエスト条件によるエリア制限

現在の実装は将来的な機能拡張に対応できる柔軟な設計基盤を提供しています。