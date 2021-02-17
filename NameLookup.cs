namespace Int19h.Bannerlord.CSharp.Scripting {
    public class NameLookup : StringLookup {
        public NameLookup(string s) : base(s) { }

        public override string What => "Name";

        protected override string? Key<T>(T x) => $"{((dynamic?)x)?.Name}";
    }
}
