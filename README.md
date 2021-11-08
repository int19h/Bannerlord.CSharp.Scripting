# C# scripting for Mount & Blade II: Bannerlord

This mod for [M&B2: Bannerlord](https://www.taleworlds.com/en/Games/Bannerlord) utilizes the .NET Compiler Platform SDK (Roslyn) to allow you to compile and execute C# code snippets and scripts at runtime, with full access to the Bannerlord mod API.

## Prerequisites

The mod is tested with Bannerlord e1.6.1-e1.6.5b. Older versions of the game *may* work, but are considered unsupported.

There are no known compatibility issues with other Bannerlord mods. 

## Installation

**WARNING**: if you had csx 0.1.0 or 0.2.0 installed, please remove it first by deleting the `csx` folder in your Bannerlord `Modules` folder.

Download the most recent version from [Releases](https://github.com/int19h/Bannerlord.CSharp.Scripting/releases), and unpack it to the `Modules` folder of your Bannerlord installation. You may need to unblock the DLLs to allow Windows to load them.

If the mod has been loaded correctly, you should see "C# Scripting" when you click Mods in the launcher.

## Usage

All functionality of the mod is accessible via new commands in the developer console, activated by pressing `Alt`+`~` in the game. 

### Evaluating C# code

The most basic command is `csx.eval`. It should be followed by a C# expression or statement, which is immediately evaluated. If it is an expression, and it produced a value, that value is printed out to the console. For example:

```
# csx.eval 1 + 2
3

# csx.eval Me.Gold = 1000000000
1000000000
```

Bannerlord developer console performs some idiosyncratic argument processing before handing it over to the specific command. In particular:
- Sequences of multiple spaces are replaced with a single space.
- Double quotes are removed.
- Semicolons are treated as console command separators.

Thus, `csx.eval` has to apply some substitutions to its input to allow for full access to C# language features:

- Single quotes (`'`) are treated as double quotes (`"`).
- A period followed by a comma (`.,`) is treated as a semicolon (`;`).

(Note that these substitutions *only* apply to the arguments of `csx.eval`! In particular, they do *not* apply to .csx files - those use regular C# scripting syntax.)

These substitutions are also made inside comments, literals etc. Thus, a string literal typed in the console as `'Foo\'s.,'` is actually `"Foo\"s;"`. If you need a single quote inside a string literal, escape it as `\u0027`. If you need the sequence `.,` inside a string literal, split it into two: `"." + ","`.

Since single quotes are appropriated for string literals, they are no longer available for `char` literals. The workaround is to index a string literal, e.g.: `'A'[0]`; or to use a cast: `(char)65`.

If evaluating a statement rather than an expression, it must be terminated with semicolon, per usual C# rules - which translates to `.,` in the console. So, a variable can be declared thus:

```
# csx.eval var sturgia = Kingdoms['Sturgia'].,
```

Variables persist between evals, and can be referenced later:

```
# csx.eval sturgia.Fiefs.Count()
4
```

Persisted variables can be deleted by using `csx.reset`.

### Inspecting and editing objects

Two helper functions are provided to quickly inspect objects in the console.

`Dump()` prints all the public properties of the object passed to it, along with their types and values. If the argument is enumerable, it is enumerated, and items are printed one by one.

`Edit()` opens a new window with a .NET Windows Forms PropertyGrid control configured to edit the object passed to it. Sometimes, it may be necessary to Alt+Tab from the main game window to see the property grid. To prevent race conditions, the game is paused for as long as the editor window remains opened.

 These functions are also exposed as shortcuts in the console, with `csx.dump …` equivalent to `csx.eval Dump(…)`, and `csx.edit …` equivalent to `csx.eval Edit(…)`.

### Running C# scripts

C# scripts are files with .csx extension that contain code in the [C# scripting dialect](https://docs.microsoft.com/en-us/archive/msdn-magazine/2016/january/essential-net-csharp-scripting). A basic Bannerlord C# script looks like this:
```cs
// Test.csx
void Test(int x = 1, float y = 2) {
    Log.WriteLine($"{x} {y}");
}
```
Note that the name of the function must match the name of the file - `Test.csx` - and return type is always `void`. This script can then be executed via `csx.eval`:
```
# csx.eval Scripts.Test()
1 2

# csx.eval Scripts.Test(3, 4)
3 4

# csx.eval Scripts.Test(y: 5)
1 5
```
`Scripts` is a "magic global" of type [dynamic](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#the-dynamic-type) that automatically forwards method calls to the corresponding script. A console command is provided as a shortcut: `csx.run …` is equivalent to `csx.eval Scripts.…`. 

To be loaded by `Scripts` or `csx.run`, .csx files must be placed in specific folders, which are searched in order:

- User scripts folder, located in your Documents; usually something like
`C:\Users\…\Documents\Mount and Blade II Bannerlord\Scripts`
- Shared scripts folder, located where the mod is installed; usually something like `C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CSharpScripting\bin\Win64_Shipping_Client\Scripts`

The mod comes with a number of stock scripts that are placed in the shared scripts folder; these can be seen by using `csx.list`. It is not recommended to change any files in there, to simplify future mod updates. Instead, place your custom scripts in the user folder. If you want to edit a stock script, copy it to the user folder, and edit the copy - it will take precedence over the shared version.

In addition to the folders above, whenever the game is running in single player campaign mode, there is also the campaign-specific scripts folder. For each campaign, Bannerlord generates a unique campaign ID, which can be accessed via `csx.eval`:
```
# csx.eval CampaignId
f1561944-22af-4153-8113-560466d1c951
```
The corresponding campaign scripts folder is `Campaigns\<id>` under the user scripts folder. For example, for the campaign above, it would be something like `C:\Users\…\Documents\Mount and Blade II Bannerlord\Scripts\Campaigns\f1561944-22af-4153-8113-560466d1c951`

The campaign folder is checked first, before the user folder. This is mainly useful for writing one-off scripts that only make sense within the context of a specific campaign, or to register campaign-specific event handlers.

Scripts can define overloaded methods:
```cs
// Test.csx

void Test(int x) {
    Log.WriteLine($"int {x}");
}

void Test(bool b) {
    Log.WriteLine($"bool {b}");
}
```
These are resolved in the usual manner when making method calls via `dynamic`:
```
# csx.run Test(123)
int 123

# csx.run Test(true)
bool True
```
Scripts can also define functions with different names:
```cs
// Test.csx
void Foo() {
    Log.WriteLine("Foo");
}

void Bar() {
    Log.WriteLine("Bar");
}
```
These can be invoked by specifying the method name when invoking the script:
```
# csx.run Test.Foo()
Foo

# csx.run Test.Bar()
Bar
```
So, `Test()` is simply a short way to write `Test.Test()`.

Every time the script is executed, it runs in a fresh new environment. This means that it doesn't have access to any of the variables or functions that were declared in the console via `csx.eval`, or by any previous invocation of that script or any other script. Thus, global variables are created and initialized anew every time:
```cs
// Test.csx

int x = 0;

void Test() {
    Log.WriteLine(++x);
}
```
```
# csx.run Test()
1

# csx.run Test()
1
```

Scripts have access to the same predefined globals as `csx.eval`. In particular, they can invoke other scripts via `Scripts`:

```cs
// OtherTest.csx
void Foo() => Scripts.Test.Foo()
```

If you need to persist some variable between two different script runs, you can use  `Shared`, which is a [dynamic](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#the-dynamic-type) global that references an instance of [ExpandoObject](https://docs.microsoft.com/en-us/dotnet/api/system.dynamic.expandoobject):

```cs
// Test.csx

void Test() {
    int x;
    try {
        x = Shared.X;
    } catch (Exception) {
        x = 0;
    }
    Shared.X = ++x;
    Log.WriteLine(x);
}
```
```
# csx.run Test()
1

# csx.run Test()
2
```
Note that this object is shared by *all* scripts, and is also accessible via `csx.eval`. 

### Accessing non-public members

Both `csx.eval` expressions and scripts are compiled with visibility checks disabled; thus, internal, protected, and private members can be accessed as well as public ones. Since CLR performs additional visibility checks at runtime, any expression that needs to ignore visibility needs to be wrapped with `IgnoreVisibility()` to disable those runtime checks. For example:

```cs
var rcb = Campaign.Current.CampaignBehaviorManager.GetBehavior<RebellionsCampaignBehavior>();
IgnoreVisibility(() => rcb.StartRebellionEvent(settlement));
```

`StartRebellionEvent` is a private member of RebellionsCampaignBehavior; thus, `IgnoreVisibility` is required here. 

The lambda passed to `IgnoreVisibility` is an expression tree, and all [limitations](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/conversions#anonymous-function-conversions) apply. In particular, the lambda cannot perform assignments. To allow private fields to be changed within the lambda, one has to use the `Set()` method instead:

```cs
IgnoreVisibility(() => Set(out ConversationManager._persuasion, new Persuasion(...)));
```

### Execution environment

Both `csx.eval` and scripts are executed in a pre-populated environment. It includes assembly references for all assemblies loaded in the Bannerlord process, and implicit `using` statements for all available namespaces that begin with `TaleWorlds`, as well as the following:

- `System`
- `System.Collections.Generic`
- `System.Linq`
- `System.Reflection`
- `System.Text`
- `Int19h.Bannerlord.CSharp.Scripting.ScriptGlobals`

The last one is a static class that serves as a mod-specific scripting API - its properties and methods become global variables and functions in the script. Some examples that were used in the code snippets earlier are `Scripts`, `Log`, and `Me`.

### Producing output

Bannerlord console does not provide facilities for commands to produce output as they are running; only when they complete. For more complicated scripts, this can be fairly limited, so the mod provides an incremental logging facility that buffers output, and prints it to console when the script finishes running (even if it throws an exception). This is exposed as global variable `Log` of type `TextWriter` - thus, it can be used much like `Console` in console C# apps.

In addition to console output, `Log` can also write output to files. This is disabled by default, and can be enabled from the console by doing `csx.log_to …`, passing the filename as the argument. To stop logging to file, use `csx.log_to -`. 

### Locating game objects

Scripts have access to the entirety of Bannerlord modding API, and can use it locate various objects in the game. For example, the `Hero` object corresponding to the main character can be obtained via `Hero.MainHero`; and the list of all heroes can be obtained via `Hero.All`.

To make the scripts more concise and facilitate `csx.eval` one-liners, the mod provides several helpers to make this easier. First, there are several shortcuts that simply return the value of the corresponding longer expression:

- `CampaignId` for `Campaign.Current.UniqueGameId`
- `Now` for `CampaignTime.Now`
- `Me` for `Hero.MainHero`
- `MyClan` for `Me.Clan`
- `MyKingdom` for `Me.Clan.Kingdom`
- `MySpouse` for `Me.Spouse`
- `MyParty` for `Me.PartyBelongedTo`
- `MyItems` for `MyParty.ItemRoster`

In addition to those, there are several *lookup tables*. A lookup table wraps some enumerable of game objects. When enumerated, it behaves the same as the original enumerable. However, it also provides indexers that can be used to look up objects by their display name:
```cs
Heroes["Rhagaea"]
```
or by their `StringId`:
```cs
Heroes[Id("main_hero")]
```
or by predicate:
```cs
Heroes[hero => hero.Age >= 18]
```
The first two indexers require there to be exactly one matching object. For example, if there are two heroes named "Asha", then `Heroes["Asha"]` will throw an exception; the same happens if there is no hero with such name. The predicate indexer allows for multiple matching objects, and returns another lookup table corresponding only to those matching objects. 

The following lookup tables are provided:

- `Kingdoms` for `Kingdom.All`
- `Clans` for `Clan.All`
- `Heroes` for `Hero.FindAll(_ => true)`
- `Nobles` for `Heroes[hero => hero.IsNoble]`
- `Wanderers` for `Heroes[hero => hero.IsWanderer]`
- `Settlements` for `ObjectManager.GetObjectTypeList<Settlement>()`
- `Fiefs` for `Town.AllFiefs`
- `Towns` for `Town.AllTowns`
- `Castles` for `Town.AllCastles`
- `Villages` for `Village.All`
- `Parties` for `MobileParty.All`
- `ItemObjects` for `ObjectManager.GetObjectTypeList<ItemObject>()`
- `Perks` for `ObjectManager.GetObjectTypeList<PerkObject>()`
- `CharacterAttributes` for `ObjectManager.GetObjectTypeList<CharacterAttribute>()`
- `Traits` for `ObjectManager.GetObjectTypeList<TraitObject>()`
- `Skills` for `ObjectManager.GetObjectTypeList<SkillObject>()`
- `MyFiefs` for `MyClan.Fiefs`
- `MyTowns` for `MyFiefs[fief => fief.IsTown]`
- `MyCastles` for `MyFiefs[fief => fief.IsCastle]`
- `MyVillages` for `MyClan.Villages`
- `MyChildren` for `Me.Children`
- `MyCompanions` for `MyClan.Companions`
- `MyFamily` for `MyClan.Lords`

### Script argument conversions

In addition to the usual C# implicit conversions, the mod also provides implicit conversions for arguments of the following types:

- `Kingdom`
- `Clan`
- `Hero`
- `Settlement`
- `Town`
- `Village`
- `MobileParty`
- `ItemObject`

When invoking a script with arguments of those types, instead of passing an object, a string or `Id()` can be used; the object is then automatically looked up in the corresponding lookup table. For example:
```cs
// Kill.csx
void Kill(Hero hero) {
    KillCharacterAction.ApplyByMurder(hero);
}
```
can be invoked as:
```
# csx.run Kill('Rhagaea')
# csx.run Kill(Id('main_hero'))
```
which has the same effect as:
```
# csx.run Kill(Heroes['Rhagaea'])
# csx.run Kill(Heroes[Id('main_hero')])
```

These conversions are also applied to arguments of array types. For example:
```cs
// Kill.csx
void Kill(Hero[] heroes) {
    foreach (var hero in heroes) {
        KillCharacterAction.ApplyByMurder(hero);
    }
}
```
can *also* be invoked as:
```
# csx.run Kill('Rhagaea')
# csx.run Kill(Id('main_hero'))
```
which in this case is equivalent to:
```
# csx.run Kill(new[] { Heroes['Rhagaea'] })
# csx.run Kill(new[] { Heroes[Id('main_hero')] })
```

Furthermore, for array arguments, it's possible to pass tuples of values, mixing strings, IDs, and enumerables together - these are all concatenated into a single array of the corresponding type, looking objects up by name or ID as needed. For example (note the extra parentheses around the tuple):
```
# csx.run Kill(('Rhagaea', Id('main_hero'), MyKingdom.Ruler, MyCompanions))
```
is equivalent to:
```
# csx.run Kill(new[] { Heroes['Rhagaea'] }.Append(Heroes[Id('main_hero')]).Append(MyKingdom.Ruler).Concat(MyCompanions).ToArray())
```

Global variable `All` has an unspecified type that is implicitly convertible to arrays of all of the above types, making it possible to write:
```
# csx.run Kill(All)
```

### `CampaignBehavior` and `CampaignEvents`

If there's a script named `CampaignBehavior`, it *must* define the following two methods:
```cs
// CampaignBehavior.csx
void RegisterEvents() => …
void SyncData(IDataStore dataStore) => …
```
The mod will automatically invoke those methods when the corresponding methods of its `CampaignBehavior` object are called by the game. This can be used to register handlers for various campaign events, for example:
```cs
// CampaignBehavior.csx
void RegisterEvents() {
    CampaignEvents.DailyTickEvent.AddNonSerializedListener(null, () => Scripts.CampaignEvents.DailyTick());
}
```
Note that this uses `Scripts` to delegate the actual handling to another script. The advantage of this approach is that `CampaignEvents.csx` will be reloaded every time the event is fired - thus, any changes to it are reflected immediately, even after campaign is loaded. If the body of the event were defined directly in `CampaignBehavior.csx`, editing it while the campaign is running would still use the original handler.

The mod comes with a stock `CampaignBehavior.csx` that already has the above snippet in it. Thus, to run some code on `DailyTick`, it's only necessary to create `CampaignEvents.csx` in the user or campaign script folder (as needed), and define method `void DailyTick()` inside. For other events, `CampaignBehavior.csx` has to be adjusted.

**WARNING**: messing around with `CampaignBehavior.SyncData()` can easily render your saves unusable! It is intentionally undocumented; if you don't already know what it is for, and how to *safely* use `IDataStore`, it's best to leave it alone.

### `SubModule`

If there's a script named `SubModule`, it may define a method called `OnGameStart`:

```cs
void OnGameStart(Game game, IGameStarter gameStarterObject) => ...
```

If present, it will be invoked from the corresponding method of the C# Scripting module. This allows registering custom campaign behaviors, e.g.:

```cs
// SubModule.csx

class ClanTierModel : DefaultClanTierModel {
    public override int GetCompanionLimit(Clan clan) => 1000;

    public override int GetPartyLimitForTier(Clan clan, int clanTierToCheck) => 100;
}

void OnGameStart(Game game, IGameStarter gameStarterObject) {
    gameStarterObject.AddModel(new ClanTierModel());
}
```

## Editing scripts

The mod automatically generates omnisharp.json in the user scripts folder, which enables Intellisense in [Visual Studio Code](https://code.visualstudio.com/) (or any other editor or IDE that uses [OmniSharp](http://www.omnisharp.net/)). To use it, simply do File ⇒ Open Folder in VSCode to open the folder, and then open individual .csx files from the Explorer pane.

Note that OmniSharp is not aware of custom visibility settings for scripts. Thus, accessing non-public members will cause error squiggles while editing, even though the code will execute correctly at runtime.
## Debugging scripts

Scripts are compiled with full debug information. If Visual Studio is [attached](https://docs.microsoft.com/en-us/visualstudio/debugger/attach-to-running-processes-with-the-visual-studio-debugger) to the Bannerlord process, it is possible to set breakpoints, break on exceptions, and use all other debugging facilities. 
