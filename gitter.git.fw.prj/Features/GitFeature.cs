﻿namespace gitter.Git
{
	using gitter.Git.AccessLayer;

	public abstract class GitFeature
	{
		private readonly string _name;

		protected GitFeature(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public abstract bool IsAvailableFor(IGitAccessor gitAccessor);
	}
}