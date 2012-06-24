using System;
using System.Configuration;

using HibernatingRhinos.Profiler.Appender.NHibernate;

namespace Criteria.NHibernate.IntegrationTests.App_Start
{
	public static class NHibernateProfilerBootstrapper
	{
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
			if (IsProfilingNHibernate())
			{
				NHibernateProfiler.Initialize();
			}
		}

		public static void PostStop()
		{
			if (IsProfilingNHibernate())
			{
				NHibernateProfiler.Stop();
			}
		}
	}
}

