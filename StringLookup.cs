namespace Int19h.Bannerlord.CSharp.Scripting {
    public abstract class StringLookup : Lookup {
        private readonly string s;

        public StringLookup(string s) {
            this.s = s;
        }

        public override string ToString() => s;

        protected abstract string? Key<T>(T x);

        public override bool Matches<T>(T x) => Key(x) == s;
    }
}
