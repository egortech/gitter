﻿namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	static class GitConfigHelper
	{
		private static void InsertConfigFileSpecifier(IList<CommandArgument> args, BaseConfigParameters parameters)
		{
			switch(parameters.ConfigFile)
			{
				case ConfigFile.Repository:
				case ConfigFile.Other:
					if(parameters.FileName != null)
						args.Add(ConfigCommand.File(parameters.FileName));
					break;
				case ConfigFile.System:
					args.Add(ConfigCommand.System());
					break;
				case ConfigFile.User:
					args.Add(ConfigCommand.Global());
					break;
			}
		}

		[DebuggerHidden]
		private static void HandleConfigResults(GitOutput output)
		{
			switch(output.ExitCode)
			{
				case 0:
					return;
				case 1:
					throw new InvalidConfigFileException();
				case 2:
					throw new CannotWriteConfigFileException();
				case 3:
					throw new NoSectionProvidedException();
				case 4:
					throw new InvalidSectionOrKeyException();
				case 5:
					throw new ConfigParameterDoesNotExistException();
				default:
					output.Throw();
					break;
			}
		}

		/// <summary>Query config parameter.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameterParameters"/>.</param>
		/// <returns><see cref="ConfigParameterData"/> for requested parameter or null if parameter does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static ConfigParameterData QueryConfigParameter(ICommandExecutor executor, QueryConfigParameterParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(2);
			InsertConfigFileSpecifier(args, parameters);
			args.Add(new CommandArgument(parameters.ParameterName));

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			if(output.ExitCode == 0)
			{
				var value = output.Output.TrimEnd('\n');
				return new ConfigParameterData(parameters.ParameterName, value, parameters.ConfigFile, parameters.FileName);
			}
			else
			{
				return null;
			}
		}

		/// <summary>Query configuration parameter list.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameters"/>.</param>
		/// <returns>List of requested parameters.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static IList<ConfigParameterData> QueryConfig(ICommandExecutor executor, QueryConfigParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(3);

			args.Add(ConfigCommand.NullTerminate());
			args.Add(ConfigCommand.List());
			InsertConfigFileSpecifier(args, parameters);

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			if(output.ExitCode != 0 && parameters.ConfigFile != ConfigFile.Other)
				return new ConfigParameterData[0];
			HandleConfigResults(output);

			var res = new List<ConfigParameterData>();
			var parser = new GitParser(output.Output);
			while(!parser.IsAtEndOfString)
			{
				var name = parser.ReadStringUpTo(parser.FindNewLineOrEndOfString(), 1);
				var value = parser.ReadStringUpTo(parser.FindNullOrEndOfString(), 1);
				if(parameters.ConfigFile != ConfigFile.Other)
					res.Add(new ConfigParameterData(name, value, parameters.ConfigFile));
				else
					res.Add(new ConfigParameterData(name, value, parameters.FileName));
			}
			return res;
		}

		/// <summary>Add config value.</summary>
		/// <param name="parameters"><see cref="AddConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void AddConfigValue(ICommandExecutor executor, AddConfigValueParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(4);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.Add());
			args.Add(new CommandArgument(parameters.ParameterName));
			args.Add(new CommandArgument(parameters.ParameterValue.SurroundWith("\"", "\"")));

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			HandleConfigResults(output);
		}

		/// <summary>Set config value.</summary>
		/// <param name="parameters"><see cref="SetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void SetConfigValue(ICommandExecutor executor, SetConfigValueParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(3);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(new CommandArgument(parameters.ParameterName));
			args.Add(new CommandArgument(parameters.ParameterValue.SurroundWith("\"", "\"")));

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			GitConfigHelper.HandleConfigResults(output);
		}

		/// <summary>Unset config parameter value.</summary>
		/// <param name="parameters"><see cref="UnsetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void UnsetConfigValue(ICommandExecutor executor, UnsetConfigValueParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(3);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.Unset());
			args.Add(new CommandArgument(parameters.ParameterName));

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			GitConfigHelper.HandleConfigResults(output);
		}

		/// <summary>Rename configuration section.</summary>
		/// <param name="parameters"><see cref="RenameConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void RenameConfigSection(ICommandExecutor executor, RenameConfigSectionParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(2);

			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.RenameSection(parameters.OldName, parameters.NewName));

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			GitConfigHelper.HandleConfigResults(output);
		}

		/// <summary>Delete configuration section.</summary>
		/// <param name="parameters"><see cref="DeleteConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public static void DeleteConfigSection(ICommandExecutor executor, DeleteConfigSectionParameters parameters)
		{
			if(parameters == null) throw new ArgumentNullException("parameters");

			var args = new List<CommandArgument>(2);
			GitConfigHelper.InsertConfigFileSpecifier(args, parameters);
			args.Add(ConfigCommand.RemoveSection(parameters.SectionName));

			var cmd = new ConfigCommand(args);
			var output = executor.ExecCommand(cmd);
			GitConfigHelper.HandleConfigResults(output);
		}
	}
}
