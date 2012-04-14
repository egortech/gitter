﻿namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml;

	using gitter.Framework;
	using gitter.Framework.Options;
	using gitter.Framework.Configuration;

	using gitter.Git.Gui;
	using gitter.Git.Gui.Dialogs;
	using gitter.Git.Integration;
	using gitter.Git.AccessLayer;
	using gitter.Git.AccessLayer.CLI;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git <see cref="Repository"/> provider.</summary>
	public sealed class RepositoryProvider : IRepositoryProvider
	{
		#region Static Data

		private static readonly Version _minVersion = new Version(1,7,0,2);
		private static Version _gitVersion;
		private static bool _autodetectGitExePath;
		private static string _gitExePath;
		private static readonly IGitAccessor _git = new GlobalCLI();
		private static readonly IntegrationFeatures _integrationFeatures = new IntegrationFeatures();
		private static IWorkingEnvironment _environment;

		internal static IWorkingEnvironment Environment
		{
			get { return _environment; }
		}

		#endregion

		static RepositoryProvider()
		{
			_autodetectGitExePath = true;
		}

		/// <summary>Create <see cref="RepositoryProvider"/>.</summary>
		public RepositoryProvider()
		{
		}

		public static Version GitVersion
		{
			get
			{
				if(_gitVersion == null)
				{
					try
					{
						_gitVersion = _git.QueryVersion();
					}
					catch { }
				}
				return _gitVersion;
			}
		}

		public static bool LogCLICalls
		{
			get;
			set;
		}

		public static Version MinimumRequiredGitVersion
		{
			get { return _minVersion; }
		}

		internal static IGitAccessor Git
		{
			get { return _git; }
		}

		public static IntegrationFeatures Integration
		{
			get { return _integrationFeatures; }
		}

		public static bool AutodetectGitExePath
		{
			get { return _autodetectGitExePath; }
			set { _autodetectGitExePath = value; }
		}

		public static string ManualGitExePath
		{
			get { return _gitExePath; }
			set { _gitExePath = value; }
		}

		public string Name
		{
			get { return "git"; }
		}

		public bool LoadFor(IWorkingEnvironment environment, Section section)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			if(section != null)
			{
				var msysgitNode = section.TryGetSection("MSysGit");
				if(msysgitNode != null)
				{
					AutodetectGitExePath = msysgitNode.GetValue("Autodetect", true);
					ManualGitExePath = msysgitNode.GetValue("Path", string.Empty);
					LogCLICalls = msysgitNode.GetValue("LogCLICalls", false);
					GitProcess.EnableAnsiCodepageFallback = msysgitNode.GetValue("EnableAnsiCodepageFallback", false);
				}
			}
			GitProcess.UpdateGitExePath();
			try
			{
				_gitVersion = _git.QueryVersion();
			}
			catch
			{
				_gitVersion = null;
			}
			if(_gitVersion == null || _gitVersion < _minVersion)
			{
				using(var dlg = new VersionCheckDialog(_minVersion, _gitVersion))
				{
					dlg.Run(environment.MainForm);
					_gitVersion = dlg.InstalledVersion;
					if(_gitVersion == null || _gitVersion < _minVersion)
						return false;
				}
			}
			if(section != null)
			{
				var integrationNode = section.TryGetSection("Integration");
				if(integrationNode != null)
					_integrationFeatures.LoadFrom(integrationNode);
			}
			GlobalOptions.RegisterPropertyPage(new PropertyPageDescription(
				GitOptionsPage.Guid,
				Resources.StrGit,
				null, /*Gui.Bitmaps["ImgGit"],*/
				PropertyPageDescription.RootGroupGuid,
				() => new GitOptionsPage()));
			GlobalOptions.RegisterPropertyPage(new PropertyPageDescription(
				IntegrationOptionsPage.Guid,
				Resources.StrIntegration,
				null,
				GitOptionsPage.Guid,
				() => new IntegrationOptionsPage()));
			GlobalOptions.RegisterPropertyPage(new PropertyPageDescription(
				ConfigurationPage.Guid,
				Resources.StrConfig,
				null,
				GitOptionsPage.Guid,
				() => new ConfigurationPage()));
			_environment = environment;
			return true;
		}

		public void SaveTo(Section section)
		{
			var msysgitNode = section.GetCreateSection("MSysGit");
			msysgitNode.SetValue("Autodetect", AutodetectGitExePath);
			msysgitNode.SetValue("Path", ManualGitExePath);
			msysgitNode.SetValue("LogCLICalls", LogCLICalls);
			msysgitNode.SetValue("EnableAnsiCodepageFallback", GitProcess.EnableAnsiCodepageFallback);

			var integrationNode = section.GetCreateSection("Integration");
			_integrationFeatures.SaveTo(integrationNode);
		}

		public bool IsValidFor(string workingDirectory)
		{
			return _git.IsValidRepository(workingDirectory);
		}

		public IRepository OpenRepository(string workingDirectory)
		{
			return new Repository(workingDirectory);
		}

		public IAsyncFunc<IRepository> OpenRepositoryAsync(string workingDirectory)
		{
			return Repository.LoadAsync(workingDirectory);
		}

		public void OnRepositoryLoaded(IWorkingEnvironment environment, IRepository repository)
		{
			if(environment == null) throw new ArgumentNullException("environment");
			if(repository == null) throw new ArgumentNullException("repository");
			var gitRepository = repository as Repository;
			if(gitRepository == null) throw new ArgumentException("repository");

			if(!gitRepository.Configuration.Exists(GitConstants.UserNameParameter))
			{
				using(var dlg = new UserIdentificationDialog(gitRepository))
				{
					dlg.Run(environment.MainForm);
				}
			}
		}

		public void CloseRepository(IRepository repository)
		{
			var repo = (Repository)repository;
			repo.Monitor.Shutdown();
			try
			{
				var cfgFileName = Path.Combine(repo.GitDirectory, "gitter-config");
				using(var fs = new FileStream(cfgFileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					repo.ConfigurationManager.Save(new XmlAdapter(fs));
				}
			}
			catch
			{
			}
		}

		public IRepositoryGuiProvider CreateGuiProvider(IRepository repository)
		{
			return new GuiProvider((Repository)repository);
		}

		public void InitRepository(IWin32Window parent)
		{
			using(var dlg = new InitDialog())
			{
				dlg.Run(parent);
			}
		}

		public void CloneRepository(IWin32Window parent)
		{
			using(var dlg = new CloneDialog())
			{
				dlg.Run(parent);
			}
		}

		public static DialogResult RunInitDialog()
		{
			return RunInitDialog(_environment);
		}

		public static DialogResult RunInitDialog(IWorkingEnvironment environment)
		{
			DialogResult res;
			string path = "";
			using(var dlg = new InitDialog()
				{
					RepositoryPath = environment.RecentRepositoryPath,
				})
			{
				res = dlg.Run(environment.MainForm);
				if(res == DialogResult.OK)
					path = Path.GetFullPath(dlg.RepositoryPath.Trim());
			}
			if(res == DialogResult.OK)
			{
				environment.OpenRepository(path);
			}
			return res;
		}

		public static DialogResult RunCloneDialog()
		{
			return RunCloneDialog(_environment);
		}

		public static DialogResult RunCloneDialog(IWorkingEnvironment environment)
		{
			DialogResult res;
			string path = "";
			using(var dlg = new CloneDialog()
				{
					RepositoryPath = environment.RecentRepositoryPath,
				})
			{
				res = dlg.Run(environment.MainForm);
				if(res == DialogResult.OK)
					path = Path.GetFullPath(dlg.TargetPath.Trim());
			}
			if(res == DialogResult.OK)
			{
				environment.OpenRepository(path);
			}
			return res;
		}

		public IEnumerable<StaticRepositoryAction> GetStaticActions()
		{
			return new[]
			{
				new StaticRepositoryAction("init", Resources.StrInit.AddEllipsis(), CachedResources.Bitmaps["ImgInit"],
					env => RunInitDialog(env)),
				new StaticRepositoryAction("clone", Resources.StrClone.AddEllipsis(), CachedResources.Bitmaps["ImgClone"],
					env => RunCloneDialog(env)),
			};
		}
	}
}
