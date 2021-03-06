﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScrewTurn.Wiki.PluginFramework;
using ScrewTurn.Wiki.Plugins.SqlCommon;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace ScrewTurn.Wiki.Plugins.FSProviders {

	/// <summary>
	/// Implements a command builder for SQL CE.
	/// </summary>
	public class SqlCECommandBuilder : ICommandBuilder {

		private static SqlCeConnection connection = null;

		/// <summary>
		/// Gets the table and column name prefix.
		/// </summary>
		public string ObjectNamePrefix {
			get { return "["; }
		}

		/// <summary>
		/// Gets the table and column name suffix.
		/// </summary>
		public string ObjectNameSuffix {
			get { return "]"; }
		}

		/// <summary>
		/// Gets the parameter name prefix.
		/// </summary>
		public string ParameterNamePrefix {
			get { return "@"; }
		}

		/// <summary>
		/// Gets the parameter name suffix.
		/// </summary>
		public string ParameterNameSuffix {
			get { return ""; }
		}

		/// <summary>
		/// Gets the parameter name placeholder.
		/// </summary>
		public string ParameterPlaceholder {
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a value indicating whether to use named parameters. If <c>false</c>,
		/// parameter placeholders will be equal to <see cref="ParameterPlaceholder"/>.
		/// </summary>
		public bool UseNamedParameters {
			get { return true; }
		}

		/// <summary>
		/// Gets the string to use in order to separate queries in a batch.
		/// </summary>
		public string BatchQuerySeparator {
			get { return "; "; }
		}

		/// <summary>
		/// Gets a new database connection, open.
		/// </summary>
		/// <param name="connString">The connection string.</param>
		/// <returns>The connection.</returns>
		public DbConnection GetConnection(string connString) {
			if(connection == null || connection.ConnectionString != connString) {
				connection = new SqlCeConnection(connString);
			}
			if(connection.State != System.Data.ConnectionState.Open) {
			    try
			    {
			        connection.Open();
			    }
			    catch (System.Data.SqlServerCe.SqlCeInvalidDatabaseFormatException)
			    {
                    // TODO: Delete in release
                    // Convert DB to CE v.4
			        SqlCeEngine engine = new SqlCeEngine(connString);
			        engine.Upgrade(connString);
			        connection = new SqlCeConnection(connString);
			        connection.Open();
			    }
			    catch (Exception)
			    {
			        throw;
			    }
			}

			return connection;
		}

		/// <summary>
		/// Gets a properly built database command, with the underlying connection already open.
		/// </summary>
		/// <param name="connString">The connection string.</param>
		/// <param name="preparedQuery">The prepared query.</param>
		/// <param name="parameters">The parameters, if any.</param>
		/// <returns>The command.</returns>
		public DbCommand GetCommand(string connString, string preparedQuery, List<Parameter> parameters) {
			return GetCommand(GetConnection(connString), preparedQuery, parameters);
		}

		/// <summary>
		/// Gets a properly built database command, re-using an open connection.
		/// </summary>
		/// <param name="connection">The open connection to use.</param>
		/// <param name="preparedQuery">The prepared query.</param>
		/// <param name="parameters">The parameters, if any.</param>
		/// <returns>The command.</returns>
		public DbCommand GetCommand(DbConnection connection, string preparedQuery, List<Parameter> parameters) {
			DbCommand cmd = connection.CreateCommand();
			cmd.CommandText = preparedQuery;

			foreach(Parameter param in parameters) {
				cmd.Parameters.Add(new SqlCeParameter("@" + param.Name, param.Value));
			}

			return cmd;
		}

		/// <summary>
		/// Gets a properly built database command, re-using an open connection and a begun transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="preparedQuery">The prepared query.</param>
		/// <param name="parameters">The parameters, if any.</param>
		/// <returns>The command.</returns>
		public DbCommand GetCommand(DbTransaction transaction, string preparedQuery, List<Parameter> parameters) {
			DbCommand cmd = transaction.Connection.CreateCommand();
			cmd.Transaction = transaction;
			cmd.CommandText = preparedQuery;

			foreach(Parameter param in parameters) {
				cmd.Parameters.Add(new SqlCeParameter("@" + param.Name, param.Value));
			}

			return cmd;
		}

	}

}
