using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace PBT
{
    /// <summary>
    /// A Static class that can retrieve the documentation from assembly xml files.
    /// </summary>
    public static class Documentation
    {
        /// <summary>
        /// Retrieves the documentation for the specified type.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>Returns the documentation xml element.</returns>
        public static XmlElement Get(Type t)
        {
            return Get(t.Assembly, "T:" + t.FullName);
        }

        /// <summary>
        /// Retrieves the documentation for the specified member.
        /// </summary>
        /// <param name="m">The member info.</param>
        /// <returns>Returns the documentation xml element.</returns>
        public static XmlElement Get(MemberInfo m)
        {
            return Get(m.DeclaringType.Assembly, m.MemberType.ToString()[0] + ":" + m.DeclaringType.FullName + "." + m.Name);
        }

        /// <summary>
        /// Retrieves the documentation for the specified method.
        /// </summary>
        /// <param name="m">The method info.</param>
        /// <returns>Returns the documentation xml element.</returns>
        public static XmlElement Get(MethodInfo m)
        {
            string p = string.Join(",", m.GetParameters().Select(mp => mp.ParameterType.FullName));
            return Get(m.DeclaringType.Assembly, "M:" + m.DeclaringType.FullName + "." + m.Name /*+ (p.Length > 0 ? "(" + p + ")" : "")*/, true);
        }

        /// <summary>
        /// Retrieves the documentation for the specified constructor.
        /// </summary>
        /// <param name="m">The constructor info.</param>
        /// <returns>Returns the documentation xml element.</returns>
        public static XmlElement Get(ConstructorInfo m)
        {
            string p = string.Join(",", m.GetParameters().Select(mp => mp.ParameterType.FullName));
            return Get(m.DeclaringType.Assembly, "M:" + m.DeclaringType.FullName + ".#ctor" /*+ (p.Length > 0 ? "(" + p + ")" : "")*/, true);
        }

        private static Dictionary<Assembly, XmlDocument> loaded = new Dictionary<Assembly, XmlDocument>();
        private static XmlElement Get(Assembly assembly, string name, bool startsWith = false)
        {
            XmlDocument doc;
            if (!loaded.TryGetValue(assembly, out doc))
            {
                if (!assembly.CodeBase.StartsWith("file:///"))
                    throw new Exception("Invalid assembly filename");
                doc = new XmlDocument();
                doc.Load(new StreamReader(Path.ChangeExtension(assembly.CodeBase.Substring(8), ".xml")));
                loaded.Add(assembly, doc);
            }
            if (startsWith)
                return doc["doc"]["members"].Cast<XmlElement>().Single(e => e.Attributes["name"].Value.StartsWith(name));
            return doc["doc"]["members"].Cast<XmlElement>().Single(e => e.Attributes["name"].Value == name);
        }
    }
}