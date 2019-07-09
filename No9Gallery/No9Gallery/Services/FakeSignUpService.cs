using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using No9Gallery.Models;
using Oracle.ManagedDataAccess.Client;

namespace No9Gallery.Services
{
    public class FakeSignUpService: ISignUpService
    {
        public Task<bool> CheckId(string signUpId)
        {
            bool isExist = false;
            using (OracleConnection con = new OracleConnection(ConString.conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = "select count(*) from users where ID = '" + signUpId + "'";
                        OracleDataReader reader = cmd.ExecuteReader();
                        reader.Read();

                        isExist = (reader.GetInt32(0) == 0) ? false : true; 

                        reader.Dispose();
                    }
                    catch (Exception ex)
                    {
                        string e = ex.Message;
                    }
                }
            }

            return Task.FromResult(isExist);
        }

        public async Task<bool> SignUpAsync(SignUpUser signUpUser)
        {
            bool success = false;
            using (OracleConnection con = new OracleConnection(ConString.conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = "insert into users values(" +
                            "'" + signUpUser.ID + "'" + "," +
                            "'" + signUpUser.Name + "'" + "," +
                            "'" + signUpUser.Password + "'" + "," +
                            "'Common'" + "," +
                            "'Default.png'" + ")";


                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "insert into common_user values(" +
                            "'" + signUpUser.ID + "'" + "," +
                            "'" + "No Introduction" + "'" + "," +
                            + 0 + "," +
                            + 0 + ")";

                        await cmd.ExecuteNonQueryAsync();

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        string e = ex.Message;
                    }

                }

            }
            return success;
        }
    }
}
