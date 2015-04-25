using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using WpfStartup.Helpers;

namespace WpfStartup.Models
{
	/// <summary>
	/// When creating classes the following design is recommended for use with the database helper classes.
	/// Note that the ID has no Get/Set. This will keep it from showing in datagrids when attaching 
	/// observable collections of the object as the data source. Also, the way to go about handling field names on
	/// datagrids, you will need to use the DataGridAutoGeneratingColumn columns event. Because the 
	/// datagrid will always generate columns in the order they are defined in the class, you can add 
	/// their friendly name to a list availible to the DataGridAutoGeneratingColumn event handler in 
	/// the same order. Then, just extract the current column index from e, and match it to the list index.
	/// This of course assumes you do not define the column bindings in the xaml.
	/// </summary>
	public class Person
	{
		public Person() { }
		public Person(SqlCommand command)
		{
			using (SqlDataReader dr = Database.DataReader(command))
			{
				if (dr.HasRows)
				{
					dr.Read();
					this.ID = Int32.Parse(dr["ID"].ToString());
					this.FirstName = dr["pFirstName"].ToString();
					this.LastName = dr["pLastName"].ToString();
					this.PhoneNumber = dr["pPhoneNumber"].ToString();
				}
			}
		}

		public Int32? ID;//<--Not Bindable(No Get/Set)
		public String FirstName { get; set; }
		public String LastName { get; set; }
		public String PhoneNumber { get; set; }

		public List<SqlParameter> GetParameters()
		{
			List<SqlParameter> pL = new List<SqlParameter>();
			if (this.ID.HasValue) { pL.Add(new SqlParameter("@ID", this.ID.Value)); }
			if (this.FirstName != "") { pL.Add(new SqlParameter("@pFirstName", this.FirstName)); }
			if (this.LastName != "") { pL.Add(new SqlParameter("@pLastName", this.LastName)); }
			if (this.PhoneNumber != "") { pL.Add(new SqlParameter("@pPhoneNumber", this.PhoneNumber)); }
			return pL;
		}
	}

	#region pluralization
	/*The type ObservableCollection<T> is great for datagrids. It carries less overhead than a List<T>, 
	though a bit less functionality. (Infact, List<T> inherits from ObservableCollection<T>)
	Howerver, it maintains all of the properties needed for binding data. 
	This could also be a List<T> or BindingList<T> and work nearly as well.
	 *NOTE: As I developed better skills with Linq, I realize that List<T> is often the better choice. 
	 **Don't be afraid to go for it and change it here.
	 */
	/// <summary>
	/// A Bindable collection of people.
	/// </summary>
	public class People : ObservableCollection<Person>
	{
		public People(){}
		public People(SqlCommand command)
		{
			using (SqlDataReader dr = Database.DataReader(command))
			{
				if (dr.HasRows)
				{
					while (dr.Read())
					{
						Person person = new Person();
						person.FirstName = dr["pFirstName"].ToString();
						person.LastName = dr["pLastName"].ToString();
						person.PhoneNumber = dr["pPhoneNumber"].ToString();
						this.Add(person);
					}
				}
			}
		}
	}
	#endregion

}
