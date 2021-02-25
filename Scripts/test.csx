// Check that TW assemblies are referenced correctly.
using TaleWorlds;

Log.ToFile();
if (Arguments.Count == 0) {
    Log.WriteLine("No arguments were passed to this script.");
} else {
    Log.WriteLine("Arguments passed to this script:");
    foreach (var arg in Arguments) {
        Log.WriteLine($"\t{arg}");
    }
}

return "C# scripting test completed successfully!";
