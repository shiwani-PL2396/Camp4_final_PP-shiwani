using Microsoft.Data.SqlClient;

namespace CMS_ClassLibraryDBConnectivity
{
    public class DatabaseConncectionManager
    {
       
 
            //SQL connection from app.config

            public static SqlConnection OpenConnection(string _connString)
            {
                //field
                SqlConnection connection = null;
                try
                {
                    //step1 :Configure Connection String
                    if (_connString != null || Convert.ToString(connection.State) == "Closed")
                    {
                        //open connection
                        connection = new SqlConnection(_connString);
                        connection.Open();
                    }

                    return connection;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Oops ,SQL Server error occured");
                    Console.WriteLine(ex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Oops ,Something went wrong\n" + ex);
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
    }
 
