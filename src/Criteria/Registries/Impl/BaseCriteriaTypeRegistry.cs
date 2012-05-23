using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Criteria.Json;

namespace Criteria.Registries.Impl
{
	public class BaseCriteriaTypeRegistry : ICriteriaTypeRegistry
	{
		private readonly IDictionary<string, TypeRegistryResult> _typeRegistry;

		public BaseCriteriaTypeRegistry()
		{
			_typeRegistry = new Dictionary<string, TypeRegistryResult>();
		}

		public virtual TypeRegistryResult Lookup(string key)
		{
			TypeRegistryResult result;

			if(_typeRegistry.TryGetValue(key, out result) == false)
			{
				throw new ArgumentException(String.Format("No registry entry found for criteria key \"{0}\".", key));
			}

			return result;
		}

		public virtual TypeRegistryResult Lookup(ICriteriaLeaf leaf)
		{
			return Lookup(leaf.Key);
		}

		private static TypeRegistryResult BuildTypeRegistryResult<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertyAccessor)
		{
			var lambda = propertyAccessor as LambdaExpression;

			if(lambda == null)
			{
				throw new ArgumentException(String.Format("\"{0}\" is not a lambda expression.", propertyAccessor));
			}

			var result = new TypeRegistryResult
			{
				AccessorExpression = lambda.Body,
				EntityType = typeof(TEntity),
				PropertyType = typeof(TProperty)
			};

			return result;
		}

		protected void RegisterCriteriaKey<TEntity, TProperty>(string keyValue, Expression<Func<TEntity, TProperty>> propertyAccessor)
		{
			_typeRegistry[keyValue] = BuildTypeRegistryResult(propertyAccessor);
		}
	}
}
