﻿namespace gitter.Redmine
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

	public abstract class RedmineObjectModification<T> : RedmineObjectDefinition<T>
		where T : RedmineObject
	{
		private readonly T _original;

		protected RedmineObjectModification(T original)
		{
			if(original == null) throw new ArgumentNullException("original");

			_original = original;

			ResetCore();
		}

		protected T Original
		{
			get { return _original; }
		}
	}
}
