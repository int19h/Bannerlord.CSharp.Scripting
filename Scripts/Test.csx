// void Test(object o) {
//     Log.WriteLine(o);
// }

float Test(int x, float y = 1) {
    Log.WriteLine($"int {x}; float {y};");
    return x * y;
}

void Test(bool z) {
    Log.WriteLine($"bool {z}");
}

void Test(string s) {
    Log.WriteLine($"string {s}");
}

// void Test(Kingdom kingdom) {
//     Log.WriteLine($"kingdom {kingdom}");
// }

// void Test(Kingdom[] kingdoms) {
//     foreach (var kingdom in kingdoms) {
//         Log.WriteLine(kingdom);
//     }
// }

// void Test(Town[] fiefs) {
//     foreach (var fief in fiefs) {
//         Log.WriteLine(fief);
//     }
// }

// void Test(MobileParty[] parties) {
//     Log.ToFile();
//     foreach (var party in parties) {
//         Log.WriteLine(party);
//     }
// }

void Foo() {
    Log.WriteLine("Foo");
}

void Bar(int x) {
    Log.WriteLine("Bar 1");
}

void Bar(string s) {
    Log.WriteLine("Bar 2");
}