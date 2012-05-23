using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Criteria.Joins
{
	public static class AnonymousClassManager
	{
		private static readonly object TypeCacheLocker = new object();
		private static readonly AssemblyName AssemblyName;
		private static readonly ConcurrentDictionary<string, Type> TypeCache;

		static AnonymousClassManager()
		{
			AssemblyName = new AssemblyName { Name = "DynamicLinqTypes" };
			TypeCache = new ConcurrentDictionary<string, Type>();
		}

		private static string GenerateClassName(IEnumerable<Type> fieldTypes)
		{
			var sortedFieldTypeNames = fieldTypes.Select(x => x.Name)
				.OrderBy(x => x);

			return String.Join("_", sortedFieldTypeNames);
		}

		private static ModuleBuilder _moduleBuilder;
		private static ModuleBuilder ModuleBuilder
		{
			get
			{
				return _moduleBuilder ?? (_moduleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name));
			}
		}

		public static Type BuildNewAnonymousTypeWithFields(IEnumerable<Type> types)
		{
			var fieldTypes = types.ToList();
			var className = GenerateClassName(fieldTypes);

			if(TypeCache.ContainsKey(className) == false)
			{
				lock(TypeCacheLocker)
				{
					if(TypeCache.ContainsKey(className) == false)
					{
						var typeBuilder = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

						for(var i = 0; i < fieldTypes.Count; i++)
						{
							// TODO: make these things properties??
							//typeBuilder.DefineProperty(String.Format("@p{0}", i), PropertyAttributes.None, CallingConventions.HasThis, fieldTypes.ElementAt(i), Type.EmptyTypes);
							typeBuilder.DefineField(String.Format("@p{0}", i), fieldTypes[i], FieldAttributes.Public);
						}

						var newType = typeBuilder.CreateType();
						TypeCache.TryAdd(className, newType);
					}
				}
			}

			return TypeCache[className];
		}
	}
}
