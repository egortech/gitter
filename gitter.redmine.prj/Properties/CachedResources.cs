﻿namespace gitter.Redmine
{
	using System;

	using gitter.Framework;

	using Resources = gitter.Redmine.Properties.Resources;

	static class CachedResources
	{
		private static readonly CachedBitmapResources _bitmaps;

		static CachedResources()
		{
			_bitmaps = new CachedBitmapResources(Resources.ResourceManager);
		}

		public static CachedBitmapResources Bitmaps
		{
			get { return _bitmaps; }
		}
	}
}
