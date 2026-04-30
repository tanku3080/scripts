# playerPack

ゲーム制作で再利用可能なプレイヤー制御コンポーネント集

---

## 概要

このパッケージは、ジャンルに依存しないプレイヤー制御の基盤と、
用途別のコントローラーを提供する。
コードに記載されている移動速度に関するロジックは基本的にUnity標準の1ユニット＝1m想定で作っている為、物によってはアセットの大きさを変える必要あり

「置くだけで動く」ことと「拡張しやすい構造」を重視して設計されている。

---

## 構成
playerPack/
├─ Core/
│ └─ PlayerCore.cs
│
├─ Controllers/
│ ├─ HumanController.cs
│ ├─ RobotController.cs
│ └─ Vehicle/
│ ├─ VehicleController.cs
│ ├─ TankController.cs
│ ├─ AircraftController.cs
│ └─ ShipController.cs
│
└─ Samples/

---

## 最小構成

以下のみで動作する：

- PlayerCore.cs

---

## 基本的な使い方

1. 空のGameObjectを作成
2. PlayerCore をアタッチ
3. 必要に応じて Controller を追加

例：

- 人間操作 → HumanController
- 乗り物 → VehicleController

---

## 設計思想

### Core
- プレイヤーの共通機能のみを持つ
- 入力・状態・制御の基盤

### Controller
- ジャンル別の挙動を追加
- 必要なものだけ選択して使う

---

## 拡張方法

新しいプレイヤータイプを追加する場合：

1. Controllers 配下に新規スクリプトを作成
2. PlayerCore と連携する処理を実装

---

## 注意点

- PlayerCore は必須
- Controller は複数同時使用を想定していない（設計次第で変更可）
- プロジェクトごとに調整が必要な場合あり

---

## 今後の予定

- 入力システムとの統合
- 状態管理の強化
- サンプルシーン追加