using System;
using System.Configuration;
using System.Threading;

using HibernatingRhinos.Profiler.Appender.NHibernate;

namespace Criteria.NHibernate.IntegrationTests.App_Start
{
	public static class NHibernateProfilerBootstrapper
	{
		private static readonly Lazy<bool> LazyIsProfiling = new Lazy<bool>(IsProfilingNHibernate, LazyThreadSafetyMode.ExecutionAndPublication);
		private static bool IsProfiling { get { return LazyIsProfiling.Value; } }

		private static bool IsProfilingNHibernate()
		{
			var result = false;
			var config = ConfigurationManager.AppSettings["ProfileNHibernate"];

			bool profile;
			if ((String.IsNullOrWhiteSpace(config) == false) && Boolean.TryParse(config, out profile) && profile)
			{
				result = true;
			}

			return result;
		}

		public static void PreStart()
		{
			if (IsProfiling)
			{
				NHibernateProfiler.Initialize();
			}
		}

		public static void PostStop()
		{
			if (IsProfiling)
			{
				NHibernateProfiler.Stop();
			}
		}
	}
}
