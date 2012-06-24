using System;
using System.Collections.Generic;
using System.Reflection;

namespace Criteria.Sql.Impl
{
	public abstract class BaseMappingAssemblyContainer : IMappingAssemblyContainer
	{
		private readonly IList<Assembly> _assemblies;

		protected BaseMappingAssemblyContainer()
		{
			_assemblies = new List<Assembly>();
		}

		public IEnumerable<Assembly> GetMappingAssemblies()
		{
			return _assemblies;
		}

		protected void AddAssembly(Assembly assembly)
		{
			_assemblies.Add(assembly);
		}

		protected void AddAssemblyContainingType<T>()
		{
			AddAssemblyContainingType(typeof(T));
		}

		protected void AddAssemblyContainingType(Type type)
		{
			_assemblies.Add(type.Assembly);
		}
	}
}
