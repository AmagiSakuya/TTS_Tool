# TTS_Tool

微软自带语音包的TTS工具

## 使用方法

1. 配置Settings.json

```
{
	"voicerName": "Microsoft Kangkang Desktop", //语音包名称 康康需要注册 下载release版本 包含注册表
	"speakVolume": 100,
	"SpeakRate":0
}
```

2. 配置Dictionary.json 

```
{
	"dictionary": [
		{
			"origin": "71014",
			"output": "拐幺洞幺四" //配置成你想要的读音
		}
	]
}
```

3.以窗体方式运行

根据UI提示 选择文本和输出路径 生成语音

4.以命令行方式运行

基于此功能 开发了[Unity_Win_TTS_Tool](https://github.com/AmagiSakuya/Unity_Win_TTS)

会实时读出输入话语 不生成语音 不会产生窗体 读完自动关闭程序
```
TTS_Tool.exe -speak 你的话语
```

## 关于我

[新浪微博](https://weibo.com/233742343/)

[bilibili](https://space.bilibili.com/259362/)

[Twitter](https://twitter.com/Sakuya_fm)
