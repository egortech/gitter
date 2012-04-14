﻿namespace gitter.Git
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using gitter.Git.AccessLayer.CLI;

	/// <summary>Represents patch.</summary>
	public sealed class Diff : IEnumerable<DiffFile>
	{
		#region Data

		private readonly DiffType _type;
		private readonly IList<DiffFile> _files;

		#endregion

		/// <summary>Create empty <see cref="Diff"/>.</summary>
		/// <param name="type">Diff type.</param>
		public Diff(DiffType type)
		{
			_type = type;
			_files = new List<DiffFile>();
		}

		/// <summary>Create <see cref="Diff"/>.</summary>
		/// <param name="type">Diff type.</param>
		/// <param name="files">List of file diffs.</param>
		public Diff(DiffType type, IList<DiffFile> files)
		{
			if(files == null) throw new ArgumentNullException("files");

			_type = type;
			_files = files;
		}

		public void Add(DiffFile diffFile)
		{
			if(diffFile == null) throw new ArgumentNullException("diffFile");

			_files.Add(diffFile);
		}

		public bool Remove(DiffFile diffFile)
		{
			if(diffFile == null) throw new ArgumentNullException("diffFile");

			return _files.Remove(diffFile);
		}

		public DiffType Type
		{
			get { return _type; }
		}

		/// <summary>Diff is empty.</summary>
		public bool IsEmpty
		{
			get { return _files.Count == 0; }
		}

		public DiffFile this[int index]
		{
			get { return _files[index]; }
		}

		public DiffFile this[string name]
		{
			get
			{
				foreach(var file in _files)
				{
					string fileName;
					if(file.Status == FileStatus.Removed)
					{
						fileName = file.SourceFile;
					}
					else
					{
						fileName = file.TargetFile;
					}
					if(fileName == name) return file;
				}
				return null;
			}
		}

		public int FileCount
		{
			get { return _files.Count; }
		}

		#region IEnumerable<DiffFile> Members

		public IEnumerator<DiffFile> GetEnumerator()
		{
			return _files.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _files.GetEnumerator();
		}

		#endregion

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach(var file in _files)
			{
				file.ToString(sb);
			}
			return sb.ToString();
		}
	}
}
