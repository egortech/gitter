﻿namespace gitter.Framework.Controls
{
	using System;
	using System.Collections.Generic;

	using Resources = gitter.Framework.Properties.Resources;

	public class WebBrowserViewFactory : ViewFactoryBase
	{
		public static readonly new Guid Guid = new Guid("BF80569F-4544-4B0F-8C5B-213215E053AA");

		public WebBrowserViewFactory()
			: base(Guid, Resources.StrWebBrowser, Resources.ImgWebBrowser, true)
		{
			this.DefaultViewPosition = ViewPosition.SecondaryDocumentHost;
		}

		/// <summary>Create new view with specified parameters.</summary>
		/// <param name="environment">Application working environment.</param>
		/// <param name="parameters">Creation parameters.</param>
		/// <returns>Created view.</returns>
		protected override ViewBase CreateViewCore(IWorkingEnvironment environment, IDictionary<string, object> parameters)
		{
			return new WebBrowserView(Guid, environment, parameters);
		}
	}
}