// void Test(object o) {
//     Log.WriteLine(o);
// }

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

float Test(int x, float y = 1) {
    Log.WriteLine($"int {x}; float {y};");
    return x * y;
}

void Test(bool z) {
    MessageBox("Foo");
}

// void Test(string s) {
//     Log.WriteLine($"string {s}");
// }

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
//     foreach (var party in parties) {
//         Log.WriteLine(party);
//     }
// }

// void Test(Settlement[] settlements) {
//     foreach (var settlement in settlements) {
//         Log.WriteLine(settlement);
//     }
// }

void Test(ItemObject[] items) {
    foreach (var item in items) {
        Log.WriteLine(item);
    }
}

int Foo() {
    Log.WriteLine("Foo!");
    return 123;
}

void Bar(int x) {
    Log.WriteLine("Bar 1");
}

void Bar(string s) {
    Log.WriteLine("Bar 2");
}