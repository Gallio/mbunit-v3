using System.Reflection;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Helper functions for the reflector.
    /// </summary>
    internal static class ReflectorUtils
    {
        public static void AddFlagIfTrue(ref TypeAttributes flags, TypeAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref MethodAttributes flags, MethodAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref FieldAttributes flags, FieldAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref PropertyAttributes flags, PropertyAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }

        public static void AddFlagIfTrue(ref ParameterAttributes flags, ParameterAttributes flagToAdd, bool condition)
        {
            if (condition)
                flags |= flagToAdd;
        }
    }
}
