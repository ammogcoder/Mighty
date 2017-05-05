﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mighty;
using Mighty.Tests;

namespace Mighty.Tests.Sqlite.TableClasses
{
	public class Playlist : MightyORM
	{
		public Playlist()
			: this(includeSchema: false)
		{
		}


		public Playlist(bool includeSchema) :
			base(TestConstants.ReadWriteTestConnection, includeSchema ? "Playlist" : "Playlist", "PlaylistId", string.Empty, "last_insert_rowid()")
		{
		}
	}
}