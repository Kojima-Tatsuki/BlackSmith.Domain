# Field ドメイン

## 概要

Field ドメインは、ゲーム世界の空間構造を4層階層システムで管理します。World（世界）→ Map（マップ）→ Field（フィールド）→ Chunk（チャンク）の階層により、スケーラブルで効率的な世界管理を実現します。

## ドメインモデル

### 階層構造

```
World（世界）
  └── Map（マップ）
      └── Field（フィールド）
          └── Chunk（チャンク）
```

### 各階層の役割

- **World**: 最上位エリア（大陸、次元など）
- **Map**: 世界内の国、山、平原レベル
- **Field**: マップ内の町、区画、ダンジョン（主要エリア単位）
- **Chunk**: フィールド内のレンダリング/処理分割単位

### 識別子

#### 各階層のID
```csharp
public record WorldID : BasicID
{
    protected override string Prefix => "WLD_";
}

public record MapID : BasicID
{
    protected override string Prefix => "MAP_";
}

public record FieldID : BasicID
{
    protected override string Prefix => "FLD_";
}

public record ChunkID : BasicID
{
    protected override string Prefix => "CHK_";
}
```

### エンティティ・値オブジェクト

#### WorldModel
```csharp
public record WorldModel
{
    public WorldID Id { get; }
    public string Name { get; }
    public string Description { get; }
    public ImmutableArray<MapID> Maps { get; }
    
    public WorldModel(WorldID id, string name, string description)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = ValidateName(name);
        Description = description ?? string.Empty;
        Maps = ImmutableArray<MapID>.Empty;
    }
    
    public WorldModel AddMap(MapID mapId)
    {
        if (Maps.Contains(mapId))
            throw new InvalidOperationException("Map already exists in this world");
        
        return this with { Maps = Maps.Add(mapId) };
    }
    
    public WorldModel RemoveMap(MapID mapId)
    {
        return this with { Maps = Maps.Remove(mapId) };
    }
    
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("World name cannot be empty");
        
        if (name.Length > 100)
            throw new ArgumentException("World name cannot exceed 100 characters");
        
        return name;
    }
}
```

#### MapModel
```csharp
public record MapModel
{
    public MapID Id { get; }
    public WorldID WorldId { get; } // 親World参照
    public string Name { get; }
    public string Description { get; }
    public MapType Type { get; }
    public ImmutableArray<FieldID> Fields { get; }
    
    public MapModel(MapID id, WorldID worldId, string name, string description, MapType type)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        WorldId = worldId ?? throw new ArgumentNullException(nameof(worldId));
        Name = ValidateName(name);
        Description = description ?? string.Empty;
        Type = type;
        Fields = ImmutableArray<FieldID>.Empty;
    }
    
    public MapModel AddField(FieldID fieldId)
    {
        if (Fields.Contains(fieldId))
            throw new InvalidOperationException("Field already exists in this map");
        
        return this with { Fields = Fields.Add(fieldId) };
    }
    
    public MapModel RemoveField(FieldID fieldId)
    {
        return this with { Fields = Fields.Remove(fieldId) };
    }
    
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Map name cannot be empty");
        
        if (name.Length > 100)
            throw new ArgumentException("Map name cannot exceed 100 characters");
        
        return name;
    }
}

public enum MapType
{
    Overworld = 1,    // 地上世界
    Underground = 2,  // 地下世界
    Sky = 3,          // 空中世界
    Dungeon = 4,      // ダンジョン
    Town = 5          // 町・都市
}
```

#### FieldModel
```csharp
public record FieldModel
{
    public FieldID Id { get; }
    public MapID MapId { get; } // 親Map参照
    public string Name { get; }
    public string Description { get; }
    public FieldType Type { get; }
    public ImmutableArray<ChunkID> Chunks { get; }
    
    public FieldModel(FieldID id, MapID mapId, string name, string description, FieldType type)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        MapId = mapId ?? throw new ArgumentNullException(nameof(mapId));
        Name = ValidateName(name);
        Description = description ?? string.Empty;
        Type = type;
        Chunks = ImmutableArray<ChunkID>.Empty;
    }
    
    public FieldModel AddChunk(ChunkID chunkId)
    {
        if (Chunks.Contains(chunkId))
            throw new InvalidOperationException("Chunk already exists in this field");
        
        return this with { Chunks = Chunks.Add(chunkId) };
    }
    
    public FieldModel RemoveChunk(ChunkID chunkId)
    {
        return this with { Chunks = Chunks.Remove(chunkId) };
    }
    
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Field name cannot be empty");
        
        if (name.Length > 100)
            throw new ArgumentException("Field name cannot exceed 100 characters");
        
        return name;
    }
}

public enum FieldType
{
    Plains = 1,       // 平原
    Forest = 2,       // 森林
    Mountain = 3,     // 山岳
    Desert = 4,       // 砂漠
    Swamp = 5,        // 沼地
    Town = 6,         // 町
    Dungeon = 7,      // ダンジョン
    Castle = 8,       // 城
    Ruins = 9         // 遺跡
}
```

