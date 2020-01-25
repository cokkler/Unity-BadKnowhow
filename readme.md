# Unityバッドノウハウ共有会

# 全般 Tips
## メジャーバージョン毎の最終マイナーバージョンがLTSになる
https://helpdesk.unity3d.co.jp/hc/ja/articles/360037095992-%E3%81%A9%E3%81%AE%E3%83%90%E3%83%BC%E3%82%B8%E3%83%A7%E3%83%B3%E3%81%AEUnity%E3%82%92%E3%83%80%E3%82%A6%E3%83%B3%E3%83%AD%E3%83%BC%E3%83%89%E3%81%99%E3%82%8C%E3%81%B0%E8%89%AF%E3%81%84%E3%81%A7%E3%81%99%E3%81%8B-

## AssemblyDefinitionFilesでビルド時間短縮(ソース変更時のビルド対象を絞る)
http://tsubakit1.hateblo.jp/entry/2018/01/18/212834  
通常、ユーザコードはILにコンパイルされ、まとめてAssembly-CSharp.dllとして吐かれる。  
AssemblyDefinitionFilesは、このdllを分割する機能。  

### Usage
分けたいdll単位でフォルダを分け、その直下にAssemblyDefinitionファイルを配置するだけ。  
プロジェクトが大きくなるとコード1行変更しただけでも数秒待たされるため、計画的に分けておかないと辛くなってきます…。

# GameObject(MonoBehaviour)
## nullと"null"
http://nobollel-tech.hatenablog.com/entry/unity-fake-null  
GameObjectはnullではない"null"という状態が存在する。

## DestroyとDestroyImmediate
https://hiyotama.hatenablog.com/entry/2018/03/15/090000  
Destroyは実際に破棄されるまで1フレームのディレイがある。

UnityプロジェクトのGameObjectSceneをベースに説明します。

# UI(UGUI)
## TextMeshProとOverlay Canvas
特定のフォントでまれにOverlay Canvasへの描画がされない時がある。  
Canvasの再描画命令などでは再描画されず、GameViewのサイズを変更すると再描画される。  
実機でもまれに発生するため、プロジェクトではなるべくOverlay Canvasを使わない事になった。

## 入れ子になったContentSizeFitter
http://blog.chatlune.jp/2018/01/22/unity-scrollview-autolayout/  
リソースによってサイズ可変なUIを作りたい時、ContentSizeFitterを使うと便利。  
ただ、ContentSizeFitterを入れ子にした場合、サイズフィットのタイミングが上手くいかない事がある。  
プロジェクトでは親をAwakeでSetActive(false)にしておき、StartでSetActive(true)するか、数フレーム後にSetActive(true)するなど、ベタな対応が必要な場面があった。

# AssetBundle
## iOSのローカルファイルはiCloudバックアップ対象外フラグを付けた方がいい
http://nakamura001.hatenablog.com/entry/20160220/1455975339  
新規保存するファイル、フォルダに対してUnityEngine.iOS.Device.SetNoBackupFlag(PATH)を呼ばないと、iCloudバックアップ対象になり、再インストール時にファイルが復元されてしまうかもしれないので注意。  
中途半端に復元され、再インストール後にゲーム起動しないなど起こりえる。

## ビルドマシンが変わるとAssetBundleのHashが変わる？
http://sassembla.github.io/Public/2018:09:02%2010-17-07/2018:09:02%2010-17-07.html  
Libraryフォルダを作り直すとHash値が変わる事がある…！？というお話。
ここで言うHash値とは、AssetBundleのバージョン管理に使う値。  
データが変わるとHash値も変わる…逆に言うと通常データが変わらなければHash値は変わらない。  

Libraryフォルダは、Assets以下のコードやリソースをUnityが使えるデータに変換しキャッシュしておくためのフォルダ。  
用途から考えると消してもよさそうなんですが…。

プロジェクトの本番環境にて、Androidのみ別マシンでAssetBundleビルドした結果、次バージョンのダウンロード容量がiOS、Androidで差異が発生。  
手元で簡易にLibraryフォルダを消す程度では変わらないが…Hashが変わる確定条件は不明。 

…不具合かな？

## ShaderLabのShaderVariant(プリプロセスっぽいやつ)が吐いた展開後のシェーダの位置が毎回変わる
https://qiita.com/Es_Program/items/79edf9f8fca786b365aa  
上記サイトを例にすると、ビルドに吐かれるキーワードごとのGREEN、RED、BLUEのシェーダ位置が毎回変わる。  
普通に使う分には並びが変わるだけなので問題ないが、AssetBundleにシェーダを持たせた場合Hash、crcが毎回変わるため、アップデートの度にダウンロードが発生する。

## AssetBundleのためのツールセット
### 2018.2以前はAutoya+AssetGraph
http://baba-s.hatenablog.com/entry/2018/03/20/090000

UnityプロジェクトのAutoyaSceneをベースに説明します。

### 2018.3以降はScriptableBuildPipeline、ResourceManager、AdressableAsset
https://www.slideshare.net/UnityTechnologiesJapan/scriptable-build-pipeline

自前で用意してたものが全て要らなくなるかも…？  
誰か使ってみてください。

## AssetBundleダウンロードの進捗をバイト単位で計測する方法
http://light11.hatenadiary.com/entry/2019/07/04/223741#UnityWebRequestGet-x-DownloadHandlerBuffer  
DownloderHandlerAssetBundleを継承してReceivedDataを監視…と言うのが出来ないので、DownloadHandlerScriptを継承してローカルストレージへのキャッシュ周りを自分で書く必要がありそう…。

# その他
意図せぬ動きをし始めたら、とりあえずReImportAllする。

# UniRx
http://neue.cc/  
Reactive Extentions for Unity

UnityプロジェクトのUniRxSceneをベースに説明します。

## 入門
https://www.slideshare.net/shoichiyasui7/unirx-79382950  
https://qiita.com/toRisouP/items/2f1643e344c741dd94f8  
イベントハンドラやシーケンスをLINQっぽい書式で書ける。

## オペレータ逆引き
https://qiita.com/toRisouP/items/3cf1c9be3c37e7609a2f

## Tips(バッドではないノウハウ)
### AddToもしくはXXXUntilDestroyで常駐するストリームの生存期間を管理
https://qiita.com/hadashiA/items/6c6f37b4b739aca3c29a  
UniRxのストリームは終了条件を満たすまで常駐する。

生存期間はモノによるが、Observableクラスから生成したストリームは生成場所のGameObjectがDestroyされても動き続ける(ゾンビになる)場合がある。  
ゾンビ化させないために何らかのオブジェクトと紐付け、それに合わせてストリームを終了させるための設定がAddToないしXXXUntilDestroy。

### Hot ObservableとCold Observableを意識する
https://qiita.com/toRisouP/items/f6088963037bfda658d3  
根元にしたい場所にShare()を入れると吉。

### SelectManyにIObservableメソッド直接置いた際の評価タイミング
Subscribeで一度評価(関数コール)されるため、ラムダ式を挟む、Observable.Create内に実装を書くなど、クッションがないと思わぬ挙動をする。  

# Arkanoid
ArkanoidScene.cs UniRxの練習にどうぞ。
