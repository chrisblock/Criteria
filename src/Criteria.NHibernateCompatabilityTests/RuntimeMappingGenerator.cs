using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;

using FluentNHibernate;
using FluentNHibernate.Mapping;

namespace Criteria.NHibernateCompatabilityTests
{
	public static class RuntimeMappingGenerator
	{
		private const string TableNameExpression = @"^\w+$";

		private static readonly AssemblyName AssemblyName;
		private static readonly ConcurrentDictionary<string, Type> RuntimeTypes;

		static RuntimeMappingGenerator()
		{
			AssemblyName = new AssemblyName { Name = "RuntimeMappingTypes" };
			RuntimeTypes = new ConcurrentDictionary<string, Type>();
		}

		private static ModuleBuilder _moduleBuilder;
		private static ModuleBuilder ModuleBuilder
		{
			get
			{
				return _moduleBuilder ?? (_moduleBuilder = Thread.GetDomain().DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name));
			}
		}

		private static bool IsValidTableName(string tableName)
		{
			return Regex.IsMatch(tableName, TableNameExpression);
		}

		public static Type ExtendMappingTypeForTable<TMapping>(string tableName) where TMapping : class, IMappingProvider
		{
			if (IsValidTableName(tableName) == false)
			{
				throw new ArgumentException(String.Format("'{0}' is not a valid table name.", tableName));
			}

			var baseType = typeof(TMapping);

			var baseConstructor = baseType.GetConstructor(Type.EmptyTypes);
			var tableMethod = baseType.GetMethod("Table", BindingFlags.Instance | BindingFlags.Public);

			var newTypeName = String.Format("RuntimeClassMapExtending_{0}_ForTable_{1}", baseType.Name, tableName);

			var type = RuntimeTypes.GetOrAdd(newTypeName, name =>
			{
				var typeBuilder = ModuleBuilder.DefineType(newTypeName, TypeAttributes.Serializable | TypeAttributes.Public | TypeAttributes.Class, baseType);

				var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

				var msilGenerator = constructorBuilder.GetILGenerator();

				msilGenerator.Emit(OpCodes.Ldarg_0);
				msilGenerator.Emit(OpCodes.Call, baseConstructor);

				msilGenerator.Emit(OpCodes.Ldarg_0);
				msilGenerator.Emit(OpCodes.Ldstr, tableName);
				msilGenerator.Emit(OpCodes.Call, tableMethod);
				msilGenerator.Emit(OpCodes.Ret);

				return typeBuilder.CreateType();
			});

			return type;
		}

		public static Type GenerateMappingFor<TEntity>(string tableName, IDictionary<string, Func<TEntity, object>> columnMappings)
		{
			var entityType = typeof (TEntity);
			var classMapType = typeof (ClassMap<>).MakeGenericType(entityType);

			if (IsValidTableName(tableName) == false)
			{
				throw new ArgumentException(String.Format("'{0}' is not a valid table name.", tableName));
			}

			var baseConstructor = classMapType.GetConstructor(Type.EmptyTypes);
			var tableMethod = classMapType.GetMethod("Table", BindingFlags.Instance | BindingFlags.Public);
			var mapMethod = classMapType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => (x.Name == "Map") && (x.GetParameters().Count() == 2))
				.Single();

			var newTypeName = String.Format("RuntimeClassMap_ForTable_{0}", tableName);

			var type = RuntimeTypes.GetOrAdd(newTypeName, name =>
			{
				var typeBuilder = ModuleBuilder.DefineType(newTypeName, TypeAttributes.Serializable | TypeAttributes.Public | TypeAttributes.Class, classMapType);

				var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);

				var msilGenerator = constructorBuilder.GetILGenerator();

				msilGenerator.Emit(OpCodes.Ldarg_0);
				msilGenerator.Emit(OpCodes.Call, baseConstructor);

				msilGenerator.Emit(OpCodes.Ldarg_0);
				msilGenerator.Emit(OpCodes.Ldstr, tableName);
				msilGenerator.Emit(OpCodes.Call, tableMethod);

				foreach (var columnMapping in columnMappings)
				{
					var columnName = columnMapping.Key;
					var columnFunc = columnMapping.Value;

					msilGenerator.Emit(OpCodes.Ldarg_0);

					// TODO: Emit the columnFunc as a parameter onto the stack for use in the Map function call later
					var localBuilder = msilGenerator.DeclareLocal(columnFunc.GetType());
					localBuilder.SetLocalSymInfo(String.Format("{0}_MappingLambda", columnName));

					msilGenerator.Emit(OpCodes.Ldind_Ref, localBuilder);

					msilGenerator.Emit(OpCodes.Ldstr, columnName);

					msilGenerator.Emit(OpCodes.Call, mapMethod);
				}

				msilGenerator.Emit(OpCodes.Ret);

				return typeBuilder.CreateType();
			});

			return type;
		}
	}
}
