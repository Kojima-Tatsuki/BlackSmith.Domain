# BlackSmith.Domain

Unity向けのドメイン駆動設計（DDD）ライブラリ。ゲーム開発におけるドメインロジックとUnity固有の実装を分離したクリーンアーキテクチャを提供します。

## 概要

BlackSmith.Domainは、ゲームプロジェクト用のドメインロジックを管理するUnityライブラリです。以下の特徴を持ちます：

- **ドメイン駆動設計（DDD）**: ビジネスロジックとインフラストラクチャの分離
- **クリーンアーキテクチャ**: 依存関係の逆転によるテスタブルなコード
- **非同期処理**: UniTaskによる効率的な非同期処理
- **リアクティブ**: R3によるリアクティブプログラミング
- **イミュータブル**: System.Collections.Immutableによる安全なデータ管理

## 主要ドメイン

- **Character**: プレイヤーエンティティ、戦闘システム、装備管理
- **Items**: 装備品、クラフト可能アイテム、武器、NPCショップ
- **Inventory**: 装備インベントリ、取引システム、ウォレットシステム
- **Field**: 4層構造の世界システム（World → Map → Field → Chunk）
- **Skills**: 戦闘スキル、生産スキル、パッシブ効果
- **Quest**: クエスト管理システム

## インストール

### Unity Package Manager (UPM)

Package ManagerのWindow > Package Managerから「Add package from git URL」を選択し、以下のURLを入力してください：

#### 最新安定版
```
https://github.com/Kojima-Tatsuki/BlackSmith.Domain.git?path=Assets/Plugins/BlackSmith.Domain
```

### 必要なUnityバージョン
- Unity 2023.2以上

### 依存関係
以下のパッケージが自動的にインストールされます：
- UniTask
- Newtonsoft.Json
- R3
- System.Collections.Immutable

## 使用方法

詳細なドキュメントは `Assets/Plugins/BlackSmith.Domain/docs/` フォルダ内のREADME.mdを参照してください。

## ライセンス

このプロジェクトのライセンスについては [LICENSE.txt](LICENSE.txt) を参照してください。

## 作者

Tatsuki Kojima

---

**注意**: .metaファイルはUnity向けのものです。Unity以外の環境で使用する場合は削除してください。
