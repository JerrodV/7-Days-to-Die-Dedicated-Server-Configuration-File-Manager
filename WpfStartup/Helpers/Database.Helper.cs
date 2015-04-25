using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;


namespace WpfStartup.Helpers
{

	/// <summary>
	/// A Static class to shorthand the process of making databasee calls.
	/// ConnectionString and DefaultConnection are availible to help create a new SQLCommand,
	/// and GetCommand  Returns an SQLCommand ready for execution into a reader, or other sql command.
	/// During creation, a private event handler is attached to the connections TransactionCompleted event. 
	/// When called to close the connection, so no followup is required.
	/// -This was designed to help facilitate a design where database calls will be built into object models.
	/// </summary>
	public static class Database
	{
		/// <summary>
		/// The default connection string. This answers to the connections string written into
		/// Properties.Settings.Default.Base_ConnectionString
		/// </summary>
		public static String ConnectionString
		{
            //This structure does not care how you get this. Just change this get to do whatever.
			get
			{
				return Properties.Settings.Default.Base_ConnectionString;
			}
		}

		/// <summary>
		/// Based on a predefined connection string, creates an SQL connection
		/// </summary>
		public static SqlConnection DefaultConnection
		{

			get
			{
				SqlConnection conn = new SqlConnection();
				try
				{
					conn = new SqlConnection(Helpers.Database.ConnectionString);					
					conn.Open();
				}
				catch (Exception ex)
				{
					Helpers.MainWindow.ShowModal(
						new Helpers.DefaultErrorMessage(
							"A problem occured attempting to open a connection to the database. "
							+ "See the error below for hints to the problem.", ex));
				}
				return conn;
			}
		}

		/// <summary>
		/// Returns an SqlCommand based on a procedure name and an optional genaric list of SqlParameters. 
		/// The Boolean usingReader should be set to indicate if the method should attach handlers to 
		/// close the connection once the command has executed.
		/// </summary>
		/// <param name="command">The SQL Server Stored Procedure name</param>
		/// <param name="usingReader">True will leave the connection open</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static SqlCommand GetCommand(String command, List<SqlParameter> parameters = null, Boolean maintainConnection = false)
		{
			SqlCommand cmd = new SqlCommand(command, DefaultConnection);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;

			if (!maintainConnection)
			{
				////This event fires when the command is complete
				cmd.StatementCompleted += CommandCompleted;
				////This event fires when an error is generated
				cmd.Connection.InfoMessage += SQlInfoMessageSent;
			}
			//This ensures that the connection will return a message if 
			//not successful.
			cmd.Connection.FireInfoMessageEventOnUserErrors = true;

			if (parameters != null)
			{
				foreach (SqlParameter s in parameters)
				{
					cmd.Parameters.Add(s);
				}
			}

			if (cmd.Connection.State != System.Data.ConnectionState.Open)
			{
				try
				{
					cmd.Connection.Open();
				}
				catch (Exception ex)
				{
					Helpers.MainWindow.ShowModal(
						new Helpers.DefaultErrorMessage(
							"A problem occured attempting to open a connection to the database. "
							+ "See the error below for hints to the problem.", ex));
					return null;
				}
			}
			return cmd;
		}

		/// <summary>
		/// Takes the given SqlCommand, closes  and disposes of its connection, then disposes of the command.
		/// </summary>
		/// <param name="command">SqlCommand to be cleaned and disposed of.</param>
		public static void CloseCommand(SqlCommand command)
		{
			command.Connection.Close();
			command.Connection.Dispose();
			command.Dispose();
		}

		/// <summary>
		/// Takes a reader and an SqlCommand as parameters, closing the connection and disposing of the items.
		/// </summary>
		/// <param name="dataReader">SqlDataReader to close and dispose of</param>
		/// <param name="command">SqlCommand: If closeConnection is true, the connection will be closed and disposed of, and the SqlCommand it's self disposed of.
		/// Otherwise it is left as it came in and only the reader is disposed of.</param>
		/// <param name="closeConnection"></param>
		public static void CloseReader(SqlDataReader dataReader, SqlCommand command = null, Boolean closeConnection = true)
		{
			//Just by passing the reader through to the method, it closes the reader, however...
			if (!dataReader.IsClosed)
			{ dataReader.Close(); }
			dataReader.Dispose();
			if (closeConnection && command != null)
			{
				if (command.Connection.State == System.Data.ConnectionState.Open)
				{ command.Connection.Close(); command.Connection.Dispose();}
				command.Dispose();
			}
		}

