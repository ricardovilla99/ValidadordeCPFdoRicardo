﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ValidadordeCPFdoRicardo
{

    public class Conn
    {

        private static string server = @"localhost";
        private static string dataBase = "sqltry1";
        private static string user = "";
        private static string password = "";


        public static string StrCon
        {


            get { return $"Data Source={server}; Integrated Security=True;Initial Catalog={dataBase}; User ID={user}; Password={password}"; }




        }
    }
}
