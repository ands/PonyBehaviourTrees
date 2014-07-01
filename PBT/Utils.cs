using System;
using System.Reflection;

namespace PBT
{
    /// <summary>
    /// PBT Utilities.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Tries to resolve a type from the given name.
        /// The type may have a generic parameter which is filled in with the specified type.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <param name="genericParam">The generic parameter.</param>
        /// <returns>Returns the combined type.</returns>
        public static Type GetType(string name, Type genericParam)
        {
            // try to get the type directly
            var type = Type.GetType(name);
            // try to get the same type with one generic parameter
            if (type == null)
                type = Type.GetType(name + "`1");
            // if everything fails, try the same in all assemblies separately
            if (type == null)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = assembly.GetType(name);
                    if (type != null)
                        break;
                    type = assembly.GetType(name + "`1");
                    if (type != null)
                        break;
                }
            }

            if (type == null)
                throw new Exception(string.Format("Type \"{0}\" not found!", name));

            // add the generic parameter to types that ask for one
            if (type.IsGenericTypeDefinition && genericParam != null)
            {
                Type[] typeArgs = { genericParam };
                type = type.MakeGenericType(typeArgs);
            }
            return type;
        }
    }
}