		//Note* If this is not firing try removing SET NOCOUNT ON from your stored procedure.
		//It should fire when the reader that is attached is closed, or a scalar call is complete.
		private static void CommandCompleted(object sender, System.Data.StatementCompletedEventArgs e)
		{			
			((SqlCommand)sender).Connection.Close();
		}

		//if a transaction is made that is not valid, CommandComplete does not fire.
		//This should kill any connection that has responded with an error.
		private static void SQlInfoMessageSent(object sender, SqlInfoMessageEventArgs e)
		{
			((SqlConnection)sender).Close();
			//TODO:Write in a way to report database errors
			Helpers.MainWindow.ShowModal(
			   new Helpers.DefaultErrorMessage("A problem occured attempting to open a connection to the database. See the error below for hints to the problem."
				   , new Exception("Error: \n" + e.Message + "\n" + "Source:\n" + e.Source)));
		}

		/// <summary>
		/// This is a very specific call to get data by primary key(int auto)
		/// For it to work, just name the keys on all the table to ID
		/// </summary>
		/// <param name="command">The stored procedure name</param>
		/// <param name="ID">The ID of the record to get</param>
		/// <returns></returns>
		public static SqlCommand GetCommand(String command, Int32 ID, String parameterName = "")
		{
			SqlCommand cmd = new SqlCommand(command, DefaultConnection);
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			string name = "@ID";
			if (parameterName != "") { name = parameterName; }
			cmd.Parameters.AddWithValue(name, ID);
			if (cmd.Connection.State != System.Data.ConnectionState.Open)
			{
				try
				{
					cmd.Connection.Open();
				}
				catch (Exception ex)
				{
					Helpers.MainWindow.ShowModal(
					   new Helpers.DefaultErrorMessage(
						   "A problem occured attempting to open a connection to the database. "
						   + "See the error below for hints to the problem.", ex));
					return null;
				}
			}
			return cmd;
		}

		/// <summary>
		/// Simply allows for a datareader to be returned having executed the SqlCommand.
		/// Allows for syntactical chaining.
		/// </summary>
		/// <param name="cmd">The command to execute on.</param>
		/// <returns>A SqlDataReader ready to be read</returns>
		public static SqlDataReader DataReader(SqlCommand cmd)
		{
			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			if (cmd.Connection.State != System.Data.ConnectionState.Open)
			{
				try
				{
					cmd.Connection.Open();
				}
				catch (Exception ex)
				{
					Helpers.MainWindow.ShowModal(
						new Helpers.DefaultErrorMessage(
							"A problem occured attempting to open a connection to the database. "
							+ "See the error below for hints to the problem.", ex));
					return null;
				}
			}
			
			return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
		}

		public static void CheckConnectionString()
		{			
			if (Properties.Settings.Default.Base_ConnectionString != null && Properties.Settings.Default.Base_ConnectionString != "")
			{/*Good*/}
			else
			{	
				Helpers.MainWindow.ShowModal(new GetConnectionString(), "");
			}
		}

        public static List<dynamic> GetDynamic(SqlCommand cmd)
        {
            List<dynamic> retval = new List<dynamic>();

            if (cmd.Connection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    cmd.Connection.Open();
                }
                catch (Exception ex)
                {
                    Helpers.MainWindow.ShowModal(
                        new Helpers.DefaultErrorMessage(
                            "A problem occured attempting to open a connection to the database. "
                            + "See the error below for hints to the problem.", ex));
                    return null;
                }
            }

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            { 
                while(dr.Read())
                {
                    int c = 0;
                    while(c < dr.VisibleFieldCount)
                    {
                        c++;
                    }
                }
            }
            return retval;
        }
	}
}
