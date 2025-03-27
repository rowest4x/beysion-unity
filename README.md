# beysion-unity
beysion-unityは、Beysionに使われているエフェクト投影システムです。
[beysion-tracker](https://github.com/rowest4x/beysion-tracker)から送られてきたコマや衝突の情報からエフェクトを発生させます。

## Beysion
Beysionはベイブレードの対戦に合わせて軌跡などのエフェクトを投影するシステムです。タッチテーブルの技術を応用しています。 
詳しくは以下のサイトをご覧ください。<br>
[紹介ページ](https://protopedia.net/prototype/4813)<br>
[プロモーション動画](https://youtu.be/p2AFd2a-vNg?si=FVmgyI9OplT2cY_B)<br>
[解説動画](https://youtu.be/wpbPGy0BBu8?si=w4hq-_JuJQdVVCqS)<br>
<br>
ヒーローズ・リーグ 2023でルーキーヒーロー賞、XUI賞、KOSEN賞をいただきました。
<br>
NT 金沢 2024 に出展しました。
<br>
第29回日本バーチャルリアリティ学会大会で発表しました。
[VR学会講演論文](https://conference.vrsj.org/ac2024/program/doc/2G-10.pdf)

## 使用方法
Unity 2022.3.15f1 で動作確認済みです。
実行する際はbeysion-trackerを先に起動しておく必要があります。
1. 以下のコマンドでクローン
```bash
git clone https://github.com/rowest4x/beysion-unity.git
```
2. Unity Hub を開く
3. 「Add」ボタンをクリックして、クローンしたプロジェクトのフォルダを選択
4. Projectタブで Assets > scene2 > scene2 を開く

## ファイル構成
beysion-trackerとの通信やコマと衝突の管理などはすべて```beysion-unity/Assets/scene2/Scripts/SceneManager2.cs```に記述しています。
具体的には以下の処理を行っています。
- **UDP通信**
    - Pythonから送信された検出結果を受信し、最新フレームのコマの座標や衝突位置を解析

- **エフェクトの再生**
    - 受信データに基づき、各コマオブジェクトをシーン上に生成または更新
    - また、衝突が検出された場合にエフェクト（衝突エフェクト）の生成を行う

- **TCP通信（キャリブレーション要求）**
    - キー入力に応じ、キャリブレーション要求をPython側に送信
    - 動画再生など、キャリブレーション時のビジュアルフィードバックを実装

- **入力操作**
    - キー入力により、エフェクトの切替、カウントダウン再生、キャリブレーション要求を処理
