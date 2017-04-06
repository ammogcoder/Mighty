using System;
using System.Data.Common;

namespace Mighty.DatabasePlugins
{
	internal class Oracle : DatabasePlugin
	{
#region Provider support
		// we must use new because there are no overrides on static methods, see e.g. http://stackoverflow.com/q/7839691
		new static internal string GetProviderFactoryClassName(string loweredProviderName)
		{
			switch (loweredProviderName)
			{
				case "oracle.manageddataaccess.client":
					return "Oracle.ManagedDataAccess.Client.OracleClientFactory";

				case "oracle.dataaccess.client":
					return "Oracle.DataAccess.Client.OracleClientFactory";

				default:
					return null;
			}
		}
#endregion

#region SQL
		override public string BuildPagingQuery(string columns, string tablesAndJoins, string orderBy, string where,
			int limit, int offset)
		{
			string CountQuery = BuildSelect("COUNT(*)", mighty.Unthingify("FROM", tablesAndJoins), where);

			// I think the basic SELECT in Oracle in Massive is technically wrong (you can never rely on the ordering of
			// an inner SELECT being preserved in an outer SELECT), and we need to use this SQL if we want to limit things.
			// (By the way, we probably don't, as we can just use the single result hint, can't we? And rely on the user
			// passing sensible SQL for Single requests. i.e. if they don't, it's not unreasonable if the SELECT takes
			// a long time!)
			//
			// 't' outer table name will not conflict with any use of 't' table name in inner SELECT
			//
			// the idea is to to call the column ROW_NUMBER() and then remove it from any results, if we are going to be
			// consistent across DBs - but maybe we don't need to be;
			//
			string PagingQuery =
				string.Format("SELECT t.*" + CRLF +
							  "FROM" + CRLF +
							  "(" + CRLF +
							  "		SELECT ROW_NUMBER() OVER ({0}) \"ROW_NUMBER()\", {1}" + CRLF +
							  "		FROM {2}" + CRLF +
							  "		WHERE {3}" + CRLF +
							  ") t" + CRLF +
							  "WHERE {4}\"ROW_NUMBER()\" < {5}" + CRLF +
							  "ORDER BY \"ROW_NUMBER()\";",
					mighty.Thingify("ORDER BY", orderBy),
					mighty.Unthingify("SELECT", columns),
					mighty.Unthingify("FROM", tablesAndJoins),
					mighty.Thingify("WHERE", where),
					offset > 0 ? string.Format("\"ROW_NUMBER()\" > {0} AND ", offset) : "",
					limit + 1
				);
			return CountQuery + CRLF + PagingQuery;
		}
		// SELECT t.*
		// FROM
		// (
		// 		SELECT ROW_NUMBER() OVER (ORDER BY t.Salary DESC, t.Employee_ID) "ROW_NUMBER()", t.*
		// 		FROM employees t
		// 		WHERE t.last_name LIKE '%i%'
		// ) t
		// WHERE "ROW_NUMBER()" > 10 AND "ROW_NUMBER()" < 21
		// ORDER BY "ROW_NUMBER()";
#endregion

#region Table info
		// owner is for owner/schema, will be null if none was specified
		// This really does vary per DB and can't be a standard virtual method which most things share.
		override public string BuildTableInfoQuery(string owner, string tableName)
		{
			return string.Format("SELECT * FROM USER_TAB_COLUMNS WHERE TABLE_NAME = {0}{1}",
				tableName,
				owner == null ? "": string.Format(" AND OWNER = {1}", owner));
		}
#endregion

#region Prefix/deprefix parameters
		override public string PrefixParameterName(string rawName, DbCommand cmd = null)
		{
			return (cmd != null) ? rawName : (":" + rawName);
		}
#endregion

#region DbParameter
		override public void SetValue(DbParameter p, object value)
		{
			// Oracle exceptions on Guid parameter - set it via string
			if (value is Guid)
			{
				p.Value = value.ToString();
				p.Size = 36;
				return;
			}
			base.SetValue(p, value);
		}

		override public bool SetCursor(DbParameter p, object value)
		{
			p.SetRuntimeEnumProperty("OracleDbType", "RefCursor");
			p.Value = value;
			return true;
		}

		override public bool IsCursor(DbParameter p)
		{
			return p.GetRuntimeEnumProperty("OracleDbType") == "RefCursor";
		}
#endregion
	}
}