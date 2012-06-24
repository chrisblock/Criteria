using System;
using System.Collections.Generic;

namespace Criteria.Registries.Impl
{
	public abstract class BaseJoinPathRegistry : IJoinPathRegistry
	{
		public ICollection<JoinPath> JoinPaths { get; private set; }
		public Dictionary<Type, bool> MultipleJoinLookup { get; private set; }

		protected BaseJoinPathRegistry()
		{
			JoinPaths = new List<JoinPath>();
			MultipleJoinLookup = new Dictionary<Type, bool>();
		}

		protected void RegisterOneTimeJoinFor<T>(Action<JoinPathConfigurator<T>> configureJoinConfigurator)
		{
			var configurator = new JoinPathConfigurator<T>(JoinPaths);

			configureJoinConfigurator(configurator);
		}

		private void AddToDictionary(Type type)
		{
			if (MultipleJoinLookup.ContainsKey(type) == false)
			{
				MultipleJoinLookup.Add(type, true);
			}
		}

		protected void RegisterOneToManyJoinFor<T>(Func<JoinPathConfigurator<T>, JoinPathToConfigurator> configureJoinConfigurator)
		{
			var configurator = new JoinPathConfigurator<T>(JoinPaths, true);

			var result = configureJoinConfigurator(configurator);
			
			AddToDictionary(result.OuterType);
		}
	}
}
