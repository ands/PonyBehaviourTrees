using System;
using System.IO;
using System.Xml;

namespace PBT
{
	public static class Compatibility
    {
        // tests compatibility of the pbt from the xml reader with the specified task context data type
        public static bool IsCompatible(XmlTextReader reader, Type dataType)
        {
            Type taskType = typeof(Task<>);
            taskType.MakeGenericType(new Type[] { dataType });

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

        // tests compatibility of the pbt from the xml file with the specified task context data type
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

