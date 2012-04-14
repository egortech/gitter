﻿namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Menu for <see cref="RemoteTag"/> object.</summary>
	[ToolboxItem(false)]
	public sealed class RemoteTagMenu : ContextMenuStrip
	{
		private readonly RemoteRepositoryTag _remoteTag;

		/// <summary>Create <see cref="RemoteBranchMenu"/>.</summary>
		/// <param name="remoteTag">Remote branch, for which menu is generated.</param>
		public RemoteTagMenu(RemoteRepositoryTag remoteTag)
		{
			if(remoteTag == null) throw new ArgumentNullException("remoteTag");
			if(remoteTag.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "RemoteTag"), "remoteTag");
			_remoteTag = remoteTag;

			Items.Add(GuiItemFactory.GetRemoveRemoteTagItem<ToolStripMenuItem>(_remoteTag, "{0}"));
		}

		/// <summary>Remote tag, for which menu is generated.</summary>
		public RemoteRepositoryTag RemoteTag
		{
			get { return _remoteTag; }
		}
	}
}
