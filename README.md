# C# scripting for Mount & Blade II: Bannerlord

This mod for [M&B2: Bannerlord](https://www.taleworlds.com/en/Games/Bannerlord) utilizes the .NET Compiler Platform SDK (Roslyn) to allow you to compile and execute C# code snippets and scripts at runtime, with full access to the Bannerlord mod API.

## Installation

Download the most recent version from [Releases](https://github.com/int19h/csx/releases), and unpack it to the Modules folder of your Bannerlord installation. You may need to unblock the DLLs to allow Windows to load them.

If the mod has been loaded correctly, you should see "C# Scripting" when you click Mods in the launcher.

## Usage

You will need the [Developer Console](https://www.nexusmods.com/mountandblade2bannerlord/mods/4) mod to access the console. This mod adds three new console commands, all under `csx` namespace.

### `csx.eval`

Evaluates its arguments as C# code. For example:
```
# csx.eval 1 + 2
3
```
```
# csx.eval Hero.MainHero.Gold = 1000000000
1000000000
```

Developer console does its own argument parsing, so `csx.eval` has to do some guesswork to reconstitute the original expression. The console replaces any sequence of spaces with a single space, and strips out double quotes (`"`) entirely.

To allow for string literals, `csx.eval` substitutes all single quotes with double quotes. Thus, you can write things like:
```
# csx.eval Hero.All.Single(hero => hero.Name.ToString() == 'Liena')
```
On the other hand, this means that character literals are unavailable. These should rarely be needed when using Bannerlord APIs, but if necessary, the workarounds include indexing a string literal: `'A'[0]`; or casting an integer literal: `(char)65`.

Note that this substitution *only* applies to the arguments of `csx.eval`. In particular, it does *not* apply to `csx.run`, which uses regular C# syntax with no changes.

To facilitate one-liners, the expression is evaluated in an environment in which every assembly that is loaded into the game process is automatically referenced, and the following namespaces are imported as if with a `using` declaration:

- `System`
- `System.Collections.Generic`
- `System.IO`
- `System.Linq`
- `System.Text`
- `TaleWorlds`
- `TaleWorlds.CampaignSystem`
- `TaleWorlds.CampaignSystem.Actions`
- `TaleWorlds.Core`
- `TaleWorlds.Library`
- `TaleWorlds.MountAndBlade`
- `TaleWorlds.ObjectSystem`

The value produced by the expression, if any, is stringified and printed to the console.

### `csx.run`

Runs a C# script file with the specified name and arguments. The first argument is the name of the script to run, without the `.csx` extension. The remaining arguments are passed as is to the script.

To determine the name of the script file, `.csx` extension is appended to the supplied script name, and the file is looked up in the `scripts` folder of the mod. For example, given:
```
# csx.run test
```
the script that is executed is `...\Mount & Blade II Bannerlord\Modules\csx\scripts\test.csx`. This sample script is provided with the mod.

The .csx file extension reflects the fact that it's a C# *script*, not a regular application. The language is the same, but the [scripting dialect](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/january/essential-net-csharp-scripting) is more relaxed - for example, top-level variable and function declarations are allowed. The dialect is the same as used by the C# REPL in Visual Studio, and by csi.exe.

Like `csx.run`, the script automatically references all assemblies that are already loaded in the game process. However, there are no implicit `using` declarations for namespaces.

Any extra arguments that are passed to `csx.run` beyond the script name, are passed to the script itself via a global variable named `Arguments`, which is a read-only list of strings. Note that all the usual quirks of argument parsing in the developer console are in full force - for example, `csx.run test "foo  bar"` will result in `Arguments` consisting of two entries `"foo"` and `"bar"` (i.e. double quotes are stripped, and any sequence of spaces is treated as a single argument separator).

To produce output in the console, the script should return some value once it's done. That value is stringified and printed, same as with `csx.run`:
```cs
using TaleWorlds.CampaignSystem;
Hero.MainHero.Gold = 1000000000;
return "I'm rich!"; // printed to console
```
Unfortunately, console in Bannerlord does not provide facilities to generate output while the script is running - only after it completed. Thus, if the script fails for any reason before `return`, you won't see the output at all. Furthermore, there is a fairly small length limit imposed on output.

Sometimes, however, you do want to log some information as the script is running - e.g. to determine where exactly it fails. Or you want to produce a very long listing that will be trimmed by the console. To make this easier, the script can use the global variable named `Log`. It's a custom implementation of `TextWriter` that writes output into a file with the same name as the script, but extension replaced with `.log`. However, the file is created on first use - if you never write anything to `Log`, the file won't be there. If the file was created, then `return`ed value is also written into it.
```cs
Log.WriteLine("Hello, world!"); // automatically creates the log file
return "All done"; // also written to log, since it's already created
```

### `csx.list`

Lists the scripts available to `csx.run`. This is simply the directory listing of `...\Mount & Blade II Bannerlord\Modules\csx\scripts\*.csx`, with `.csx` file extensions removed. It also hides any filename that starts with an underscore - by convention, leading underscore is for reusable scripts that are referenced via `#load` from other scripts.

## Savegame compatibility

The mod itself does not affect saved games in any way. However, C# scripts can define custom classes, and create instances of those classes inside the game. This can break saves, because such classes cannot be deserialized later.
