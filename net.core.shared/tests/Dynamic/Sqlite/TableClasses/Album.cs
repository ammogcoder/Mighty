﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mighty;
using Mighty.Dynamic.Tests;

namespace Mighty.Dynamic.Tests.Sqlite.TableClasses
{
	public class Album : MightyOrm
	{
		public Album()
			: this(includeSchema: false)
		{
		}


		public Album(bool includeSchema) :
			base(TestConstants.ReadWriteTestConnection, includeSchema ? "Album" : "Album", "AlbumId", string.Empty, "last_insert_rowid()")
		{
		}
	}
}
