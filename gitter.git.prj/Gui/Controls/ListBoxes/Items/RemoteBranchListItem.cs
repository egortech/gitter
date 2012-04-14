﻿namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>A <see cref="CustomListBoxItem"/> representing <see cref="RemoteBranch"/> object.</summary>
	public sealed class RemoteBranchListItem : ReferenceListItemBase<RemoteBranch>
	{
		private static readonly Bitmap ImgBranchRemote = CachedResources.Bitmaps["ImgBranchRemote"];

		#region .ctor

		/// <summary>Create <see cref="RemoteBranchListItem"/>.</summary>
		/// <param name="branch">Related <see cref="RemoteBranch"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="branch"/> == <c>null</c>.</exception>
		public RemoteBranchListItem(RemoteBranch branch)
			: base(branch)
		{
			if(branch == null) throw new ArgumentNullException("branch");
		}

		#endregion

		protected override Image Image
		{
			get { return ImgBranchRemote; }
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Name:
					var rli = Parent as RemoteListItem;
					return measureEventArgs.MeasureImageAndText(ImgBranchRemote,
						rli != null ? Data.Name.Substring(rli.Data.Name.Length + 1) : Data.Name);
				default:
					return base.OnMeasureSubItem(measureEventArgs);
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					var rli = Parent as RemoteListItem;
					paintEventArgs.PaintImageAndText(ImgBranchRemote,
						rli != null ? Data.Name.Substring(rli.Data.Name.Length + 1) : Data.Name);
					break;
				default:
					base.OnPaintSubItem(paintEventArgs);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var mnu = new BranchMenu(Data);
			Utility.MarkDropDownForAutoDispose(mnu);
			return mnu;
		}
	}
}
