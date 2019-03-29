using System;
using System.Data.Common;

namespace Mighty.ConnectionProviders
{
	abstract public class ConnectionProvider
	{
		public DbProviderFactory ProviderFactoryInstance { get; protected set; }
		public Type DatabasePluginType { get; protected set; }
		public string ConnectionString { get; protected set; }

		// fluent API, must return itself at the end; should set all three public properties (may ignore connectionStringOrName input here if you wish,
		// in which case you would pass null as the connectionStringOrName value to the MightyOrm constructor)
		abstract public ConnectionProvider Init(string connectionStringOrName);
	}
}