#### ChunkModel
```csharp
public record ChunkModel
{
    public ChunkID Id { get; }
    public FieldID FieldId { get; } // 親Field参照
    public Position Position { get; }
    public ChunkSize Size { get; }
    public bool IsLoaded { get; }
    
    public ChunkModel(ChunkID id, FieldID fieldId, Position position, ChunkSize size)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        Position = position;
        Size = size;
        IsLoaded = false;
    }
    
    public ChunkModel Load()
    {
        return this with { IsLoaded = true };
    }
    
    public ChunkModel Unload()
    {
        return this with { IsLoaded = false };
    }
}

// 【未実装】位置管理システム
public record Position
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    
    public Position(int x, int y, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public float DistanceTo(Position other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        var dz = Z - other.Z;
        return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}

// 【未実装】チャンクサイズ管理
public record ChunkSize
{
    public int Width { get; }
    public int Height { get; }
    
    public ChunkSize(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Chunk size must be positive");
        
        Width = width;
        Height = height;
    }
    
    public int Area => Width * Height;
}
```

## ビジネスルール

### 階層制約

#### 親子関係の強制
```csharp
public static class HierarchyValidator
{
    public static bool ValidateWorldMapRelation(WorldModel world, MapModel map)
    {
        return world.Maps.Contains(map.Id) && map.WorldId == world.Id;
    }
    
    public static bool ValidateMapFieldRelation(MapModel map, FieldModel field)
    {
        return map.Fields.Contains(field.Id) && field.MapId == map.Id;
    }
    
    public static bool ValidateFieldChunkRelation(FieldModel field, ChunkModel chunk)
    {
        return field.Chunks.Contains(chunk.Id) && chunk.FieldId == field.Id;
    }
}
```

#### 循環参照の防止
- 各階層は必ず上位階層への参照を持つ
- 下位階層から上位階層への逆参照は禁止
- 同一階層間の直接参照は禁止

### 命名規則

#### 名前の制約
- **長さ制限**: 各階層名は100文字以内
- **必須項目**: 名前は必須（空白・null不可）
- **重複制限**: 同一親内での名前重複は許可（IDで識別）

### チャンク管理

#### 読み込み制限
```csharp
public static class ChunkManager
{
    public static bool ShouldLoadChunk(ChunkModel chunk, Position playerPosition, int loadRadius)
    {
        var distance = chunk.Position.DistanceTo(playerPosition);
        return distance <= loadRadius;
    }
    
    public static IEnumerable<ChunkModel> GetChunksToLoad(
        IEnumerable<ChunkModel> allChunks,
        Position playerPosition,
        int loadRadius)
    {
        return allChunks
            .Where(chunk => !chunk.IsLoaded)
            .Where(chunk => ShouldLoadChunk(chunk, playerPosition, loadRadius));
    }
    
    public static IEnumerable<ChunkModel> GetChunksToUnload(
        IEnumerable<ChunkModel> loadedChunks,
        Position playerPosition,
        int unloadRadius)
    {
        return loadedChunks
            .Where(chunk => chunk.IsLoaded)
            .Where(chunk => !ShouldLoadChunk(chunk, playerPosition, unloadRadius));
    }
}
```

## ゲームロジック

### 世界構築

```csharp
// 世界作成の例
public static class WorldBuilder
{
    public static WorldModel CreateBasicWorld(string worldName)
    {
        var worldId = new WorldID();
        return new WorldModel(worldId, worldName, $"The world of {worldName}");
    }
    
    public static MapModel CreateMap(WorldModel world, string mapName, MapType type)
    {
        var mapId = new MapID();
        var map = new MapModel(mapId, world.Id, mapName, $"A {type} area", type);
        
        // 世界にマップを追加
        var updatedWorld = world.AddMap(mapId);
        
        return map;
    }
    
    public static FieldModel CreateField(MapModel map, string fieldName, FieldType type)
    {
        var fieldId = new FieldID();
        var field = new FieldModel(fieldId, map.Id, fieldName, $"A {type} field", type);
        
        // マップにフィールドを追加
        var updatedMap = map.AddField(fieldId);
        
        return field;
    }
    
    public static ChunkModel CreateChunk(FieldModel field, Position position, ChunkSize size)
    {
        var chunkId = new ChunkID();
        var chunk = new ChunkModel(chunkId, field.Id, position, size);
        
        // フィールドにチャンクを追加
        var updatedField = field.AddChunk(chunkId);
        
        return chunk;
    }
}
```

