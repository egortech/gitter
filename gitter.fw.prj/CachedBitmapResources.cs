﻿namespace gitter.Framework
{
	using System;
	using System.Resources;
	using System.Collections.Generic;
	using System.Drawing;

	/// <summary>Provides cached bitmap resources.</summary>
	public sealed class CachedBitmapResources
	{
		private readonly Dictionary<string, Bitmap> _cache = new Dictionary<string, Bitmap>();
		private readonly ResourceManager _manager;

		public CachedBitmapResources(ResourceManager manager)
		{
			if(manager == null) throw new ArgumentNullException("manager");
			_manager = manager;
		}

		public Bitmap this[string name]
		{
			get
			{
				Bitmap bmp;
				if(!_cache.TryGetValue(name, out bmp))
				{
					bmp = (Bitmap)_manager.GetObject(name);
					_cache.Add(name, bmp);
				}
				return bmp;
			}
		}

		public Bitmap CombineBitmaps(string resBackground, string resOverlay)
		{
			var res = new Bitmap(this[resBackground]);
			var ovl = this[resOverlay];
			using(var gx = Graphics.FromImage(res))
			{
				gx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				gx.DrawImage(ovl, new Rectangle(0, 0, ovl.Width, ovl.Height), new Rectangle(0, 0, ovl.Width, ovl.Height), GraphicsUnit.Pixel);
			}
			return res;
		}
	}
}