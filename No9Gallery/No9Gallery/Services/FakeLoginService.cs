﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using No9Gallery.Models;
using Oracle.ManagedDataAccess.Client;

namespace No9Gallery.Services
{
    public class FakeLoginService: ILoginServiceInterface
    {

        public LoginUser CheckLogin(string ID, string password)
        {

            string getID, getPassword, status;
            string conString = "User Id=DBCD;Password=12345678;Data Source=localhost:1521/orcl";

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = "select ID, password, status from users where ID = '" + ID + "'";
                        OracleDataReader reader = cmd.ExecuteReader();

                        if (reader.Read() != false)
                        {
                            getID = reader.GetString(0);
                            getPassword = reader.GetString(1);
                            status = reader.GetString(2);

                            LoginUser getUser = new LoginUser()
                            {
                                ID = getID,
                                Password = getPassword,
                                Status = status
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