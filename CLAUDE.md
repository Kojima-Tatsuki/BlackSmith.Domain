# CLAUDE.md

このファイルは、Claude Code (claude.ai/code) がこのリポジトリで作業する際のガイダンスを提供します。

## プロジェクト概要

BlackSmith.Domain は、ゲームプロジェクト用の Unity ベースのドメインライブラリです。\
ドメイン駆動設計（DDD）の原則を実装し、ドメインロジックと Unity 固有の実装を分離したクリーンアーキテクチャを採用しています。

## アーキテクチャ

### コア構造
- **ドメイン層**: `Assets/Plugins/BlackSmith.Domain/Domain/` にあるコアビジネスロジック
- **ユースケース層**: `Assets/Plugins/BlackSmith.Domain/Usecase/` にあるアプリケーションロジック
- **テスト**: `Assets/Plugins/BlackSmith.Domain/Tests/` にある単体テスト

### 主要ドメイン領域
- **Character**: プレイヤーエンティティ、戦闘システム、装備管理
- **Items**: 装備品、クラフト可能アイテム、武器、NPC ショップ
- **Inventory**: 装備インベントリ、取引システム、ウォレットシステム
- **Field**: 4層構造の世界システム（World → Map → Field → Chunk）
- **Skills**: 戦闘スキル、生産スキル、パッシブ効果
- **Quest**: クエスト管理システム

### 依存関係
- **UniTask**: Unity 用の async/await サポート
- **R3**: リアクティブエクステンション
- **Newtonsoft.Json**: JSON シリアライゼーション
- **System.Collections.Immutable**: イミュータブルコレクション

## ビルドとテストコマンド

### Unity エディタ
- Unity エディタ（Unity 2023.2+）でプロジェクトを開く
- テストは Unity Test Runner ウィンドウから実行

### Visual Studio ソリューション
```bash
# ソリューションをビルド
dotnet build BlackSmith.Domain.sln

# 特定のプロジェクトをビルド
dotnet build BlackSmith.Domain.csproj
dotnet build BlackSmith.Domain.Test.csproj
```

### Unity Package Manager
プロジェクトは UPM パッケージとして配布されています：
- メイン: `https://github.com/Kojima-Tatsuki/BlackSmith.Domain.git?path=Assets/Plugins/BlackSmith.Domain`
- 開発版: `https://github.com/Kojima-Tatsuki/BlackSmith.Domain.git?path=Assets/Plugins/BlackSmith.Domain#develop`

## フィールドシステムアーキテクチャ

世界は4つの階層構造で構成されています：
1. **World（世界）**: 最上位エリア（1桁数を想定）
2. **Map（マップ）**: 世界内の国、山、平原
3. **Field（フィールド）**: マップ内の町、区画、ダンジョン（主要エリア単位）
4. **Chunk（チャンク）**: フィールド内のレンダリング/処理分割

## コーディング規約

- C# 9.0 言語機能を使用
- 参照型の null 許容性を有効化（`#nullable enable`）
- ドメインエンティティは値オブジェクトに record 型を使用（例: `BasicID`）
- エンティティ作成にファクトリーパターンを使用
- エンティティ再構築にコマンドパターンを使用
- データアクセスにリポジトリパターンを使用（ユースケース層のインターフェース）

## ドキュメント管理

### 機能追加・修正時の注意点
- 新機能を追加または既存機能を修正した場合は、必ず関連する `Assets/Plugins/BlackSmith.Domain/docs/` 内のドキュメントも更新してください
- 影響するドキュメントファイル例：
  - `docs/README.md`: 全体概要やインストール手順
  - `docs/domains/`: 各ドメインの詳細説明
  - `docs/systems/`: システム設計の詳細
  - `docs/integration/`: システム間の連携方法
- ドキュメントの更新は機能実装と同じコミットに含めるか、直後に行ってください
- APIの変更がある場合は、使用例やサンプルコードも併せて更新してください
