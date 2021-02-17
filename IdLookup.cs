using TaleWorlds.ObjectSystem;

namespace Int19h.Bannerlord.CSharp.Scripting {
    public class IdLookup : StringLookup {
        public IdLookup(string s) : base(s) { }

        public override string What => "StringId";

        protected override string? Key<T>(T x) => (x as MBObjectBase)?.StringId;
    }
}
