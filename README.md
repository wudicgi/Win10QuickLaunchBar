# Quick Launch Bar for Windows 10

[![GitHub release](https://img.shields.io/github/release/wudicgi/Win10QuickLaunchBar.svg)](https://github.com/wudicgi/Win10QuickLaunchBar/releases/latest)

## 1. 简介

在 Windows 10 上，当任务栏选择使用小按钮时，快速启动栏中的图标会有对齐和缩放问题:

![Windows 10 Built-in Quick Launch Bar](docs/quick_launch_bar_win10_built_in.png)

在高分辨率和高缩放系数 (如 4k 分辨率 + 250% 缩放率) 的情况下问题更为明显。

本程序为基于 WPF 和 DeskBand 编写的快速启动栏，可正确对图标进行对齐和缩放。显示效果如下:

![This Quick Launch Bar](docs/quick_launch_bar_this_program.png)

## 2. 使用方法

1. 下载最新的 release 版本程序，解压到任意位置。
2. 打开 settings.ini 修改配置参数，并保存。
3. 右键点击 Install.bat 选择以管理员身份运行。
4. 在任务栏上空白处右键点击，在“工具栏”子菜单中选择“Quick Launch Bar”。
5. 在任务栏上空白处右键点击，取消选中“锁定任务栏”。
6. 拖动调整工具栏的位置。如果中途发现工具栏不见了，可以关闭“Quick Launch Bar”工具栏后再打开。
7. 如果显示效果不满意，在修改 settings.ini 后，可以在“Quick Launch Bar”工具栏内任意图标或空白位置点击鼠标右键，选择“重新加载配置”。

## 3. 使用提示

1. backgroundColor 可以通过对屏幕截图后，拾取任务栏的颜色获取。可使用 [https://imagecolorpicker.com/](https://imagecolorpicker.com/) 在线工具，将屏幕截图通过 Ctrl-V 粘贴到页面上之后即可拾取颜色。

2. 当图标大小与开始菜单不一致，导致显示不协调时，可适当增大或减小 buttonPadding 的值来修改图标大小。

3. 要指定图标的显示顺序，可在快捷方式的文件名中加 01_, 02_ 等前缀。提示文本 (ToolTip) 中将不会包含这些前缀。
