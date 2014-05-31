using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace PBT
{
    /// <summary>
    /// An expression that does not return anything.
    /// Use this class to compile expressions.
    /// </summary>
    public class Expression
    {
        private static string codeTemplate = @"{0}{1} class ExpressionCode {{ public {2} Execute(object[] executionParams) {{ {3}; }} }}";
        private static string pragmas = "";
        private static string paramTemplate = "var {0} = ({1})executionParams[executionParam++];";
        private static CSharpCodeProvider compiler = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });

        private static Dictionary<string, Action<object[]>> actionCache = new Dictionary<string, Action<object[]>>();
        private static Dictionary<string, Func<object[], object>> funcCache = new Dictionary<string, Func<object[], object>>();

        [DebuggerStepThrough]
        private static object Compile(string[] usingsList, string returnType, string expressionString, string[] parameterNames, object[] parameterValues)
        {
            if (parameterNames.Length != parameterValues.Length)
                throw new ArgumentException("The length of parameterValues must be equal to the length of parameterNames!");

            var compilerParams = new CompilerParameters()
            {
                CompilerOptions = "/warn:0",
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            string[] doNotReference;
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                doNotReference = new string[] {
                    "Microsoft.CSharp.dll",
                    "mscorlib.dll",
                    "System.dll",
                    "System.Xml.dll",
                    "System.Core.dll"
                };
            }
            else
            {
                doNotReference = new string[] {
                    "Microsoft.CSharp.dll"
                };
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                    continue;

                try
                {
                    string location = assembly.Location;
                    if (!String.IsNullOrEmpty(location))
                    {
                        if(!doNotReference.Any(r => location.Contains(r)))
                            compilerParams.ReferencedAssemblies.Add(location);
                    }
                }
                catch (NotSupportedException)
                {
                    // this happens for dynamic assemblies, so just ignore it. 
                }
            }
            compilerParams.ReferencedAssemblies.Add("Microsoft.CSharp.dll");

            string usings = string.Concat(usingsList.Select(u => "using " + u + ";").ToArray());

            string code = "int executionParam = 0;";
            for (int i = 0; i < parameterNames.Length; i++)
                code += string.Format(paramTemplate, parameterNames[i], parameterValues[i].GetType().FullName);
            code += "\n";
            code += expressionString;

            code = string.Format(codeTemplate, pragmas, usings, returnType, code);
            var results = compiler.CompileAssemblyFromSource(compilerParams, new string[] { code });

            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                    if (!error.IsWarning)
                        errors.AppendFormat("Line {0}, Column {1}:\n{2}\n\n", error.Line - 2, error.Column, error.ErrorText);
                errors.Append("\n");

                var ex = new TypeLoadException(errors.ToString());
                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        var key = new Tuple<string, int, int>(error.ErrorNumber, error.Line - 2, error.Column);
                        if(!ex.Data.Contains(key))
                            ex.Data.Add(key, error.ErrorText);
                    }
                }
                throw ex;
            }

            return results.CompiledAssembly.CreateInstance("ExpressionCode");
        }

        /// <summary>
        /// Compiles an expression that does not return anything.
        /// </summary>
        /// <param name="usings">The csharp usings for the code.</param>
        /// <param name="expressionString">The csharp code.</param>
        /// <param name="parameterNames">The list of variable names that will be set prior to executing the code.</param>
        /// <param name="parameterValues">The list of variable values that will be set prior to executing the code.</param>
        /// <returns>Returns the compiled expression.</returns>
        [DebuggerStepThrough]
        public static Expression Compile(string[] usings, string expressionString, string[] parameterNames, object[] parameterValues)
        {
            Action<object[]> execute;
            if (!actionCache.TryGetValue(expressionString, out execute))
            {
                object instance = Compile(usings, "void", expressionString, parameterNames, parameterValues);
                execute = (Action<object[]>)Delegate.CreateDelegate(typeof(Action<object[]>), instance, "Execute");
                actionCache.Add(expressionString, execute);
            }
            return new Expression(expressionString, execute, parameterValues);
        }

        /// <summary>
        /// Compiles an expression that returns something.
        /// </summary>
        /// <typeparam name="ReturnType">The type of the return value.</typeparam>
        /// <param name="usings">The csharp usings for the code.</param>
        /// <param name="expressionString">The csharp code.</param>
        /// <param name="parameterNames">The list of variable names that will be set prior to executing the code.</param>
        /// <param name="parameterValues">The list of variable values that will be set prior to executing the code.</param>
        /// <returns>Returns the compiled expression.</returns>
        [DebuggerStepThrough]
        public static Expression<ReturnType> Compile<ReturnType>(string[] usings, string expressionString, string[] parameterNames, object[] parameterValues)
        {
            Func<object[], object> execute;
            if (!funcCache.TryGetValue(expressionString, out execute))
            {
                if (typeof(ReturnType).GetInterface("ICustomEnum") != null)
                {
                    object instance = Compile(usings, "int", expressionString, parameterNames, parameterValues);
                    var exec = (Func<object[], int>)Delegate.CreateDelegate(typeof(Func<object[], int>), instance, "Execute");
                    var conv = (Func<int, ReturnType>)Delegate.CreateDelegate(typeof(Func<int, ReturnType>), typeof(ReturnType), "FromValue");
                    execute = o => conv(exec(o));
                }
                else
                {
                    object instance = Compile(usings, "object", expressionString, parameterNames, parameterValues);
                    execute = (Func<object[], object>)Delegate.CreateDelegate(typeof(Func<object[], object>), instance, "Execute");
                }
                funcCache.Add(expressionString, execute);
            }
            return new Expression<ReturnType>(expressionString, execute, parameterValues);
        }

        /// <summary>
        /// Creates a wrapper that behaves like an expression, but just returns the specified value.
        /// </summary>
        /// <typeparam name="ReturnType">The type of the return value.</typeparam>
        /// <param name="result">The return value.</param>
        /// <returns>Returns the specified value.</returns>
        public static Expression<ReturnType> WrapResult<ReturnType>(ReturnType result)
        {
            return new ResultExpression<ReturnType>(result);
        }

        /// <summary>
        /// If called with typeof(Expression&lt;ReturnValue&gt;), returns typeof(ReturnValue); otherwise, the specified type.
        /// </summary>
        /// <param name="type">The expression type to unwrap.</param>
        /// <returns>Returns typeof(ReturnValue) if called with typeof(Expression&lt;ReturnValue&gt;); otherwise, the specified type.</returns>
        public static Type UnwrapType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>))
            {
                var genericArguments = type.GetGenericArguments();
                return genericArguments[0];
            }
            return type;
        }

        private string code;
        private Action<object[]> execute;
        private object[] parameterValues;

        internal Expression(string code, Action<object[]> execute, object[] parameterValues)
        {
            this.code = code;
            this.execute = execute;
            this.parameterValues = parameterValues;
        }

        /// <summary>
        /// Executes the compiled code of the expression.
        /// </summary>
        public void Execute()
        {
            try
            {
                execute(parameterValues);
            }
            catch (Exception e)
            {
                throw new Exception(code, e);
            }
        }

        /// <summary>
        /// Shows the source code of the expression.
        /// </summary>
        /// <returns>Returns the source code of the expression.</returns>
        public override string ToString()
        {
            return code;
        }
    }

    /// <summary>
    /// An expression that returns something.
    /// </summary>
    /// <typeparam name="ReturnType">The type of the return value.</typeparam>
    public class Expression<ReturnType>
    {
        private string code;
        private Func<object[], object> execute;
        private object[] parameterValues;

        internal Expression()
        {
        }

        internal Expression(string code, Func<object[], object> execute, object[] parameterValues)
        {
            this.code = code;
            this.execute = execute;
            this.parameterValues = parameterValues;
        }

        /// <summary>
        /// Executes the compiled code of the expression.
        /// </summary>
        /// <returns>Returns the return value of the execution.</returns>
        public virtual ReturnType Execute()
        {
            try
            {
                ReturnType retValue = (ReturnType)execute(parameterValues);
                return retValue;
            }
            catch (Exception e)
            {
                throw new Exception(code, e);
            }
        }

        /// <summary>
        /// Implicit ReturnType evaluator.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator ReturnType(Expression<ReturnType> value)
        {
            return value.Execute();
        }

        /// <summary>
        /// Shows the source code of the expression.
        /// </summary>
        /// <returns>Returns the source code of the expression.</returns>
        public override string ToString()
        {
            return code;
        }
    }

    internal class ResultExpression<ReturnType> : Expression<ReturnType>
    {
        private ReturnType result;

        internal ResultExpression(ReturnType result)
        {
            this.result = result;
        }

        public override ReturnType Execute()
        {
            return result;
        }

        public override string ToString()
        {
            return result.ToString();
        }
    }
}
