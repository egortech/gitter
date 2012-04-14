﻿namespace gitter.Git.Gui.Controls
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public sealed class ReflogRecordMenu : ContextMenuStrip
	{
		public ReflogRecordMenu(ReflogRecord reflogRecord)
		{
			if(reflogRecord == null) throw new ArgumentNullException("reflogRecord");
			var revision = reflogRecord.Revision;

			Items.Add(GuiItemFactory.GetViewDiffItem<ToolStripMenuItem>(new RevisionChangesDiffSource(revision)));
			Items.Add(GuiItemFactory.GetViewTreeItem<ToolStripMenuItem>(revision));
			Items.Add(GuiItemFactory.GetSavePatchItem<ToolStripMenuItem>(revision));

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCheckoutRevisionItem<ToolStripMenuItem>(revision, "{0}"));
			Items.Add(GuiItemFactory.GetResetHeadHereItem<ToolStripMenuItem>(revision));
			Items.Add(GuiItemFactory.GetCherryPickItem<ToolStripMenuItem>(revision, "{0}"));

			Items.Add(new ToolStripSeparator()); // copy to clipboard section

			var item = new ToolStripMenuItem(Resources.StrCopyToClipboard);
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrHash, revision.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyHashToClipboardItem<ToolStripMenuItem>(Resources.StrTreeHash, revision.TreeHash));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrMessage, reflogRecord.Message));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrSubject, revision.Subject));
			if(!string.IsNullOrEmpty(revision.Body))
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrBody, revision.Body));
			if(revision.Committer != revision.Author)
			{
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitter, revision.Committer.Name));
				item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrCommitterEmail, revision.Committer.Email));
			}
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthor, revision.Author.Name));
			item.DropDownItems.Add(GuiItemFactory.GetCopyToClipboardItem<ToolStripMenuItem>(Resources.StrAuthorEmail, revision.Author.Email));
			Items.Add(item);

			Items.Add(new ToolStripSeparator());

			Items.Add(GuiItemFactory.GetCreateBranchItem<ToolStripMenuItem>(reflogRecord.Revision));
			Items.Add(GuiItemFactory.GetCreateTagItem<ToolStripMenuItem>(reflogRecord.Revision));
		}
	}
}
