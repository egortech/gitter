﻿namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Tag description.</summary>
	public sealed class TagData : IObjectData<Tag>, INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _sha1;
		private readonly TagType _tagType;

		#endregion

		#region .ctor

		public TagData(string name, string sha1, TagType tagType)
		{
			if(name == null) throw new ArgumentNullException("name");
			if(name.Length == 0) throw new ArgumentException("name");
			if(sha1 == null) throw new ArgumentNullException("sha1");
			if(sha1.Length != 40) throw new ArgumentException("sha1");
			_name = name;
			_sha1 = sha1;
			_tagType = tagType;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public string SHA1
		{
			get { return _sha1; }
		}

		public TagType TagType
		{
			get { return _tagType; }
		}

		#endregion

		public void Update(Tag tag)
		{
			if(tag.Revision.Name != _sha1)
			{
				var repo = tag.Repository;
				Revision revision;
				lock(repo.Revisions.SyncRoot)
				{
					revision = repo.Revisions.GetOrCreateRevision(_sha1);
				}
				tag.Pointer = revision;
			}
			tag.TagType = _tagType;
		}

		public Tag Construct(IRepository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");
			var repo = (Repository)repository;
			Revision revision;
			lock(repo.Revisions.SyncRoot)
			{
				revision = repo.Revisions.GetOrCreateRevision(_sha1);
			}
			return new Tag(repo, _name, revision, _tagType);
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
