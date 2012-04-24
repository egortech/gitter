﻿namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	public partial class ProgressForm : Form, IAsyncProgressMonitor
	{
		#region Data

		private bool _canCancel;
		private IAsyncResult _context;

		#endregion

		#region Events

		/// <summary>
		/// Monitor raises this event to stop monitored action execution.
		/// </summary>
		public event EventHandler Cancelled;
		/// <summary>
		/// Monitor raises this event when it is ready to receive Set() calls after Start() call.
		/// </summary>
		public event EventHandler Started;

		#endregion

		/// <summary>Create <see cref="ProgressForm"/>.</summary>
		public ProgressForm()
		{
			InitializeComponent();

			_btnCancel.Text = Resources.StrCancel;

			_lblAction.Text = string.Empty;
			_canCancel = true;

			if(!Application.RenderWithVisualStyles)
			{
				_pnlContainer.BackColor = SystemColors.Control;
				_pnlLine.BackColor = SystemColors.Control;

				int d = _btnCancel.Top - _pnlContainer.Bottom;

				_pnlContainer.Height += d;
				ClientSize = new Size(ClientSize.Width, ClientSize.Height - d);
			}
		}

		private void UpdateWin7ProgressBar()
		{
			var form = GitterApplication.MainForm;
			if(form != null && !form.IsDisposed)
			{
				if(_progressBar.Style == ProgressBarStyle.Marquee)
				{
					form.SetTaskbarProgressState(TbpFlag.Indeterminate);
				}
				else
				{
					form.SetTaskbarProgressState(TbpFlag.Normal);
					form.SetTaskbarProgressValue(
						(long)(_progressBar.Value - _progressBar.Minimum),
						(long)(_progressBar.Maximum - _progressBar.Minimum));
				}
			}
		}

		private void StopWin7ProgressBar()
		{
			var form = GitterApplication.MainForm;
			if(form != null && !form.IsDisposed)
			{
				form.SetTaskbarProgressState(TbpFlag.NoProgress);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			var handler = Started;
			if(handler != null) handler(this, EventArgs.Empty);
			UpdateWin7ProgressBar();
		}

		/// <summary>
		/// Starts monitor.
		/// </summary>
		/// <param name="parent">Reference to parent window which is related to executing action.</param>
		/// <param name="context">Execution context.</param>
		/// <param name="blocking">Block calling thread until ProcessCompleted() is called.</param>
		public void Start(IWin32Window parent, IAsyncResult context, bool blocking)
		{
			_context = context;
			if(blocking)
				ShowDialog(parent);
			else
				Show(parent);
		}

		/// <summary>
		/// Executing action.
		/// </summary>
		/// <value></value>
		public IAsyncResult CurrentContext
		{
			get { return _context; }
		}

		/// <summary>
		/// Async action name.
		/// </summary>
		/// <value></value>
		public string ActionName
		{
			get { return Text; }
			set
			{
				if(InvokeRequired)
				{
					Invoke(new Action<string>((text) => Text = text), value);
				}
				else
				{
					Text = value;
				}
			}
		}

		/// <summary>
		/// Determines if action can be cancelled.
		/// </summary>
		/// <value></value>
		public bool CanCancel
		{
			get { return _canCancel; }
			set
			{
				if(InvokeRequired)
				{
					Invoke(new Action<bool>(SetCanCancel), value);
				}
				else
				{
					SetCanCancel(value);
				}
			}
		}

		/// <summary>
		/// Sets the can cancel.
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		private void SetCanCancel(bool value)
		{
			if(_canCancel != value)
			{
				_btnCancel.Enabled = value;
				_canCancel = value;
			}
		}

		/// <summary>
		/// Sets the name of currently executed step.
		/// </summary>
		/// <param name="action">Step name.</param>
		public void SetAction(string action)
		{
			if(InvokeRequired)
			{
				Invoke(new Action<string>(SetAction), action);
			}
			else
			{
				_lblAction.Text = action;
			}
		}

		/// <summary>
		/// Sets progress range information.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public void SetProgressRange(int min, int max)
		{
			if(InvokeRequired)
			{
				Invoke(new Action<int, int>(SetProgressRange), min, max);
			}
			else
			{
				_progressBar.Style = ProgressBarStyle.Continuous;
				_progressBar.Minimum = min;
				_progressBar.Maximum = max;
				UpdateWin7ProgressBar();
			}
		}

		/// <summary>
		/// Sets progress range information and current step name.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		/// <param name="action">Step name.</param>
		public void SetProgressRange(int min, int max, string action)
		{
			if(InvokeRequired)
			{
				Invoke(new Action<int, int, string>(SetProgressRange), min, max, action);
			}
			else
			{
				_progressBar.Minimum = min;
				_progressBar.Maximum = max;
				_lblAction.Text = action;
				UpdateWin7ProgressBar();
			}
		}

		/// <summary>
		/// Sets current progress.
		/// </summary>
		/// <param name="val">Progress.</param>
		public void SetProgress(int val)
		{
			if(InvokeRequired)
			{
				Invoke(new Action<int>(SetProgress), val);
			}
			else
			{
				_progressBar.Value = val;
				UpdateWin7ProgressBar();
			}
		}

		/// <summary>
		/// Sets current progress and step name.
		/// </summary>
		/// <param name="val">Progress.</param>
		/// <param name="action">Step name.</param>
		public void SetProgress(int val, string action)
		{
			if(InvokeRequired)
			{
				Invoke(new Action<int, string>(SetProgress), val, action);
			}
			else
			{
				_progressBar.Value = val;
				_lblAction.Text = action;
				UpdateWin7ProgressBar();
			}
		}

		/// <summary>
		/// Sets progress as unknown.
		/// </summary>
		public void SetProgressIntermediate()
		{
			if(InvokeRequired)
			{
				Invoke(new Action(SetProgressIntermediate));
			}
			else
			{
				_progressBar.Style = ProgressBarStyle.Marquee;
				UpdateWin7ProgressBar();
			}
		}

		/// <summary>
		/// Notifies that action is completed or cancelled and monitor must be shut down.
		/// </summary>
		public void ProcessCompleted()
		{
			_context = null;
			if(InvokeRequired)
			{
				Invoke(new Action(ProcessCompleted));
			}
			else
			{
				StopWin7ProgressBar();
				Close();
			}
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			_btnCancel.Enabled = false;
			var handler = Cancelled;
			if(handler != null) handler(this, EventArgs.Empty);
		}
	}
}