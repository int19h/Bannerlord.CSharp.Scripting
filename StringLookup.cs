namespace Int19h.Bannerlord.CSharp.Scripting {
    public abstract class StringLookup : Lookup {
        private readonly string s;

        public StringLookup(string s) {
            this.s = s;
        }

        public override string ToString() => s;

        public static implicit operator string(StringLookup lookup) => lookup.ToString();

        public override bool Matches<T>(T x) => this == Key(x);

        protected abstract string? Key<T>(T x);
    }
}
