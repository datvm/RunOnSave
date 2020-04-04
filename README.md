# RunOnSave
A simple .NET Core tool to run shell command(s) whenever a file/folder changes.

![Use for Live Reloading for Cordova](https://i.imgur.com/vQcBmIX.png)

# Installation

You will need to have at least [.NET Core 3.1](https://dotnet.microsoft.com/download) installed.

![Install](https://i.imgur.com/SNPCiP5.png)

Install the tool globally (-g or --global) from [Nuget](https://www.nuget.org/packages/LukeVo.RunOnSave):

```dotnet tool install -g LukeVo.RunOnSave```

You can check if it's successfully installed by running:

```runonsave```

# Usage

Simply run from your shell with this syntax:

```runonsave <path> <command1> [<command2> [<command...>]]```

![Run](https://i.imgur.com/229SS8W.png)

For example

```runonsave .\ "echo Hello World" "cordova build browser"```

will print out `Hello World` and then run the build command of `Cordova` whenever the current folder is changed.

There can only be one execution at the same time. If there is file change during the execution, they will be ignored.

## Note about double quote " on Windows

On Windows, the commands are executed by calling `cmd /c <the command>`, therefore you may have trouble with the double quote. For example, if I run from PowerShell:

```runonsave .\ "`"D:\Some Path\script.bat`""```

The script receive the argument as the string `"D:\Some Path\script.bat"` and then when entered into `cmd /c` it becomes:

```cmd /c "D:\Some Path\script.bat"```

Which, in turn cause problem because the command is actually `D:\Some Path\script.bat` (`D:\Some` filename with `Path\script.bat` argument).

To solve it, please use double quote instead:

```runonsave .\ "`"`"D:\Some Path\script.bat`"`""```
