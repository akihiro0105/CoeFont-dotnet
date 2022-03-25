# CoeFont-dotnet
- 音声合成サービスCoeFontをC#環境で動作させるサンプルコード
    - [https://coefont.cloud/coefonts](https://coefont.cloud/coefonts)

## CoeFont-dotnet-VisualStudio
- .NetCoreと.NetFramework環境で音声合成を確認できるプロジェクト
- 実行ファイル階層にwavが保存される

## CoeFont-dotnet-Unity
- Unity環境で音声合成を確認できるプロジェクト
- [README.md](./CoeFont-dotnet-Unity/Assets/com.akihiro.coefont-dotnet/Documentation/README.md)

### UPMインポート手順
- Window > Package Managerで左上の＋ボタンを選択。
- Add Package from git URL ...で下のリンクを入力し、Addボタンでインポートする。

#### https
```git+https://github.com/akihiro0105/CoeFont-dotnet.git?path=CoeFont-dotnet-Unity/Assets/com.akihiro.coefont-dotnet```

or

#### ssh
```git+ssh://git@github.com/akihiro0105/CoeFont-dotnet.git?path=CoeFont-dotnet-Unity/Assets/com.akihiro.coefont-dotnet```
