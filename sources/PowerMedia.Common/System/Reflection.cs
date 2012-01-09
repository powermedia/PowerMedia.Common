using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PowerMedia.Common.System.Reflection
{
    public static class ReflectionUtils
    {
        private static void ValidateArgument<T>(T element) where T : class
        {
            if (element == null)
            {
                throw new ArgumentNullException();
            }
        }

        //TODO: extract to pm.commons
        public static bool DoesImplementInterface(Type possibleImplementor, Type interfaceMeta)
        {
            if (possibleImplementor == null)
            {
                throw new ArgumentNullException();
            }
            var interfaces = possibleImplementor.GetInterfaces();
            return interfaces.Contains(interfaceMeta);
        }

        //TODO: extract to pm.commons
        /// <summary>
        /// example: DoesImplementGenericInterface( obj, typeof(IEnumerable), typeof(int) ) means:
        /// does obj implement IEnumerable(int) ?
        /// </summary>
        /// <param name="possibleImplementor"></param>
        /// <param name="interfaceMeta"></param>
        /// <param name="interfaceGenericArgument"></param>
        /// <returns></returns>
        public static bool DoesImplementGenericInterface(Type possibleImplementor, Type interfaceMeta, Type interfaceGenericArgument)
        {
            var interfaces = possibleImplementor.GetInterfaces();
            var interfacesAndImplementor = interfaces.ToList();
            interfacesAndImplementor.Add(possibleImplementor);

            var genericInterfacesQuery = from i in interfacesAndImplementor
                                         where i.IsGenericType &&
                                         i.FullName.Contains(interfaceMeta.Name)
                                         select i;

            if (!genericInterfacesQuery.Any()) { return false; }

            if (genericInterfacesQuery.Count() > 1) { throw new InvalidOperationException(); }

            var genericInterface = genericInterfacesQuery.First();
            var genericArguments = genericInterface.GetGenericArguments();

            if (genericArguments.Count() != 1) { throw new InvalidOperationException(); }

            var genericArgument = genericArguments.First();
            return genericArgument == interfaceGenericArgument;
        }
        
        public static Type GetFirstGenericArgument(object obj)
        {        	
        	var type = obj.GetType();
        	var intefaces = type.GetInterfaces();
		
        	foreach(Type currentInterface in intefaces)
        	{
        		if ( currentInterface.IsGenericType == false)
        		{
        			continue;
        		}
        		var genericArguments = currentInterface.GetGenericArguments();
        		if ( genericArguments.Count()>0 )
        		{
        			return genericArguments[0];        			
        		}
        	}
        	throw new ArgumentException("No generic argument found");
        }

        //TODO: extract to pm.commons
        public static bool DoesImplementInterface(object obj, Type interfaceMeta)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            return DoesImplementInterface(obj.GetType(), interfaceMeta);
        }

        //TODO: extract to pm.commons
        public static bool DoesImplementIEnumerable(object possibleIEnumerable)
        {
            return DoesImplementInterface(possibleIEnumerable, typeof(IEnumerable));
        }

        public static IList CreateIListOfType(Type type)
        {
            Type genericList = typeof(List<>).MakeGenericType(new Type[] { type });
            var constructor = genericList.GetConstructor(Type.EmptyTypes);
            return (IList)constructor.Invoke(null);
        }

        public static bool IsNullable<T>(T obj)
        {
            if (obj == null) return true; // obvious
            Type type = typeof(T);
            if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        // TODO: refactor
        public static bool IsDefaultValueOfType<T>(T obj) where T : class, IComparable
        {
            bool isNullable = IsNullable<T>(obj);
            //To be sure the object is inserted manually. Not null, not default value.
            //object is nullable and is null 
            // or object is not nullable, which means it's value-type. Then compare it with its default value			
            if ((isNullable && obj == null) ||
                (!isNullable && obj.CompareTo(default(T)) == 0))
            {
                return true;
            }
            return false;
        }

        public static object GetDefault(Type type)
        {
            if(type.IsValueType)
            {
              return Activator.CreateInstance(type);
            }
            return null;
        }

        public static bool AreObjectPropertiesInDefaultState(object obj)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var currentProperty in properties)
            {
                var value = currentProperty.GetValue(obj, null);
                var defaultValue = GetDefault(currentProperty.PropertyType);
                if (currentProperty.PropertyType == typeof(string) && String.IsNullOrEmpty(value as string))
                {
                    continue;
                }
                if (defaultValue == null && value == null)
                {
                    continue;
                }   
                if ( defaultValue == null && value != null )
                {
                    return false;
                }                             
                if ( ! defaultValue.Equals(value) )
                {
                    return false;
                }
            }
            return true;
        }

    }
}
