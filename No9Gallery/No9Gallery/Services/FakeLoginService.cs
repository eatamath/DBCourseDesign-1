using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using No9Gallery.Models;
using Oracle.ManagedDataAccess.Client;

namespace No9Gallery.Services
{
    public class ConString
    {
        public static string conString = "User Id=C##DBCD;Password=12345678;Data Source=localhost:1521/orcl";
    }

    public class FakeLoginService: ILoginService
    {

        public async Task<LoginUser> CheckLogin(string ID, string password)
        {

            string getID, getName, getAvatar, getStatus;

            using (OracleConnection con = new OracleConnection(ConString.conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = "select * from users where ID = '" + ID + "' and password = '" + password + "'";
                        OracleDataReader reader = cmd.ExecuteReader();

                        if (reader.Read() != false)
                        {
                            getID = reader.GetString(0);
                            getName = reader.GetString(1);
                            getStatus = reader.GetString(3);
                            getAvatar = reader.GetString(4);

                            LoginUser getUser = new LoginUser()
                            {
                                ID = getID,
                                Name = getName,
                                Status = getStatus,
                                Avatar = getAvatar
                            };

                            reader.Dispose();
                            return getUser;
                        }

                        reader.Dispose();
                    }
                    catch (Exception ex)
                    {
                        string e = ex.Message;
                        int i = 0;
                    }
                }
            }
            return null;
        }
    }
}
