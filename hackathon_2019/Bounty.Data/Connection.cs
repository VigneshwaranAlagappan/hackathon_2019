﻿namespace Bounty.Data
{
    using System;
    using System.Web.Configuration;

    public class Connection
    {
        public static string ConnectionString => "Data Source = " + AppDomain.CurrentDomain.BaseDirectory + WebConfigurationManager.AppSettings["AppDataPath"] + "BugReport.sdf; Password = syncfusion";      
    }
}