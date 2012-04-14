﻿namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Text;
	using System.Diagnostics;

	using gitter.Framework;

	/// <summary>Represent an async execution process of git.exe.</summary>
	internal sealed class GitAsync : IDisposable
	{
		#region Data

		private readonly Process _process;
		private readonly StringBuilder _stdout;
		private readonly StringBuilder _stderr;

		#endregion

		#region Events

		public event EventHandler<DataReceivedEventArgs> OutputReceived;
		public event EventHandler<DataReceivedEventArgs> ErrorReceived;
		public event EventHandler Exited;

		#endregion

		#region .ctor

		public GitAsync(Process process)
		{
			if(process == null) throw new ArgumentNullException("process");

			_process = process;

			_process.OutputDataReceived += OnOutputDataReceived;
			_process.ErrorDataReceived += OnErrorDataReceived;
			_process.Exited += OnExited;

			_stdout = new StringBuilder();
			_stderr = new StringBuilder();
		}

		#endregion

		#region Event Handlers

		private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if(e.Data != null)
			{
				_stdout.Append(e.Data);
				_stdout.Append('\n');
			}
			var handler = OutputReceived;
			if(handler != null) handler(this, e);
		}

		private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if(e.Data != null)
			{
				_stderr.Append(e.Data);
				_stderr.Append('\n');
			}
			var handler = ErrorReceived;
			if(handler != null) handler(this, e);
		}

		private void OnExited(object sender, EventArgs e)
		{
			var handler = Exited;
			if(handler != null) handler(this, e);
		}

		#endregion

		#region Properties

		public string StdOut
		{
			get { return _stdout.ToString(); }
		}

		public string StdErr
		{
			get { return _stderr.ToString(); }
		}

		public bool HasExited
		{
			get { return _process.HasExited; }
		}

		public int ExitCode
		{
			get { return _process.ExitCode; }
		}

		#endregion

		#region Methods

		public void Start()
		{
			_process.Start();
			_process.BeginOutputReadLine();
			_process.BeginErrorReadLine();
		}

		public void Kill()
		{
			try
			{
				_process.Kill();
				//Utility.TerminateProcessTree(_process.Handle, _process.Id, -1);
			}
			catch
			{
			}
			finally
			{
				//_process.Refresh();
			}
		}

		public void WaitForExit()
		{
			_process.WaitForExit();
		}

		public bool WaitForExit(int milliseconds)
		{
			return _process.WaitForExit(milliseconds);
		}

		public void Dispose()
		{
			_process.Dispose();
		}

		/// <summary>Throw <see cref="GitException"/> if process exit code is non-zero.</summary>
		[DebuggerHidden]
		public void ThrowOnBadReturnCode()
		{
			if(_process.ExitCode != 0)
			{
				throw new GitException(_stderr.Length == 0 ? _stdout.ToString() : _stderr.ToString());
			}
		}

		#endregion
	}
}
