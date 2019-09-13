using AzureFunc.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.WindowsAzure.Storage.Table;
//using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;

namespace AzureFunc
{
    public static class CloudTableExtensions
    {
        public static async Task<ObjectInfo> AddOrUpdateToTable(ObjectInfo objectInfo)
        {
            //Sql connection 
            var guid = string.Empty;
            var text = string.Empty;
            if (string.IsNullOrEmpty(objectInfo.id))
            {
                guid = Guid.NewGuid().ToString();;
                text = $"insert into ${FunctionsSettings.TableName} (Id, Title, Description, StartDate, EndDate, CreatedBy, CreatedDate ) " +
                    $"Values (@Id, @Title, @Description, @StartDate, @EndDate, @CreatedBy, @CreatedDate)";
            }
            else
            {
                guid = objectInfo.id;
                text = $"update  ${FunctionsSettings.TableName} set Title = @Title, Description = @Description, StartDate = @StartDate," +
                    $" EndDate = @EndDate, CreatedBy = @CreatedBy, CreatedDate = @CreatedDate Where  Id = @Id";
            }

           


            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
              

                var sqlParameters = new List<SqlParameter>();
                sqlParameters.Add(new SqlParameter("@Id", guid));
             
                 
                sqlParameters.Add(new SqlParameter("@Description", objectInfo.description ?? (object)DBNull.Value));

                sqlParameters.Add(new SqlParameter("@Title", objectInfo.title ?? (object)DBNull.Value));
                sqlParameters.Add(new SqlParameter("@StartDate",objectInfo.startDate ?? (object)DBNull.Value));
                sqlParameters.Add(new SqlParameter("@EndDate",objectInfo.endDate ?? (object)DBNull.Value));

                sqlParameters.Add(new SqlParameter("@CreatedBy", objectInfo.createdBy ?? (object)DBNull.Value));
                sqlParameters.Add(new SqlParameter("@CreatedDate", DateTime.Now));

              
                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    cmd.Parameters.AddRange(sqlParameters.ToArray());

                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    //log.LogInformation($"{rows} rows were updated");
                }

                conn.Close();
            }
            objectInfo.id = guid;

            return objectInfo;

        

        }

        public static async Task<ObjectInfo> GetObjFromTable(string id, string CreatedBy)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            var query =  $"select TOP 1 Id, Title, Description, StartDate, EndDate, CreatedBy, CreatedDate from ${FunctionsSettings.TableName} where Id=@Id and CreatedBy=@CreatedBy";

            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@Id", Guid.NewGuid().ToString()));
            sqlParameters.Add(new SqlParameter("@CreatedBy", CreatedBy ?? (object)DBNull.Value));

            var objInfo = new ObjectItem();

            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqlParameters.ToArray());
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        objInfo = new ObjectItem()
                        {
                            RowKey = reader["Id"].ToString(),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            StartDate = (reader["StartDate"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["BirthDate"]),
                            EndDate = (reader["EndDate"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["BirthDate"]),
                            CreatedDate = (reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["HireDate"]),
                            IsComplete = reader["IsCompleted"] == DBNull.Value? default(bool?): (Boolean)reader["IsCompleted"]
                        };
                        break;
                    }
                    
                }

                conn.Close();
            }

            return objInfo.MapFromTableEntity();

           
        }


        public static async Task<List<ObjectItem>> GetAllObject( string CreatedBy)
        {
            var str = Environment.GetEnvironmentVariable("sqldb_connection");

            var sqlParameter = new SqlParameter("@CreatedBy", CreatedBy ?? (object)DBNull.Value);

            var query = $"select Id, Title, Description, StartDate, EndDate, CreatedBy, CreatedDate, IsComplete from ${FunctionsSettings.TableName} where CreatedBy=@CreatedBy";
            var result = new List<ObjectItem>();
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add(sqlParameter);

                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        var objInfo = new ObjectItem()
                        {
                            RowKey = reader["Id"].ToString(),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            StartDate = (reader["StartDate"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["StartDate"]),
                            EndDate = (reader["EndDate"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["EndDate"]),
                            CreatedDate = (reader["CreatedDate"] == DBNull.Value ? (DateTime?)null : (DateTime?)reader["CreatedDate"]),
                            IsComplete = reader["IsComplete"] == DBNull.Value ? default(bool?) : (Boolean)reader["IsComplete"]
                        };
                        result.Add(objInfo);
                    }
                }

                conn.Close();
            }

            return result;
        }

        public static async Task DeleteObjFromTable( string id, string CreatedBy)
        {
            var item = new ObjectItem { RowKey = id, ETag = "*" };

            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            var query = $"DELETE FROM ${FunctionsSettings.TableName} WHERE  Id=@Id and CreatedBy=@CreatedBy";

            var sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@Id", id));
            sqlParameters.Add(new SqlParameter("@CreatedBy", CreatedBy ?? (object)DBNull.Value));


            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqlParameters.ToArray());
                    var row = await cmd.ExecuteNonQueryAsync();
                }
                conn.Close();
            }

           
        }
    }
}
