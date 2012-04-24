﻿namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;
	using gitter.Framework.Options;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class RenameRemoteDialog : GitDialogBase, IExecutableDialog
	{
		private readonly Remote _remote;

		public RenameRemoteDialog(Remote remote)
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "remote"), "remote");
			_remote = remote;

			InitializeComponent();

			SetupReferenceNameInputBox(_txtNewName, ReferenceType.Remote);

			Text = Resources.StrRenameRemote;

			_lblOldName.Text = Resources.StrRemote.AddColon();
			_lblNewName.Text = Resources.StrNewName.AddColon();

			_txtOldName.Text = remote.Name;
			_txtNewName.Text = remote.Name;
			_txtNewName.SelectAll();

			GitterApplication.FontManager.InputFont.Apply(_txtNewName, _txtOldName);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrRename; }
		}

		public Remote Remote
		{
			get { return _remote; }
		}

		public string NewName
		{
			get { return _txtNewName.Text; }
			set { _txtNewName.Text = value; }
		}

		public bool Execute()
		{
			var repository = _remote.Repository;
			var oldName = _txtOldName.Text;
			var newName = _txtNewName.Text.Trim();
			if(oldName == newName) return true;
			if(newName.Length == 0)
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrNoRemoteNameSpecified,
					Resources.ErrRemoteNameCannotBeEmpty);
				return false;
			}
			if(repository.Remotes.Contains(newName))
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidRemoteName,
					Resources.ErrRemoteAlreadyExists);
				return false;
			}
			string errmsg;
			if(!Reference.ValidateName(newName, out errmsg))
			{
				NotificationService.NotifyInputError(
					_txtNewName,
					Resources.ErrInvalidRemoteName,
					errmsg);
				return false;
			}
			try
			{
				Cursor = Cursors.WaitCursor;
				_remote.Name = newName;
				Cursor = Cursors.Default;
			}
			catch(GitException exc)
			{
				Cursor = Cursors.Default;
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					string.Format(Resources.ErrFailedToRenameRemote, oldName),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			return true;
		}
	}
}