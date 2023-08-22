using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Collections.ObjectModel;
using ForceTools.Models;
using System.Collections.Generic;

namespace ForceTools
{
    public class SqlDatabaseHandler
    {
        // SqlConnectionString ---> Dynamic Connection string for Database use
        // DefaultServerString ---> Dynamic Connection string for Server use

        private static void CreateDatabase(string DbName, string Connection)
        {
            using (var con = new SqlConnection(Connection))
            {
                #region Code for instantiating filenames - Unused
                //string[] filesMdf = Directory.GetFiles(DataFolderPath, "ForceTools*.mdf");
                //string[] filesLdf = Directory.GetFiles(DataFolderPath, "ForceTools*.ldf");
                //string baseName = System.IO.Path.Combine(DataFolderPath, "ForceTools");
                //string filenameMdf;
                //string filenameLdf;
                //int i = 0;
                //do
                //{
                //    filenameMdf = baseName + ++i + ".mdf";
                //    filenameLdf = baseName + ++i + ".ldf";
                //} while (filesMdf.Contains(filenameMdf) && filesLdf.Contains(filenameLdf));
                #endregion
                string DbLocation = System.IO.Path.Combine(FileSystemHelper.DataFolderPath, DbName);

                string Dbstr = "CREATE DATABASE " + $"\"{DbName}\"" + " ON PRIMARY " +
                "(NAME = " + $"\"{DbName}\"" + "," +
                $"FILENAME = '{DbLocation}.mdf', " +
                "SIZE = 2MB, MAXSIZE = UNLIMITED, FILEGROWTH = 10%)" +
                "LOG ON (NAME = MyDatabase_Log, " +
                $"FILENAME = '{DbLocation}.ldf', " +
                "SIZE = 1MB, " +
                "MAXSIZE = 50MB, " +
                "FILEGROWTH = 10%)";

                using (var myCommand = new SqlCommand(Dbstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }
        }
        private static void CreateTables(string ConStringForTheNewlyCreatedDB)
        {
            List<string> SqlTablesCreationQuarryList = new List<string>();
            //Fakturi Table
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[Fakturi] (
                                                 [Id]                     INT             IDENTITY (1, 1) NOT NULL,
                                                 [DocPayableReceivableId] INT             NULL,
                                                 [KontragentiId]          BIGINT          NULL,
                                                 [AccDate]                DATE            NULL,
                                                 [Date]                   DATE            NULL,
                                                 [Number]                 BIGINT          NULL,
                                                 [DO]                     DECIMAL (18, 2) NULL,
                                                 [DDS]                    DECIMAL (18, 2) NULL,
                                                 [FullValue]              DECIMAL (18, 2) NULL,
                                                 [DealKindId]             INT             NULL,
                                                 [DocTypeId]              INT             NULL,
                                                 [Account]                INT             NULL,
                                                 [InCashAccount]          INT             NULL,
                                                 [Note]                   NVARCHAR (50)   NULL,
                                                 [Image]                  IMAGE           NULL,
                                                 [AccountingStatusId]     INT             NULL,
                                                 [PurchaseOrSale]         NVARCHAR (20)   NULL,
                                                 PRIMARY KEY CLUSTERED ([Id] ASC)
                                             );");

            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[AccountingStatuses] (
                                                [Id]               INT        NOT NULL,
                                                [AccountingStatus] NCHAR (15) NOT NULL,
                                                PRIMARY KEY CLUSTERED ([Id] ASC)
                                            );");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[ImportList] (
                                                [Id]                 INT             IDENTITY (1, 1) NOT NULL,
                                                [KontragentiId]      BIGINT          NULL,
                                                [Date]               DATE            NULL,
                                                [Number]             BIGINT          NULL,
                                                [DO]                 DECIMAL (18, 2) NULL,
                                                [DDS]                DECIMAL (18, 2) NULL,
                                                [FullValue]          DECIMAL (18, 2) NULL,
                                                [AccountingStatusId] INT             NULL,
                                                [NameAndEik]         NVARCHAR (50)   NULL,
                                                PRIMARY KEY CLUSTERED ([Id] ASC)
                                            );");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[Kontragenti] (
                                                [Id]             INT           IDENTITY (1, 1) NOT NULL,
                                                [Name]           NVARCHAR (50) NULL,
                                                [EIK]            NVARCHAR (12) NULL,
                                                [DDSNumber]      NVARCHAR (50) NULL,
                                                [LastUsedDataId] INT           NULL,
                                                PRIMARY KEY CLUSTERED ([Id] ASC)
                                            );");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[KindOfDeals] (
                                                [Id]         INT           NOT NULL,
                                                [Percentage] NVARCHAR (5)  NULL,
                                                [DealName]   NVARCHAR (50) NULL,
                                                PRIMARY KEY CLUSTERED ([Id] ASC)
                                            );");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[DocumentTypes] (
                                                [Id]       INT           NOT NULL,
                                                [TypeName] NVARCHAR (30) NULL,
                                                PRIMARY KEY CLUSTERED ([Id] ASC)
                                            );");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[Accounts] (
                                                [Account]     INT           NOT NULL,
                                                [AccountName] NVARCHAR (65) NULL,
                                                PRIMARY KEY CLUSTERED ([Account] ASC)
                                            );");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[DefaultValues] (
                                            	[Id] INT NOT NULL PRIMARY KEY, 
                                                [Name] NVARCHAR(50) NULL, 
                                                [Value] NVARCHAR(50) NULL
                                            )");
            SqlTablesCreationQuarryList.Add(@"CREATE TABLE [dbo].[LastUsedKontragentData] (
                                                [Id]           INT           IDENTITY (1, 1) NOT NULL,
                                                [PurchaseAcc]  INT           NULL,
                                                [SaleAcc]      INT           NULL,
                                                [PurchaseNote] NVARCHAR (50) NULL,
                                                [SaleNote]     NVARCHAR (50) NULL,
                                                PRIMARY KEY CLUSTERED ([Id] ASC)
                                            )");
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                using (var myCommand = new SqlCommand("", con))
                {
                    myCommand.Connection = con;
                    try
                    {
                        con.Open();
                        foreach (string quarry in SqlTablesCreationQuarryList)
                        {
                            myCommand.CommandText = quarry;
                            myCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }
        }
        private static void InsertDefaultDataInTables(string ConStringForTheNewlyCreatedDB)
        {
            string[] QuaryArray = new string[]
                {
                    "INSERT INTO DocumentTypes(Id, TypeName) VALUES (1, 'Фактура');",
                    "INSERT INTO DocumentTypes(Id, TypeName) VALUES (2, 'Дебитно известие');",
                    "INSERT INTO DocumentTypes(Id, TypeName) VALUES (3, 'Кредитно известие');",
                    "INSERT INTO DocumentTypes(Id, TypeName) VALUES (4, 'Протокол или друг документ');",

                    "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (1, 'Нови');",
                    "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (2, 'Неосчетоводени');",
                    "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (3, 'ГотовиЗаЕкспорт');",
                    "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (4, 'Експортирани');",
                    "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (5, 'Осчетоводени');",

                    "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (21, '20%','Продажба със ставка 20%');",
                    "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (24, '9%','Продажба със ставка 9%');",
                    "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (25, '0%','Продажба със ставка 0%');",
                    "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (12, '20%','Покупка с пълен ДК');",

                    "INSERT INTO DefaultValues(Id, Name, Value) VALUES (1, 'Покупки Сметка', 602);",
                    "INSERT INTO DefaultValues(Id, Name, Value) VALUES (2, 'Продажби Сметка', 703);",
                    "INSERT INTO DefaultValues(Id, Name, Value) VALUES (3, 'Каса Сметка', 503);",
                    "INSERT INTO DefaultValues(Id, Name) VALUES (4, 'Бележка покупка');",
                    "INSERT INTO DefaultValues(Id, Name) VALUES (5, 'Бележка продажба');",
                };

            using (var myCommand = new SqlCommand())
            {
                using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
                {
                    myCommand.Connection = con;
                    try
                    {
                        con.Open();
                        foreach (string Quarry in QuaryArray)
                        {
                            myCommand.CommandText = Quarry;
                            myCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }
        }

        public static void CreateNewDatabase(string DbName, string Connection)
        {
            string ConStringForTheNewlyCreatedDB = $"{ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString};Initial Catalog={DbName}";
            CreateDatabase(DbName, Connection);
            CreateTables(ConStringForTheNewlyCreatedDB);
            InsertDefaultDataInTables(ConStringForTheNewlyCreatedDB);
            MessageBox.Show($"Датабаза \"{DbName}\" е създадена успешно.", "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static void DeleteDatabase(string DbName, string Connection)
        {
            using (var con = new SqlConnection(Connection))
            {
                using (var myCommand = new SqlCommand($"ALTER DATABASE \"{DbName}\" SET SINGLE_USER     WITH ROLLBACK IMMEDIATE", con))
                {
                    con.Open();
                    myCommand.ExecuteNonQuery();

                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
                using (var myCommand = new SqlCommand($"DROP DATABASE \"{DbName}\"", con))
                {
                    con.Open();
                    myCommand.ExecuteNonQuery();

                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }
        public static void FirstTimeLoginSettingsSetter(string Account)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
            {
                Con.Open();

                //Checking if ForceToolsAdmin Role exists
                using (SqlCommand sqcmd = new SqlCommand("select *  from sys.server_principals where name = 'ForceToolsAdmin'", Con))
                {
                    SqlDataAdapter Adapter = new SqlDataAdapter(sqcmd);
                    DataTable dataTableRoleCheck = new DataTable();
                    Adapter.Fill(dataTableRoleCheck);
                    if (dataTableRoleCheck.Rows.Count <= 0)
                    {
                        //Creating the ForceToolsAdmin Server role and Adding the first logged in account to the role
                        using (SqlCommand cmd = new SqlCommand($"CREATE SERVER ROLE ForceToolsAdmin;", Con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Con.Close();
            }
            //Setting up the server for remote access via local ip 
            #region Here Will be the code which automatically configures SQL Server for Remote access via TCP/IP Protocol
            //using (SqlConnection Con = new SqlConnection(DefaultServerString))
            //{
            //      string ServerName = "";
            //      string SqlServiceName = "";
            //      string FullServiceName = "";
            //  
            //    string commands2 = "SELECT @@SERVERNAME";
            //    string commands3 = "SELECT DSS.servicename FROM sys.dm_server_services AS DSS;";

            //    //Getting the Server name
            //    using (SqlCommand sqcmd = new SqlCommand(commands2, Con))
            //    {
            //        try
            //        {
            //            SqlDataReader dr = sqcmd.ExecuteReader();
            //            dr.Read();
            //            ServerName = dr[0].ToString();
            //            dr.Close();
            //        }
            //        catch
            //        {
            //            MessageBox.Show("Failed to Set SQL Server Properties for remote connections.");
            //        }
            //    }
            //    //Getting the Service name
            //    using (SqlCommand sqcmd = new SqlCommand(commands3, Con))
            //    {
            //        try
            //        {
            //            SqlDataReader dr = sqcmd.ExecuteReader();
            //            dr.Read();
            //            FullServiceName = dr[0].ToString().ToUpper();
            //            SqlServiceName = FullServiceName.Replace("SQL SERVER (", "").Replace(")", "");
            //            dr.Close();
            //        }
            //        catch
            //        {
            //            MessageBox.Show("Failed to Set SQL Server Properties for remote connections.");
            //        }
            //    }
            //    if (Con.State == ConnectionState.Open)
            //    {
            //        Con.Close();
            //    }
            //}
            #endregion
        }
        public static ObservableCollection<Databases> GetUserCreatedDatabasesObservableCollection()
        {
            ObservableCollection<Databases> databaseObserbavleCollection = new ObservableCollection<Databases>();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString))
            {
                con.Open();
                DataTable databases = con.GetSchema("Databases");
                foreach (DataRow database in databases.Rows)
                {
                    String databaseName = database.Field<String>("database_name");
                    string dbID = database.Field<short>("dbid").ToString();
                    //DateTime creationDate = database.Field<DateTime>("create_date");

                    if (databaseName != "master" && databaseName != "tempdb" && databaseName != "model" && databaseName != "msdb" && databaseName != "Users")
                    {
                        Databases data = new Databases(databaseName, dbID, BitmapCreator.DefaultDatabaseBitmapImage());
                        databaseObserbavleCollection.Add(data);
                    }
                }
            }
            return databaseObserbavleCollection;
        }  
    }
}
