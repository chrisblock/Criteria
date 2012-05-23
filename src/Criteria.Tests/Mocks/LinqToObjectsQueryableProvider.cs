using System;
using System.Collections.Generic;
using System.Linq;

using Criteria.Tests.LinqToObjectsModel;

namespace Criteria.Tests.Mocks
{
	class LinqToObjectsQueryableProvider : IQueryableProvider
	{
		private readonly IQueryable<LinqToObjectsOne> _queryableOne= new List<LinqToObjectsOne>
			{
				new LinqToObjectsOne{LinqToObjectsOneId = 1, LinqToObjectsOneField = "OneOne"},
				new LinqToObjectsOne{LinqToObjectsOneId = 2, LinqToObjectsOneField = "OneTwo"},
				new LinqToObjectsOne{LinqToObjectsOneId = 3, LinqToObjectsOneField = "OneThree"},
				new LinqToObjectsOne{LinqToObjectsOneId = 4, LinqToObjectsOneField = "OneFour"},
				new LinqToObjectsOne{LinqToObjectsOneId = 5, LinqToObjectsOneField = "OneFive"},
				new LinqToObjectsOne{LinqToObjectsOneId = 6, LinqToObjectsOneField = "OneSix"}
			}.AsQueryable();

		private readonly IQueryable<LinqToObjectsTwo> _queryableTwo = new List<LinqToObjectsTwo>
			{
				new LinqToObjectsTwo{LinqToObjectsTwoId = 1, LinqToObjectsOneParentId = 1, LinqToObjectsTwoField = "TwoOne"},
				new LinqToObjectsTwo{LinqToObjectsTwoId = 2, LinqToObjectsOneParentId = 1, LinqToObjectsTwoField = "TwoTwo"},
				new LinqToObjectsTwo{LinqToObjectsTwoId = 3, LinqToObjectsOneParentId = 2, LinqToObjectsTwoField = "TwoThree"},
				new LinqToObjectsTwo{LinqToObjectsTwoId = 4, LinqToObjectsOneParentId = 2, LinqToObjectsTwoField = "TwoFour"},
				new LinqToObjectsTwo{LinqToObjectsTwoId = 5, LinqToObjectsOneParentId = 3, LinqToObjectsTwoField = "TwoFive"},
				new LinqToObjectsTwo{LinqToObjectsTwoId = 6, LinqToObjectsOneParentId = 3, LinqToObjectsTwoField = "TwoSix"}
			}.AsQueryable();

		private readonly IQueryable<LinqToObjectsThree> _queryableThree = new List<LinqToObjectsThree>
			{
				new LinqToObjectsThree{LinqToObjectsThreeId = 1, LinqToObjectsOneParentId = 1, LinqToObjectsThreeField = "ThreeOne"},
				new LinqToObjectsThree{LinqToObjectsThreeId = 2, LinqToObjectsOneParentId = 1, LinqToObjectsThreeField = "ThreeTwo"},
				new LinqToObjectsThree{LinqToObjectsThreeId = 3, LinqToObjectsOneParentId = 4, LinqToObjectsThreeField = "ThreeThree"},
				new LinqToObjectsThree{LinqToObjectsThreeId = 4, LinqToObjectsOneParentId = 4, LinqToObjectsThreeField = "ThreeFour"},
				new LinqToObjectsThree{LinqToObjectsThreeId = 5, LinqToObjectsOneParentId = 5, LinqToObjectsThreeField = "ThreeFive"},
				new LinqToObjectsThree{LinqToObjectsThreeId = 6, LinqToObjectsOneParentId = 5, LinqToObjectsThreeField = "ThreeSix"}
			}.AsQueryable();

		public IQueryable<T> GetQueryableFor<T>()
		{
			IQueryable<T> result;

			if(typeof(T) == typeof(LinqToObjectsOne))
			{
				result = _queryableOne.Cast<T>();
			}
			else if(typeof(T) == typeof(LinqToObjectsTwo))
			{
				result = _queryableTwo.Cast<T>();
			}
			else if(typeof(T) == typeof(LinqToObjectsThree))
			{
				result = _queryableThree.Cast<T>();
			}
			else
			{
				throw new ArgumentException(String.Format("IQueryable for type \"{0}\" not found.", typeof (T)));
			}

			return result;
		}

		public IQueryable GetQueryable(Type type)
		{
			IQueryable result;

			if(type == typeof(LinqToObjectsOne))
			{
				result = _queryableOne;
			}
			else if(type == typeof(LinqToObjectsTwo))
			{
				result = _queryableTwo;
			}
			else if(type == typeof(LinqToObjectsThree))
			{
				result = _queryableThree;
			}
			else
			{
				throw new ArgumentException(String.Format("IQueryable for type \"{0}\" not found.", type));
			}

			return result;
		}
	}
}
