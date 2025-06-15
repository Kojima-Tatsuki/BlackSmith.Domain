# BlackSmith.Domain ドキュメント

このディレクトリには、BlackSmith.Domain ライブラリのゲームロジックに関する詳細なドキュメントが格納されています。

## ドキュメント構成

### [GameLogic.md](./GameLogic.md)
ゲームロジック全体の概要と主要システムの説明。
- プロジェクト概要とアーキテクチャ
- 7つの主要ドメイン（Character、Item、Inventory、Field、Skill、PassiveEffect、Quest）
- ユースケース層とビジネスルール
- 技術的特徴と依存関係

### [Architecture.md](./Architecture.md)
アーキテクチャ設計の詳細説明。
- ドメイン駆動設計（DDD）とクリーンアーキテクチャの実装
- レイヤー構成と責務分離
- 設計パターン（値オブジェクト、ファクトリー、コマンド、リポジトリ）
- 依存関係管理と拡張性設計

### [DomainModels.md](./DomainModels.md)
ドメインモデルの詳細仕様。
- 基盤クラス（BasicID）
- 各ドメインのエンティティ・値オブジェクト
- ファクトリー・コマンドパターンの実装
- ドメイン間の統合モジュール

### [BusinessRules.md](./BusinessRules.md)
ビジネスルールと制約の詳細。
- キャラクターシステム（レベル、経験値、ステータス、戦闘）
- アイテムシステム（装備、強化、クラフト）
- インベントリシステム（容量、移動制限）
- 通貨システム（両替レート、取引制限）
- スキルシステム（習得条件、熟練度）
- フィールド・ステータス効果・クエストシステム

## 推奨読み順

1. **[GameLogic.md](./GameLogic.md)** - 全体像の把握
2. **[Architecture.md](./Architecture.md)** - 設計思想の理解
3. **[DomainModels.md](./DomainModels.md)** - 実装詳細の確認
4. **[BusinessRules.md](./BusinessRules.md)** - ルール詳細の理解

## 対象読者

- **ゲーム開発者**: システム仕様とビジネスルールの理解
- **アーキテクト**: 設計パターンとアーキテクチャの参考
- **QAエンジニア**: テスト設計とバリデーション仕様
- **新規参画者**: プロジェクト理解のためのオンボーディング

## 関連リソース

- [CLAUDE.md](../CLAUDE.md) - プロジェクト全体の設定とビルド手順
- [Assets/Plugins/BlackSmith.Domain/](../Assets/Plugins/BlackSmith.Domain/) - 実装コード
- [Tests/](../Assets/Plugins/BlackSmith.Domain/Tests/) - 単体テスト

## 更新履歴

- 2025-06-15: 初版作成