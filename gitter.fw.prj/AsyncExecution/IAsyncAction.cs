﻿namespace gitter.Framework
{
	using System;
	using System.Windows.Forms;

	/// <summary>Represents a method which can be executed sync/async with progress monitoring.</summary>
	public interface IAsyncAction
	{
		/// <summary>Invoke synchronously without monitoring.</summary>
		void Invoke();

		/// <summary>Invoke synchronously.</summary>
		/// <typeparam name="TMonitor">Monitor type.</typeparam>
		/// <param name="parent">Parent window reference.</param>
		void Invoke<TMonitor>(IWin32Window parent)
			where TMonitor : IAsyncProgressMonitor, new();

		/// <summary>Invoke synchronously.</summary>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="monitor">Progress monitor.</param>
		void Invoke(IWin32Window parent, IAsyncProgressMonitor monitor);

		/// <summary>Begin async execution.</summary>
		/// <typeparam name="TMonitor">Monitor type.</typeparam>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="callback">Callback to call on completion.</param>
		/// <param name="asyncState">Associated object reference.</param>
		/// <returns><see cref="IAsyncResult"/>.</returns>
		IAsyncResult BeginInvoke<TMonitor>(IWin32Window parent, AsyncCallback callback, object asyncState)
			where TMonitor : IAsyncProgressMonitor, new();

		/// <summary>Begin async execution.</summary>
		/// <param name="parent">Parent window reference.</param>
		/// <param name="monitor">Progress monitor.</param>
		/// <param name="callback">Callback to call on completion.</param>
		/// <param name="asyncState">Associated object reference.</param>
		/// <returns><see cref="IAsyncResult"/>.</returns>
		IAsyncResult BeginInvoke(IWin32Window parent, IAsyncProgressMonitor monitor, AsyncCallback callback, object asyncState);

		/// <summary>Complete async execution.</summary>
		/// <param name="asyncResult">
		/// <see cref="IAsyncResult"/>, returned by previous call to <see cref="BeginInvoke"/> or
		/// <see cref="BeginInvoke&lt;TMonitor&gt;"/>.
		/// </param>
		void EndInvoke(IAsyncResult asyncResult);
	}
}