namespace Int19h.Bannerlord.CSharp.Scripting {
    internal static class Helpers {
        private static bool IsAssignableToImpl<TTarget>(TTarget x) => true;

        private static bool IsAssignableToImpl<TTarget>(object x) => typeof(TTarget) == typeof(object);

        public static bool IsAssignableTo<TTarget>(dynamic x) => IsAssignableToImpl<TTarget>(x);
    }
}
