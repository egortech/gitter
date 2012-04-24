﻿namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework.Controls;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	partial class RemotesView : GitViewBase
	{
		private RemotesToolbar _toolbar;

		public RemotesView(IDictionary<string, object> parameters, GuiProvider gui)
			: base(Guids.RemotesViewGuid, gui, parameters)
		{
			InitializeComponent();

			Text = Resources.StrRemotes;

			_lstRemotes.Text = Resources.StrNoRemotes;

			_lstRemotes.ItemActivated += OnItemActivated;
			_lstRemotes.PreviewKeyDown += OnKeyDown;

			AddTopToolStrip(_toolbar = new RemotesToolbar(this));
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var remote = ((CustomListBoxItem<Remote>)e.Item).Data;
			using(var dlg = new RemoteReferencesDialog(remote))
			{
				dlg.Run(this);
			}
		}

		protected override void AttachToRepository(Repository repository)
		{
			_lstRemotes.LoadData(repository);
		}

		protected override void DetachFromRepository(Repository repository)
		{
			_lstRemotes.LoadData(null);
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgRemote"]; }
		}

		public override void RefreshContent()
		{
			if(InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(RefreshContent));
			}
			else
			{
				if(Repository != null)
				{
					Cursor = Cursors.WaitCursor;
					Repository.Remotes.Refresh();
					Cursor = Cursors.Default;
				}
			}
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			OnKeyDown(this, e);
			base.OnPreviewKeyDown(e);
		}

		private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.F5:
					RefreshContent();
					e.IsInputKey = true;
					break;
			}
		}

		protected override void SaveMoreViewTo(Section section)
		{
			base.SaveMoreViewTo(section);
			var listNode = section.GetCreateSection("RemoteList");
			_lstRemotes.SaveViewTo(listNode);
		}

		protected override void LoadMoreViewFrom(Section section)
		{
			base.LoadMoreViewFrom(section);
			var listNode = section.TryGetSection("RemoteList");
			if(listNode != null)
				_lstRemotes.LoadViewFrom(listNode);
		}
	}
}