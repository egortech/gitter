﻿namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	internal partial class ReflogView : GitViewBase, ISearchableView<ReflogSearchOptions>
	{
		#region Data

		private readonly ReflogToolbar _toolbar;
		private ReflogSearchToolBar<ReflogView> _searchToolbar;
		private ISearch<ReflogSearchOptions> _search;
		private Reflog _reflog;
		private Reference _reference;

		#endregion

		#region .ctor

		public ReflogView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.ReflogViewGuid, gui, parameters)
		{
			InitializeComponent();

			_lstReflog.SelectionChanged += OnReflogSelectionChanged;
			_lstReflog.ItemActivated += OnReflogItemActivated;
			_lstReflog.PreviewKeyDown += OnKeyDown;

			_search = new ReflogSearch<ReflogSearchOptions>(_lstReflog);

			ApplyParameters(parameters);
			AddTopToolStrip(_toolbar = new ReflogToolbar(this));
		}

		#endregion

		#region Properties

		public override bool IsDocument
		{
			get { return true; }
		}

		public override Image Image
		{
			get
			{
				if(Reflog != null && Reflog.Reference.Type == ReferenceType.RemoteBranch)
				{
					return CachedResources.Bitmaps["ImgViewReflogRemote"];
				}
				else
				{
					return CachedResources.Bitmaps["ImgViewReflog"];
				}
			}
		}

		public Reflog Reflog
		{
			get { return _reflog; }
			private set
			{
				if(_reflog != value)
				{
					_reflog = value;
					_lstReflog.Load(value);
					Reference = value != null ? value.Reference : null;
				}
			}
		}

		public Reference Reference
		{
			get { return _reference; }
			private set
			{
				if(_reference != value)
				{
					if(_reference != null)
					{
						var branch = _reference as Branch;
						if(branch != null)
						{
							branch.Renamed -= OnBranchRenamed;
						}
					}
					_reference = value;
					if(_reference != null)
					{
						var branch = _reference as Branch;
						if(branch != null)
						{
							branch.Renamed += OnBranchRenamed;
						}
					}
					UpdateText();
				}
			}
		}

		public ISearch<ReflogSearchOptions> Search
		{
			get { return _search; }
		}

		public bool SearchToolBarVisible
		{
			get { return _searchToolbar != null && _searchToolbar.Visible; }
			set
			{
				if(value)
				{
					ShowSearchToolBar();
				}
				else
				{
					HideSearchToolBar();
				}
			}
		}

		#endregion

		private void ShowSelectedCommitDetails()
		{
			switch(_lstReflog.SelectedItems.Count)
			{
				case 1:
					{
						var item = _lstReflog.SelectedItems[0] as ReflogRecordListItem;
						if(item != null)
						{
							ShowContextualDiffView(item.DataContext.Revision.GetDiffSource());
						}
					}
					break;
			}
		}

		public override void RefreshContent()
		{
			if(Reflog != null)
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					Reflog.Refresh();
				}
			}
		}

		private static Reflog TryGetReflog(IDictionary<string, object> parameters)
		{
			object reflog;
			if(parameters.TryGetValue("reflog", out reflog))
			{
				return reflog as Reflog;
			}
			else
			{
				return null;
			}
		}

		public override void ApplyParameters(IDictionary<string, object> parameters)
		{
			base.ApplyParameters(parameters);

			Reflog = TryGetReflog(parameters);
		}

		private void UpdateText()
		{
			if(Reference != null)
			{
				Text = Resources.StrReflog + ": " + Reference.Name;
			}
			else
			{
				Text = Resources.StrReflog;
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void ShowSearchToolBar()
		{
			if(_searchToolbar == null)
			{
				AddBottomToolStrip(_searchToolbar = new ReflogSearchToolBar<ReflogView>(this));
			}
			_searchToolbar.FocusSearchTextBox();
		}

		private void HideSearchToolBar()
		{
			if(_searchToolbar != null)
			{
				RemoveToolStrip(_searchToolbar);
				_searchToolbar.Dispose();
				_searchToolbar = null;
			}
		}

		#region Event Handlers

		private void OnReflogSelectionChanged(object sender, EventArgs e)
		{
			ShowSelectedCommitDetails();
		}

		private void OnReflogItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as ReflogRecordListItem;
			if(item != null)
			{
				var reflogRecord = item.DataContext;
				ShowDiffView(reflogRecord.Revision.GetDiffSource());
			}
		}

		private void OnBranchRenamed(object sender, NameChangeEventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(UpdateText));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					UpdateText();
				}
			}
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F:
					if(e.Modifiers == Keys.Control)
					{
						ShowSearchToolBar();
						e.IsInputKey = true;
					}
					break;
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		#endregion
	}
}
