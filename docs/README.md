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

### 📁 [systems/](./systems/) - 複合システム仕様
複数ドメインを統合した統合システム
- **[BattleSystem.md](./systems/BattleSystem.md)** - 戦闘システム (Character + Item + PassiveEffect)
- **[CraftingSystem.md](./systems/CraftingSystem.md)** - クラフトシステム (Item + Inventory + Skill)
- **[EquipmentSystem.md](./systems/EquipmentSystem.md)** - 装備システム (Character + Item + Inventory)
- **[LevelingSystem.md](./systems/LevelingSystem.md)** - レベリングシステム (Character + Skill + PassiveEffect)

### 📁 [integration/](./integration/) - ドメイン統合
ドメイン間の相互作用と統合パターン
- **[DomainInteractions.md](./integration/DomainInteractions.md)** - ドメイン間依存関係とデータフロー
- **[SystemIntegration.md](./integration/SystemIntegration.md)** - システム統合パターンとアーキテクチャ

### 📄 基盤ドキュメント
- **[Architecture.md](./Architecture.md)** - 全体アーキテクチャ設計
- **[GameLogic.md](./GameLogic.md)** - ゲームロジック概要（レガシー）
- **[DomainModels.md](./DomainModels.md)** - ドメインモデル概要（レガシー）
- **[BusinessRules.md](./BusinessRules.md)** - ビジネスルール概要（レガシー）

## 推奨読み順

### 📚 新規参入者向け
1. **[Architecture.md](./Architecture.md)** - 全体アーキテクチャの理解
2. **[domains/Character.md](./domains/Character.md)** - 中核ドメインの理解
3. **[systems/BattleSystem.md](./systems/BattleSystem.md)** - 統合システムの理解
4. 必要に応じて他のドメイン・システムドキュメントを参照

### 🔧 開発者向け
1. 担当ドメインの `domains/*.md` を熟読
2. 関連する `systems/*.md` で統合パターンを確認
3. `integration/*.md` でドメイン間連携を理解

### 🏗️ アーキテクト向け
1. **[Architecture.md](./Architecture.md)** - 設計思想
2. **[integration/](./integration/)** - 統合パターン
3. **[systems/](./systems/)** - 複合システム設計

## 情報検索ガイド

### ドメイン固有の情報を探す場合
- **キャラクター関連**: [domains/Character.md](./domains/Character.md)
- **アイテム・装備関連**: [domains/Item.md](./domains/Item.md) + [systems/EquipmentSystem.md](./systems/EquipmentSystem.md)
- **レベル・成長関連**: [domains/Character.md](./domains/Character.md) + [systems/LevelingSystem.md](./systems/LevelingSystem.md)

### システム横断的な情報を探す場合
- **戦闘処理**: [systems/BattleSystem.md](./systems/BattleSystem.md)
- **アイテム作成**: [systems/CraftingSystem.md](./systems/CraftingSystem.md)
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

## 更新履歴

- 2025-06-15: ドメイン分割構造への移行完了
- 2025-06-15: 初版作成