### プレイヤー位置管理

```csharp
public record PlayerLocation
{
    public WorldID WorldId { get; }
    public MapID MapId { get; }
    public FieldID FieldId { get; }
    public ChunkID? ChunkId { get; }
    public Position Position { get; }
    
    public PlayerLocation(
        WorldID worldId,
        MapID mapId,
        FieldID fieldId,
        Position position,
        ChunkID? chunkId = null)
    {
        WorldId = worldId ?? throw new ArgumentNullException(nameof(worldId));
        MapId = mapId ?? throw new ArgumentNullException(nameof(mapId));
        FieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        Position = position;
        ChunkId = chunkId;
    }
    
    public PlayerLocation MoveTo(Position newPosition)
    {
        return this with { Position = newPosition };
    }
    
    public PlayerLocation ChangeField(FieldID newFieldId, Position newPosition)
    {
        return this with 
        { 
            FieldId = newFieldId, 
            Position = newPosition,
            ChunkId = null // チャンクは再計算
        };
    }
}
```

### エリア遷移

```csharp
public static class AreaTransition
{
    public static PlayerLocation TransferToField(
        PlayerLocation currentLocation,
        FieldID targetFieldId,
        Position spawnPosition)
    {
        // フィールド間移動
        return currentLocation.ChangeField(targetFieldId, spawnPosition);
    }
    
    public static PlayerLocation TransferToMap(
        PlayerLocation currentLocation,
        MapID targetMapId,
        FieldID targetFieldId,
        Position spawnPosition)
    {
        // マップ間移動
        return new PlayerLocation(
            currentLocation.WorldId,
            targetMapId,
            targetFieldId,
            spawnPosition
        );
    }
    
    public static PlayerLocation TransferToWorld(
        WorldID targetWorldId,
        MapID targetMapId,
        FieldID targetFieldId,
        Position spawnPosition)
    {
        // 世界間移動
        return new PlayerLocation(
            targetWorldId,
            targetMapId,
            targetFieldId,
            spawnPosition
        );
    }
}
```

## 他ドメインとの連携

### Character ドメインとの連携
- **プレイヤー位置**: 現在位置の追跡
- **移動制限**: エリア固有の移動制限（将来拡張）
- 詳細: [Character.md](./Character.md)

### Quest ドメインとの連携
- **クエスト条件**: 特定エリアでの達成条件
- **目標位置**: クエスト目標の位置指定
- 詳細: [Quest.md](./Quest.md)

### 将来の連携可能性
- **NPCシステム**: エリア固有NPC配置
- **イベントシステム**: 位置トリガーイベント
- **PvPシステム**: エリア制限PvP

## 拡張ポイント

### エリア属性システム
```csharp
// エリア固有の属性・効果
public record AreaAttributes
{
    public float MovementSpeedMultiplier { get; }
    public float ExperienceMultiplier { get; }
    public bool IsPvPEnabled { get; }
    public bool IsSafeZone { get; }
    public WeatherType Weather { get; }
}

public record FieldModel
{
    // 既存プロパティ...
    public AreaAttributes Attributes { get; }
}
```

### ワープポイントシステム
```csharp
public record WarpPoint
{
    public WarpPointID Id { get; }
    public string Name { get; }
    public FieldID TargetFieldId { get; }
    public Position TargetPosition { get; }
    public bool IsUnlocked { get; }
    public Currency Cost { get; }
}
```

### 動的エリア生成
```csharp
// 手続き生成システム
public interface IAreaGenerator
{
    FieldModel GenerateField(MapModel parentMap, FieldType type, int seed);
    IEnumerable<ChunkModel> GenerateChunks(FieldModel field, int chunkSize);
}
```

### エリア制限システム
```csharp
public record AccessRestriction
{
    public PlayerLevel MinLevel { get; }
    public ImmutableArray<QuestID> RequiredQuests { get; }
    public ImmutableArray<ItemID> RequiredItems { get; }
}

public record FieldModel
{
    // 既存プロパティ...
    public AccessRestriction? AccessRestriction { get; }
}
```

Field ドメインは、ゲーム世界の空間的基盤を提供し、他のドメインと連携してリッチなゲーム体験を実現します。\
4層階層により、小規模から大規模まで柔軟にスケールできる設計となっています。