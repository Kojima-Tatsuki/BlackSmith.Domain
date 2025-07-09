# BlackSmith.Domain ドキュメント

このディレクトリには、BlackSmith.Domain ライブラリのゲームロジックに関する詳細なドキュメントが格納されています。

## ドキュメント構成

### 📁 [domains/](./domains/) - ドメイン別詳細仕様
各ドメインの完全仕様（モデル + ビジネスルール + ゲームロジック）
- **[Character.md](./domains/Character.md)** - プレイヤー・レベル・戦闘システム
- **[Item.md](./domains/Item.md)** - アイテム・装備・強化システム  
- **[Inventory.md](./domains/Inventory.md)** - インベントリ・通貨システム
- **[Field.md](./domains/Field.md)** - 世界・フィールドシステム
- **[Skill.md](./domains/Skill.md)** - スキル・熟練度システム
- **[PassiveEffect.md](./domains/PassiveEffect.md)** - ステータス効果システム
- **[Quest.md](./domains/Quest.md)** - クエスト・依頼システム

### 📁 [systems/](./systems/) - ドメインサービスシステム
複数ドメインを統合したドメインサービスの仕様（What）
- **[BattleSystem.md](./systems/BattleSystem.md)** - 戦闘ドメインサービス (Character + Item + PassiveEffect)
- **[CraftingSystem.md](./systems/CraftingSystem.md)** - クラフトドメインサービス (Item + Inventory + Skill)
- **[CurrencySystem.md](./systems/CurrencySystem.md)** - 通貨ドメインサービス (Inventory + Item)
- **[EquipmentSystem.md](./systems/EquipmentSystem.md)** - 装備ドメインサービス (Character + Item + Inventory)
- **[InventoryManagementSystem.md](./systems/InventoryManagementSystem.md)** - インベントリドメインサービス (Inventory + Item)
- **[LevelingSystem.md](./systems/LevelingSystem.md)** - レベリングドメインサービス (Character + Skill + PassiveEffect)
- **[QuestManagementSystem.md](./systems/QuestManagementSystem.md)** - クエストドメインサービス (Quest)

### 📁 [integration/](./integration/) - 技術的ドメイン統合
開発者視点でのドメイン間連携の技術的説明（How）
- **[DomainInteractions.md](./integration/DomainInteractions.md)** - ドメイン間依存関係とデータフロー
- **[SystemIntegration.md](./integration/SystemIntegration.md)** - システム統合パターンとアーキテクチャ

### 📄 基盤ドキュメント
- **[Architecture.md](./Architecture.md)** - 全体アーキテクチャ設計

## 推奨読み順

### 📚 新規参入者向け
1. **[Architecture.md](./Architecture.md)** - 全体アーキテクチャの理解
2. **[domains/Character.md](./domains/Character.md)** - 中核ドメインの理解
3. **[systems/BattleSystem.md](./systems/BattleSystem.md)** - 統合システムの理解
4. 必要に応じて他のドメイン・システムドキュメントを参照

### 🔧 開発者向け
1. 担当ドメインの `domains/*.md` で実装詳細を理解
2. 関連する `systems/*.md` でドメインサービス仕様を確認
3. `integration/*.md` で技術的連携方法を理解

### 🏷️ アーキテクト向け
1. **[Architecture.md](./Architecture.md)** - 設計思想
2. **[integration/](./integration/)** - 統合パターン
3. **[systems/](./systems/)** - 複合システム設計

## 情報検索ガイド

### ドメイン固有の情報を探す場合
- **キャラクター関連**: [domains/Character.md](./domains/Character.md)
- **アイテム・装備関連**: [domains/Item.md](./domains/Item.md) + [systems/EquipmentSystem.md](./systems/EquipmentSystem.md)
- **レベル・成長関連**: [domains/Character.md](./domains/Character.md) + [systems/LevelingSystem.md](./systems/LevelingSystem.md)

### ドメインサービスを探す場合
- **戦闘ドメインサービス**: [systems/BattleSystem.md](./systems/BattleSystem.md)
- **クラフトドメインサービス**: [systems/CraftingSystem.md](./systems/CraftingSystem.md)
- **通貨ドメインサービス**: [systems/CurrencySystem.md](./systems/CurrencySystem.md)
- **インベントリドメインサービス**: [systems/InventoryManagementSystem.md](./systems/InventoryManagementSystem.md)
- **ドメイン間連携**: [integration/DomainInteractions.md](./integration/DomainInteractions.md)

## 対象読者

- **ゲーム開発者**: ドメイン仕様とビジネスルールの理解
- **アーキテクト**: 設計パターンと統合アーキテクチャの参考
- **QAエンジニア**: テスト設計とシステム検証
- **新規参画者**: プロジェクト理解とオンボーディング

## 関連リソース

- [CLAUDE.md](../CLAUDE.md) - プロジェクト全体の設定とビルド手順
- [Assets/Plugins/BlackSmith.Domain/](../Assets/Plugins/BlackSmith.Domain/) - 実装コード
- [Tests/](../Assets/Plugins/BlackSmith.Domain/Tests/) - 単体テスト

## ドキュメント管理方針

### 実装状況の記載ルール
- **実装済み機能を主体とした記載**: ドキュメントは現在実装されている機能を主体として記載
- **未実装機能の明示的分離**: 未実装機能は「未実装～」「拡張ポイント」等の専用セクションで明示的に分離
- **混在の禁止**: 実装済み機能の説明内で未実装機能を言及することは禁止
- **将来計画の適切な表現**: 未実装機能セクションでは「将来実装可能」「拡張予定」等の表現を使用可能

#### 未実装機能記載の理由
BlackSmith.Domainは**ドメインライブラリ**として、利用者が以下を理解する必要があります：
- **機能の範囲**: 現在利用可能な機能の境界
- **設計意図**: 将来的な拡張を考慮した設計の理解
- **実装計画**: ライブラリの発展方向性

### ドメイン間連携の記載方針
- **domains/*.md**: 各ドメインの詳細な実装内容とビジネスルール
- **integration/*.md**: ドメイン間連携の技術的実装方法
- **systems/*.md**: 複数ドメインを統合したドメインサービス仕様

### 情報の重複回避
- **詳細実装**: 各ドメインファイルに記載
- **連携概要**: integration ディレクトリに技術的連携を記載
- **ドメインサービス**: systems ディレクトリでドモインサービス仕様を説明
- **相互参照**: 適切なリンクで情報を繋ぐ

## 更新履歴

- 2025-06-18: ドメイン間連携の整理とドキュメント管理方針の明確化
- 2025-06-15: ドメイン分割構造への移行完了
- 2025-06-15: 初版作成