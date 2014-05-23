using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace PBT
{
	public static class Parser
    {
        // tries to resolve a type from its name.
        // the type may have a generic parameter which is filled in with the specified type
		internal static Type GetType(string name, Type genericParam)
		{
            // try to get the type directly
			var type = Type.GetType(name);
            // try to get the same type with one generic parameter
			if(type == null)
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
			if(type.IsGenericTypeDefinition)
			{
                Type[] typeArgs = { genericParam };
				type = type.MakeGenericType(typeArgs);
			}
			return type;
		}

        // cache of all the type converters, that we've seen
        private static Dictionary<Type, TypeConverter> TypeConverters = new Dictionary<Type, TypeConverter>();

        // converts strings to objects of the given type if they specify a type converter
        private static object ParseObjectString(string str, Type type)
        {
            TypeConverter tc;
            if (!TypeConverters.TryGetValue(type, out tc))
                tc = TypeConverters[type] = TypeDescriptor.GetConverter(type);
            return tc.ConvertFromString(null, CultureInfo.InvariantCulture, str);
        }

        // parses (and compiles) a parameter if it contains a code expression, otherwise returns null
        private static object ParseExpressionParameter<DataType>(TaskContext<DataType> context, Type parameterType, string expressionString)
        {
            MethodInfo method = null;
            if (parameterType != typeof(Expression))
            {
                if (!expressionString.Contains("return"))
                    return null;
                method = typeof(Expression).GetMethods().First(m => m.Name == "Compile" && m.ContainsGenericParameters);
                method = method.MakeGenericMethod(parameterType);
            }
            else
            {
                method = typeof(Expression).GetMethods().First(m => m.Name == "Compile" && !m.ContainsGenericParameters);
            }
            object expression = method.Invoke(null, new object[] {
                            context.Usings,
                            expressionString,
                            new string[] { "data", "vars", "random" },
                            new object[] { context.Data, context.Variables, context.Random }});
            return expression;
        }

        // parses a parameter that directly contains a stringified value of the specified type
        private static object ParseParameter<DataType>(TaskContext<DataType> context, Type parameterType, string valueString, bool wrapInExpression)
        {
            var conversion = ParseObjectString(valueString, parameterType);
            if (wrapInExpression)
            {
                MethodInfo method = typeof(Expression).GetMethod("WrapResult");
                method = method.MakeGenericMethod(parameterType);
                var expression = method.Invoke(null, new object[] { conversion });
                return expression;
            }
            return conversion;
        }

        // tries to parse all the parameters as code expressions and as stringified values
        private static void ParseParams<DataType>(XmlTextReader reader, string source, TaskContext<DataType> context, ConstructorInfo constructorInfo, List<object> parameters)
		{
			var parameterInfos = constructorInfo.GetParameters();
			for(int i = parameters.Count; i < parameterInfos.Length; i++)
			{
				string textValue = reader.GetAttribute(parameterInfos[i].Name);
                if(textValue == null)
                    throw new Exception(
                        "\nSource:    " + source + ", Line: " + reader.LineNumber +
                        "\nTask:      " + constructorInfo.ReflectedType.ToString() +
                        "\nParameter: " + parameterInfos[i].Name +
                        "\nError:     Value not found!");

                Type type = Expression.UnwrapType(parameterInfos[i].ParameterType);
                bool wrapInExpression = type != parameterInfos[i].ParameterType;

                try
                {
                    var expression = ParseExpressionParameter<DataType>(context, type, textValue + string.Format(" /* Source: {0}, Line: {1} */", source, reader.LineNumber));
                    if (expression != null)
                        parameters.Add(expression);
                    else
                        parameters.Add(ParseParameter<DataType>(context, type, textValue, wrapInExpression));
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw new Exception(
                            "\nSource:    " + source + ", Line: " + reader.LineNumber +
                            "\nTask:      " + constructorInfo.ReflectedType.ToString() +
                            "\nParameter: " + parameterInfos[i].Name +
                            "\nValue:\n" + reader.GetAttribute(parameterInfos[i].Name) +
                            "\n\n" + ex.InnerException.Message, ex.InnerException);
                    }
                    else
                    {
                        throw new Exception(
                            "\nSource:    " + source + ", Line: " + reader.LineNumber +
                            "\nTask:      " + constructorInfo.ReflectedType.ToString() +
                            "\nParameter: " + parameterInfos[i].Name +
                            "\nValue:\n" + reader.GetAttribute(parameterInfos[i].Name) +
                            "\n\n" + ex.Message);
                    }
                }
			}
		}
		
        // parses a single task node and its children recursively
        private static Task<DataType> ParseTask<DataType>(XmlTextReader reader, string source, TaskContext<DataType> context)
		{
			int depth = reader.Depth;
			
			var type = GetType(reader.Name, typeof(DataType));
			var constructor = type.GetConstructors()[0];
			List<object> parameters = new List<object>() { context };
			
			if(type.IsSubclassOf(typeof(LeafTask<DataType>)))
			{
                ParseParams<DataType>(reader, source, context, constructor, parameters);
				while(reader.Depth != depth)
					reader.Read();
				return (LeafTask<DataType>)constructor.Invoke(parameters.ToArray());
			}
			
			if(type.IsSubclassOf(typeof(TaskDecorator<DataType>)))
			{
				parameters.Add(null);
                ParseParams<DataType>(reader, source, context, constructor, parameters);
				parameters[1] = Parse<DataType>(reader, source, context);
				while(reader.Depth != depth)
					reader.Read();
				return (TaskDecorator<DataType>)constructor.Invoke(parameters.ToArray());
			}
			
			if(type.IsSubclassOf(typeof(ParentTask<DataType>)))
			{
                parameters.Add(null);
                ParseParams<DataType>(reader, source, context, constructor, parameters);
                List<Task<DataType>> subtasks = new List<Task<DataType>>();

				while(true)
				{
					var subtask = Parse<DataType>(reader, source, context);
					if(subtask == null)
						break;
                    subtasks.Add(subtask);
				}
                parameters[1] = subtasks.ToArray();
                var task = (ParentTask<DataType>)constructor.Invoke(parameters.ToArray());

				while(reader.Depth > depth)
					reader.Read();
				return task;
			}
			
			throw new NotSupportedException(type.FullName);
		}

        // parses a pbt (sub)tree from an xml reader
        private static Task<DataType> Parse<DataType>(XmlTextReader reader, string name, TaskContext<DataType> context)
		{
			Task<DataType> task = null;
			while(reader.Read())
			{
				if(reader.NodeType == XmlNodeType.Element)
				{
                    task = ParseTask<DataType>(reader, name, context);
					break;
				}
				else if(reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
			}
			return task;
		}

        // parses a pbt (sub)tree from an encoded xml buffer
        internal static Task<DataType> Parse<DataType>(byte[] data, string name, TaskContext<DataType> context)
        {
            MemoryStream stream = new MemoryStream(data);
            XmlTextReader reader = new XmlTextReader(stream);
            var task = Parse<DataType>(reader, name, context);
            reader.Close();
            stream.Close();
            return task;
        }

        // parses a pbt (sub)tree from an xml file
        internal static Task<DataType> Parse<DataType>(string filename, TaskContext<DataType> context)
		{
			XmlTextReader reader = new XmlTextReader(filename);
			var task = Parse<DataType>(reader, filename, context);
			reader.Close();
			return task;
		}

        // creates a reference to a pbt file
        public static LeafTasks.Reference<DataType> Load<DataType, ImpulseType>(string directory, string name, DataType data, string[] usings, ILogger logger)
        {
            return new LeafTasks.Reference<DataType>(
                new TaskContext<DataType>(data, typeof(ImpulseType), usings, directory, logger),
                Expression.WrapResult<string>(name));
        }
    }
}