using System;
using System.Reflection;
using System.Collections.Generic;
namespace Helper.Core
{
    public sealed class Function
    {
        public readonly Assembly sourceAssembly = null;
        public readonly Type sourceType = null;
        public readonly ParameterInfo[] parameters = null;
        public readonly MethodInfo sourceMethod = null;
        public readonly RegisterFunctionAttribute registerFunctionAttribute = null;
        public string name
        {
            get
            {
                if (registerFunctionAttribute.methodNameIsName)
                {
                    return sourceMethod.Name;
                }
                else
                {
                    return registerFunctionAttribute.customName;
                }
            }
        }
        public string description
        {
            get
            {
                return registerFunctionAttribute.description;
            }
        }
        public string[] aliases
        {
            get
            {
                return registerFunctionAttribute.aliases;
            }
        }
        public Function(MethodInfo sourceMethod)
        {
            if (sourceMethod is null)
            {
                throw new NullReferenceException($"Could not create function from method because method was null.");
            }
            registerFunctionAttribute = sourceMethod.GetCustomAttribute<RegisterFunctionAttribute>();
            if (registerFunctionAttribute is null)
            {
                throw new NullReferenceException($"Could not create function from method \"{sourceMethod.Name}\" beacuse it does not have the RegisterFunction attribute.");
            }
            if (!sourceMethod.IsStatic)
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method was not static.");
            }
            if (!sourceMethod.IsPublic)
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method was not public.");
            }
            if (sourceMethod.ContainsGenericParameters || sourceMethod.IsGenericMethod || sourceMethod.IsGenericMethodDefinition)
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method contains generic types.");
            }
            this.sourceMethod = sourceMethod;
            parameters = sourceMethod.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.IsOut)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was an out parameter.");
                }
                if (parameter.HasDefaultValue)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" has a default value.");
                }
                if (parameter.IsOptional)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as optional.");
                }
                if (parameter.IsLcid)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as a Lcid.");
                }
                if (parameter.IsRetval)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as retval.");
                }
            }
            sourceType = sourceMethod.DeclaringType;
            if (sourceType is null)
            {
                throw new NullReferenceException($"Could not create function from method \"{sourceMethod.Name}\" beacuse its source type was null.");
            }
            sourceAssembly = sourceMethod.DeclaringType.Assembly;
            if (sourceAssembly is null)
            {
                throw new NullReferenceException($"Could not create function from method \"{sourceMethod.Name}\" beacuse its source assembly was null.");
            }
        }
        public object Invoke(List<object> arguments)
        {
            if (arguments is null)
            {
                arguments = new List<object>();
            }
            if (arguments.Count < parameters.Length)
            {
                throw new ArgumentException($"No argument was provided for property \"{parameters[arguments.Count].Name}\".");
            }
            else if (arguments.Count > parameters.Length)
            {
                throw new ArgumentException("Too many arguments were provided.");
            }
            return sourceMethod.Invoke(null, arguments.ToArray());
        }
        public override string ToString()
        {
            return $"Helper.Function(\"{name}\", \"{description}\")";
        }
    }
}
