using Asce.Managers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Asce.Managers.Utils
{
    public static class TypeCacheUtils
    {
        public static List<Type> GetConcreteSubclassesOf<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => SafeGetTypes(x))
                .Where(t => typeof(T).IsAssignableFrom(t)
                            && !t.IsAbstract
                            && !t.IsInterface)
                .ToList();
        }

        public static string GetMenuName(Type type)
        {
            return type.GetCustomAttribute<MenuNameAttribute>()?.DisplayName ?? type.Name;
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try 
            { 
                return assembly.GetTypes(); 
            }
            catch (ReflectionTypeLoadException e) 
            { 
                return e.Types.Where(t => t != null); 
            }
        }
    }
}