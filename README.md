<div align="center">
<img src="RumDiceLogo.png" width="300"/>

# RumDice

_⚡ 以方便拓展为目标而开发的 原生C# 跑团机器人核心 ⚡_

![LICENSE](https://img.shields.io/badge/license-MIT-yellow.svg?style=flat)
</div>

## 简介
> **这里是朗姆骰，你也可以叫它怪怪骰！**

当前主流的机器人核心框架不能直接进行C#的插件开发，并且想要开发非常复杂的插件功能并不是很好实现。

出于为了给这种状况提供一个解决方案的目的，我开发了此框架。

在这个项目下，你可以用C#非常快捷的进行回复接口声明和编写，也可以用C#、lua等语言作为脚本语言进行外挂插件开发。
同时，我们制作了大量接口以方便插件作者进行自定义操作，包括自定义任意数据类型并利用我们提供的api进行自动的序列化存取。

## 如何使用 RumDice
无论你是开发者还是普通使用者，我都专门撰写了详细的文档以介绍使用方法。（不过目前还没太写完

在正式版（v1.0）版本推出时，所有类型的用户都可以利用文档快速掌握 RumDice. 

[RumDice的文档中心！](https://github.com/iLoner121/RumDice/wiki)

### 简单入门
#### 骰主
RumDice目前支持QQ（需要gocqhttp）和KOOK（需要你去申请一个Kook机器人token）

你可以在exe同目录的AppSetting.json文件中选择开启哪些平台的服务。（True表示开启，False表示关闭）
##### 搭建RumDice
首先需要下载一个gocqhttp（或许以后项目会附带一个）

[gocqhttp](https://github.com/Mrs4s/go-cqhttp)是一个实现了QQ客户端协议的框架，RumDice目前就是在gocqhttp的基础上运行的（或许以后会推出RumDice自己的）

在gocqhttp内，你需要选择“**正向**websocket代理”模式，设置一个空闲的端口号。然后打开RumDice根目录下的AppSetting.json（如果没有的话运行一下RumDice就有了），把其中的ServerConfig中的地址和端口号填写正确，再次运行，就能让RumDice和gocqhttp连接了。连接成功的话，gocqhttp的控制台窗口会显示：已连接xxxxxx；而RumDice在以服务模式运行的情况下如果连接失败系统就会直接退出。

在上述过程完成之后，RumDice就可以检测到gocqhttp所挂载的那个QQ账号上面发生的事情，并且根据内置的指令做出相应的反应了。
##### 简单自定义
我们的 RumDice 也尽可能为非开发者用户提供了简单快捷的自定义方式（只是没有图形界面罢了）。

目前你可以去 根目录下的\System\Text文件夹内找到一个叫做ReturnWordTable.json的文件，里面就是每一条内置服务的回复语句中。每条语句内部可能会有{0}{1}之类的由花括号括起来的内容，这些东西是回复语句内置的“指令”，它们最终都会被替换成对应的内容。你可以随意修改回复语句，包括增加和修改指令的位置和顺序，甚至加入你自己的指令。日后我将写一个专门的语句指令使用指南。

#### 开发者
RumDice的开发目的就是为了让想要定制自己的功能的开发者可以极其方便的进行拓展开发！
##### 代码结构
在这里简单说明一下整个项目的代码结构
- RumDice 总文件夹
  - Core 核心部分，包含了项目的初始化、收发信息、自动匹配关键词接口等模块（一般不用修改这里）
    - Initializer 初始化程序
    - ClientConnector 和gocqhttp交互的组件
    - CoreData 系统核心功能所需数据及其管理器
    - EventManager 事件处理器，包括关键词匹配等功能
    - MessagePipeline 信息管线，用于均衡消息传入和发送的速率，以及多线程调用事件处理器
    - Startup 启动类，Main函数所在位置，系统组件注册依赖注入的位置
  - Framework 重要的服务类，主要包含文件系统、Log系统、语句处理工具等模块（一般情况下只需调用就好了）
    - Attribute文件夹 所有的Attribute在此声明（用于识别自定义服务）
    - Datatype文件夹 系统主要的数据类型在此声明，包括各类数据包的声明
    - Enum文件夹 系统主要的枚举类型在此声明
    - DataCenter **文件系统**，实现自定义数据类型的本地序列化存、取、在内存中缓存
    - MessageTool **语句处理工具**，包含自动替换回复语句中的参数，自动提取数据包内的类型、文本消息，自动生成回复包等功能
    - RumLogger **日志系统**，在控制台以及本地输出和存储系统日志
    - ServiceManager 对象管理器，管理自定义类型的生成和生命周期，一般情况下用不到（直接new或者访问单例就好了）
  - Module 功能模块部分，也就是说如果你要新增回复接口等功能，就要在这里写代码
    - Example **示例类**，该类和它的接口演示了该系统增加自定义服务的标准流程，内置了几个示例回复接口（你可以修改这个类的内容，但是**一定不要将它和它的接口删除！！！！**）
  - Repository 仓库文件夹，里面全都是各类json。如果你要存储文件，就会存在这个文件夹下面的相对路径里
  - Test 测试程序
  - AppSetting.json 系统设置，如果缺失该文件会在第一次运行时自动创建，其值为默认值
##### 具体写法
具体的一些写法当然还是要看文档：
[开发者文档](https://github.com/iLoner121/RumDice/wiki/%E5%BC%80%E5%8F%91%E8%80%85%E6%96%87%E6%A1%A3)

## 更新日志
### 最新版本
**v0.6.0**
- 订阅事件功能
- 自主发送回信
- 更多控制指令
- 多平台支持
#### 版本总结
处理框架已经基本完成，可以支持内容功能的开发。目前可以通过回复接口的返回值自动发送群消息和私聊消息，也可以主动在任何地方发送群聊消息和私聊消息。

可以订阅大部分onebot协议所提供的事件（如加群、退群事件等）

可订阅报时系统。

可以发送踢人、修改群名等控制指令

提供了多平台支持，并且重构部分代码，增加了RumDice日后增加更多平台的可拓展性。目前基本支持onebot协议系的平台以及discord系

### 日志全文
[更新日志全文](https://github.com/iLoner121/RumDice/wiki/%E6%9B%B4%E6%96%B0%E6%97%A5%E5%BF%97)


## 当前开发计划
目前，该库尚未开发完成，我提前发出以记录我的开发过程并邀请广大对C#有热情的跑团人的参加。
在完成该项目的开发之后，我会制作两个基于此框架的机器人“德洛莉丝”和“菲洛米娜”作为示例项目，以便想要搭建自己的骰子的骰主参考或直接使用。
再往后，我会自己实现和gocqhttp的连接-自己完成对mirai的封装-自己实现协议。届时，将有一个完全C#原生的机器人框架。
## 使用开源库
- [EleCho.GoCqhttpSdk](https://github.com/OrgEleCho/EleCho.GoCqHttpSdk)
- [Kook.Net](https://github.com/gehongyan/Kook.Net)
- [CS-Script](https://github.com/oleg-shilo/cs-script)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [gocqhttp](https://github.com/Mrs4s/go-cqhttp)
