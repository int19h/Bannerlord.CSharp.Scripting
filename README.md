# C# scripting for Mount & Blade II: Bannerlord

This mod for [M&B2: Bannerlord](https://www.taleworlds.com/en/Games/Bannerlord) utilizes the .NET Compiler Platform SDK (Roslyn) to allow you to compile and execute C# code snippets and scripts at runtime, with full access to the Bannerlord mod API.

## Prerequisites

The mod is tested with Bannerlord e1.5.6 and e1.5.7 beta. Older versions of the game *may* work, but are considered unsupported.

There are no known compatibility issues with other Bannerlord mods. 

## Installation

Download the most recent version from [Releases](https://github.com/int19h/csx/releases), and unpack it to the Modules folder of your Bannerlord installation. You may need to unblock the DLLs to allow Windows to load them.

If the mod has been loaded correctly, you should see "C# Scripting" when you click Mods in the launcher.

## Quick start

This mod adds several new console commands, all under `csx` namespace. In the game, press `Alt`+`~` to activate the console. To try out the mod, first do:
```
# csx.list
```
If the mod is installed correctly, you should see output that looks something like this:
```
@ C:\Users\...\Documents\Mount and Blade II Bannerlord\Scripts:

always_pregnant
feed_all_settlements
kill_all_bandits
kill_all_nobles
test
world_revolution
```
Now try running the test script:
```
# csx.run test 1 2 3
```
You should see:
```
Arguments passed to this script:
    1
    2
    3
C# scripting test completed successfully.
```

## Console commands

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

Developer console does its own argument parsing, so `csx.eval` has to do some guesswork to reconstitute the original expression. The console replaces any sequence of spaces with a single space, strips out double quotes (`"`) entirely, and treats semicolons (`;`) as console command separators.

To allow for string literals, `csx.eval` substitutes all single quotes with double quotes. Thus, you can write things like:
```
# csx.eval Hero.All.Single(hero => hero.Name.ToString() == 'Liena')
```
On the other hand, this means that character literals are unavailable. These should rarely be needed when using Bannerlord APIs, but if necessary, the workarounds include indexing a string literal: `'A'[0]`; or casting an integer literal: `(char)65`.

To allow for statements, `csx.eval` substitutes every period that are immediately followed by a comma (`.,`) with a semicolon:
```
# csx.eval var x = 42.,
```

If the sequence must appear as is, e.g. in a string literal, split it into two literals and concatenate them: `'.' + ','`.

(Note that these substitutions *only* apply to the arguments of `csx.eval`. In particular, it does *not* apply to `csx.run`, which uses regular C# syntax with no changes.)

To facilitate one-liners, the expression is evaluated in an environment in which every assembly that is loaded into the game process is automatically referenced. Furthermore, several namespaces are implicitly imported, as if with a `using` declaration:

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

This list is configurable via `csx.xml`, located under `...\Documents\Mount and Blade II Bannerlord\Configs` - if it's not present, it will be created the first time the mod is loaded.

The value produced by the evaluated expression, if any, is stringified and printed to the console.

Script state, including any declared variables and functions, is preserved between evaluations. Thus, it's possible to reference some object repeatedly without having to look it up every time:
```
# csx.eval var npc = Hero.All.Single(hero => hero.Name.ToString() == 'Liena').,
# csx.eval npc
Liena
```

### `csx.reset`
Resets the script state. This effectively deletes all variables and functions previously declared using `csx.eval`.

### `csx.run`

Runs a C# script file with the specified name and arguments. The first argument is the name of the script to run, without the `.csx` extension. The remaining arguments are passed as is to the script.

To determine the name of the script file, `.csx` extension is appended to the supplied script name, and the file is looked up in the `...\Documents\Mount and Blade II Bannerlord\Scripts` folder. For example, given:
```
# csx.run test
```
the script that is executed is `...\Documents\Mount and Blade II Bannerlord\Scripts\test.csx`. This sample script is provided with the mod.

The `.csx` file extension reflects the fact that it's a C# *script*, not a regular application. The language is the same, but the [scripting dialect](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/january/essential-net-csharp-scripting) is more relaxed - for example, top-level variable and function declarations are allowed. The dialect is the same as used by the C# REPL in Visual Studio, and by csi.exe.

Like `csx.run`, the script automatically references all assemblies that are already loaded in the game process. However, there are no implicit `using` declarations for namespaces.

Any extra arguments that are passed to `csx.run` beyond the script name, are passed to the script itself via a global variable named `Arguments`, which is a read-only list of strings. Note that all the usual quirks of argument parsing in the developer console are in full force - for example, `csx.run test "foo  bar"` will result in `Arguments` consisting of two entries `"foo"` and `"bar"` (i.e. double quotes are stripped, and any sequence of spaces is treated as a single argument separator).

To produce output in the console, the script should return some value once it's done. That value is stringified and printed, same as with `csx.run`:
```cs
using TaleWorlds.CampaignSystem;
Hero.MainHero.Gold = 1000000000;
return "I'm rich!"; // printed to console
```
Unfortunately, console in Bannerlord does not provide facilities to generate output while the script is running - only after it completed. Thus, if the script fails for any reason before `return`, you won't see the output at all. 

Sometimes, however, you do want to log some information as the script is running - e.g. to determine where exactly it fails. To make this easier, the script can use the global variable named `Log`. It's a custom implementation of `TextWriter` that remembers everything that's written into it, and prints it to the console once the script completes - even if it fails with an exception. If the script successfully returns some value, it's printed after everything that was in `Log`.

Some scripts produce so much output that reading it in the console is inconvenient. For such cases, `Log` also provides a way to write output to file - just call `Log.ToFile()`, and optionally provide the filename to write to. If filename is not specified, the output is written to file with the same name as the script, but extension replaced with `.log`:
```cs
// test.csx
Log.WriteLine("foo");  // This is only written to game console.
Log.ToFile();          // ... now also writing to test.log ...
Log.WriteLine("bar");  // This is written to console and to test.log,
return "baz";          // and so is this
```

### `csx.list`

Lists the scripts available to `csx.run`. This is simply the directory listing of `...\Documents\Mount and Blade II Bannerlord\Scripts\*.csx`, with file extensions removed. It also hides any filename that starts with an underscore - by convention, leading underscore is for reusable scripts that are referenced via `#load` from other scripts.

## Savegame compatibility

The mod itself does not affect saved games in any way. However, C# scripts can define custom classes, and create instances of those classes inside the game. This can break saves, because such classes cannot be deserialized later.
