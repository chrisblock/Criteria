using System;
using System.Linq;

using NHibernate;
using NHibernate.Linq;

namespace Criteria.NHibernate
{
	public class NHibernateQueryableProvider : IQueryableProvider
	{
		private readonly ISession _session;

		public NHibernateQueryableProvider(ISession session)
		{
			_session = session;
		}

		public IQueryable<T> GetQueryableFor<T>()
		{
			return _session.Query<T>();
		}

		public IQueryable GetQueryable(Type type)
		{
			var queryable = GetType().GetMethod("GetQueryableFor").MakeGenericMethod(new[] { type }).Invoke(this, new object[0]) as IQueryable;

			return queryable;
		}
	}
}
