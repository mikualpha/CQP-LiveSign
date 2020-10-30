# CQP-LiveSign
一个基于[CoolQ平台](https://cqp.cc/forum.php)的直播间开播提醒工具（目前支持BILIBILI、斗鱼、Twitch、Langlive）。

因CoolQ已经关闭，可采用[MiraiNative框架](https://github.com/iTXTech/mirai-native)使用本插件。

### 使用组件

[C# Native SDK](https://github.com/Jie2GG/Native.Csharp.Frame/) ([MIT License](https://github.com/Jie2GG/Native.Csharp.Frame/blob/Final/LICENSE))

[SQLite-Net](https://github.com/praeclarum/sqlite-net) ([MIT License](https://archive.codeplex.com/?p=sqlitepcl))

[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) ([MIT License](https://raw.githubusercontent.com/JamesNK/Newtonsoft.Json/master/LICENSE.md))

### 启用方法
请将插件放置在`MiraiNative/plugins`内，并启动Mirai。

随后`MiraiNative/data/cn.mikualpha.livesign/Config.ini`中修改配置：

```
//请仅修改等号后部分，其余部分修改可能会出现问题！
//需要在哪些群中启用，以半角逗号分隔，标0为全部启用
Group=0
//允许哪些群成员修改群订阅设置
Admin=123456789,987654321
//在群组中提醒时是否需要@全体成员，0为禁用，1为启用
AtAll=0
//是否对部分平台启用代理，0为禁用，1为启用
EnableProxy=0
ProxyAddress=127.0.0.1
ProxyPort=1080
//是否开启附加(彩蛋)语句功能(测试中)
EasterEgg=0
```
修改完毕后，重启Mirai。

### 使用指令
```
# 斗鱼
启用订阅: /斗鱼订阅-[房间号](如：/斗鱼订阅-1234)
禁用订阅: /斗鱼取消订阅-[房间号]
查询订阅: /斗鱼订阅查询

# Bilibili
启用订阅: /B站订阅-[房间号](如：/B站订阅-1234)
禁用订阅: /B站取消订阅-[房间号]
查询订阅: /B站订阅查询

# Twitch (需要代理)
启用订阅: /Twitch订阅-[主播ID(详见地址栏)](如：/Twitch订阅-blizzard)
禁用订阅: /Twitch取消订阅-[主播ID]
查询订阅: /Twitch订阅查询

# LangLive (原金刚)
启用订阅: /金刚订阅-[房间号](如：/B站订阅-1234)
禁用订阅: /金刚取消订阅-[房间号]
查询订阅: /金刚订阅查询
```

## Community
Gitter聊天室：[![Gitter](https://badges.gitter.im/MikuAlphaBot/community.svg)](https://gitter.im/MikuAlphaBot/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
