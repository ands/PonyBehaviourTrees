using System;
using System.IO;
using System.Xml;

namespace PBT
{
    /// <summary>
    /// Provides static methods to test the compatibility of pbt source code with a particular task context data type.
    /// </summary>
	public static class Compatibility
    {
        /// <summary>
        /// Tests the compatibility of the provided pbt source with the specified task context data type.
        /// </summary>
        /// <param name="reader">The xml reader that contains the pbt source.</param>
        /// <param name="dataType">The task context data type.</param>
        /// <returns>Returns true if the pbt source contains only tasks that are generic or that derive from task with the specified data type as the generic parameter.</returns>
        public static bool IsCompatible(XmlTextReader reader, Type dataType)
        {
            Type taskType = typeof(Task<>);
            taskType = taskType.MakeGenericType(new Type[] { dataType }); // TODO: test

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    Type t = Parser.GetType(reader.Name, dataType);
                    if (t == null)
                        return false;
                    while (t != null && t.Name != taskType.Name)
                        t = t.BaseType;
                    Type genericType = t.GetGenericArguments()[0];
                    if (!genericType.IsGenericParameter && genericType != dataType)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tests the compatibility of the provided pbt source with the specified task context data type.
        /// </summary>
        /// <param name="filename">The xml file that contains the pbt source.</param>
        /// <param name="dataType">The task context data type.</param>
        /// <returns>Returns true if the pbt source contains only tasks that are generic or that derive from task with the specified data type as the generic parameter.</returns>
        public static bool IsCompatible(string filename, Type dataType)
        {
            if (!File.Exists(filename)) // everything is compatible with an empty tree? :P
                return true;

            XmlTextReader reader = new XmlTextReader(filename);
            bool compatible = IsCompatible(reader, dataType);
            reader.Close();
            return compatible;
        }
    }
}

