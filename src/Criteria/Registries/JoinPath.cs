using System;
using System.Linq.Expressions;

using Criteria.Joins;

namespace Criteria.Registries
{
	public abstract class JoinPath
	{
		public bool IsOneToMany { get; private set; }
		public Type Start { get; private set; }
		public Type End { get; private set; }

		protected LambdaExpression OuterKey { get; private set; }
		protected LambdaExpression InnerKey { get; private set; }

		public static JoinPath Create<TOuter, TInner, TKey>(Expression<Func<TOuter, TKey>> outerKey, Expression<Func<TInner, TKey>> innerKey, bool isOneToManyJoin = false)
		{
			var dependency = new JoinPath<TOuter, TInner, TKey>
			{
				IsOneToMany = isOneToManyJoin,
				Start = typeof(TOuter),
				End = typeof(TInner),
				OuterKey = outerKey,
				InnerKey = innerKey
			};

			return dependency;
		}

		public abstract JoinPart ApplyJoin(JoinPart joinPart);

		public abstract JoinPart StartJoin(JoinConfiguration joinConfiguration);

		public bool Equals(JoinPath other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Equals(other.End, End) && Equals(other.Start, Start) && Equals(other.IsOneToMany, IsOneToMany);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var type = obj.GetType();
			var isConvertibleToJoinPath = typeof (JoinPath).IsAssignableFrom(type);

			return isConvertibleToJoinPath && Equals((JoinPath)obj);
		}

		public override int GetHashCode()
		{
			return String.Format("End:{0};Start:{1};", End, Start).GetHashCode();
		}
	}

	public class JoinPath<TOuter, TInner, TKey> : JoinPath
	{
		private Expression<Func<TInner, TKey>> TypedInnerKey { get { return (Expression<Func<TInner, TKey>>) InnerKey; } }

		private Expression<Func<TOuter, TKey>> TypedOuterKey { get { return (Expression<Func<TOuter, TKey>>) OuterKey; } }

		public override JoinPart ApplyJoin(JoinPart joinPart)
		{
			return joinPart.Join<TOuter>().To<TInner>().On(TypedOuterKey, TypedInnerKey);
		}

		public override JoinPart StartJoin(JoinConfiguration joinConfiguration)
		{
			return Join.Using(joinConfiguration)
				.StartWith<TOuter>()
				.Join<TOuter>().To<TInner>().On(TypedOuterKey, TypedInnerKey);
		}
	}
}
