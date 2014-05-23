using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace PBT
{
	public static class TypeExporter
    {
        // returns the name of a type without any generic parameter indicators
        private static string CleanTypeName(Type type)
        {
            string name = type.ToString();
            if (type.IsGenericType)
                return name.Substring(0, name.LastIndexOf('`'));
            return name;
        }

        // exports subtypes (implementations) of a task category. TODO: clean this up. the parameters are a mess!
		private static void ExportAllSubTypes(XmlTextWriter writer, Assembly[] assemblies, Type parentType, Type dataType, Type impulseType, int skip, HashSet<Type> enums)
		{
			foreach(var assembly in assemblies)
			{
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.BaseType != null)
                        {
                            Type t = type.BaseType;
                            while (t != null && t.Name != parentType.Name)
                                t = t.BaseType;
                            if (t == null || !t.IsGenericType)
                                continue;
                            Type genericType = t.GetGenericArguments()[0];
                            if (!genericType.IsGenericParameter && genericType != dataType)
                                continue;

                            var parameters = type.GetConstructors()[0].GetParameters();
                            writer.WriteStartElement(CleanTypeName(type));
                            for (int i = skip; i < parameters.Length; i++)
                            {
                                Type unwrappedType = Expression.UnwrapType(parameters[i].ParameterType);
                                if (unwrappedType == typeof (string) && parameters[i].Name == "impulse")
                                    unwrappedType = impulseType;
                                if (unwrappedType.IsEnum || unwrappedType.GetInterface("ICustomEnum") != null)
                                    enums.Add(unwrappedType);
                                writer.WriteAttributeString(parameters[i].Name, unwrappedType.ToString());
                            }
                            writer.WriteEndElement();
                        }
                    }
                }
                catch(ReflectionTypeLoadException re)
                {
                    var sb = new StringBuilder();
                    foreach (var type in re.LoaderExceptions)
                    {
                        sb.AppendLine(type.ToString());
                    }
                    Console.WriteLine("[PBT.PBT.ExportAllSubTypes] ReflectionTypeLoadException:\n{0}", sb);
                }
			}
		}

        // exports the specified enum types
        private static void ExportAllEnums(XmlTextWriter writer, HashSet<Type> enums)
        {
            foreach (var type in enums)
            {
                writer.WriteStartElement(CleanTypeName(type));
                if (type.IsEnum)
                {
                    writer.WriteAttributeString("names", string.Join(", ", Enum.GetNames(type)));
                    //writer.WriteAttributeString("values", string.Join(", ", Enum.GetValues(type)));
                }
                else
                {
                    writer.WriteAttributeString("names", string.Join(", ", (string[])type.GetProperty("Names", BindingFlags.Public | BindingFlags.Static).GetValue(null, new object[0])));
                }
                writer.WriteEndElement();
            }
        }

        // exports all task types from the three categories and the relevant enum types for the editor into an xml writer
        public static void ExportAllTaskTypes(Type dataType, Type impulseType, XmlTextWriter writer)
        {
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            HashSet<Type> enums = new HashSet<Type>();

            writer.WriteStartDocument();
            writer.WriteStartElement("TaskTypes");
                writer.WriteStartElement("Categories");
                    writer.WriteStartElement("Decorators");
                        ExportAllSubTypes(writer, assemblies, typeof(TaskDecorator<>), dataType, impulseType, 2, enums);
                    writer.WriteEndElement();
                    writer.WriteStartElement("LeafTasks");
                        ExportAllSubTypes(writer, assemblies, typeof(LeafTask<>), dataType, impulseType, 1, enums);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ParentTasks");
                        ExportAllSubTypes(writer, assemblies, typeof(ParentTask<>), dataType, impulseType, 2, enums);
                    writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("Enums");
                    ExportAllEnums(writer, enums);
                writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        // exports all task types from the three categories and the relevant enum types for the editor into an xml file
		public static void ExportAllTaskTypes(Type dataType, Type impulseType, string filename)
		{
			XmlTextWriter writer = new XmlTextWriter(filename, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
            ExportAllTaskTypes(dataType, impulseType, writer);
			writer.Close();
        }
    }
}

