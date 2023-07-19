using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Collections.ObjectModel;
using ForceTools.Models;
using Microsoft.SqlServer.Management.Smo;

namespace ForceTools
{
    public class SqlDatabaseHandler
    {
        // SqlConnectionString ---> Dynamic Connection string for Database use
        // DefaultServerString ---> Dynamic Connection string for Server use


        private static string DataFolderPath = AppDomain.CurrentDomain.BaseDirectory + "\\Database";

        public static void CreateNewDatabase(string DbName, string Connection)
        {
            #region Creating The Database
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
                string DbLocation = System.IO.Path.Combine(DataFolderPath, DbName);

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
                        MessageBox.Show("DataBase is Created Successfully", "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
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
            #endregion
            #region Creating The Tables
            //New DB connection String
            string ConStringForTheNewlyCreatedDB = $"{ConfigurationManager.ConnectionStrings["DefaultSqlConnection"].ConnectionString};Initial Catalog={DbName}"; ;

            #region Creating Fakturi Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                string Dbstr = "CREATE TABLE [dbo].[Fakturi] (\r\n    [Id]                     INT             IDENTITY (1, 1) NOT NULL,\r\n    [DocPayableReceivableId] INT             NULL,\r\n    [KontragentiId]          BIGINT          NULL,\r\n    [AccDate]                DATE            NULL,\r\n    [Date]                   DATE            NULL,\r\n    [Number]                 BIGINT          NULL,\r\n    [DO]                     DECIMAL (18, 2) NULL,\r\n    [DDS]                    DECIMAL (18, 2) NULL,\r\n    [FullValue]              DECIMAL (18, 2) NULL,\r\n    [DealKindId]             INT             NULL,\r\n    [DocTypeId]              INT             NULL,\r\n    [Account]                INT             NULL,\r\n    [InCashAccount]          INT             NULL,\r\n    [Note]                   NVARCHAR (50)   NULL,\r\n    [Image]                  IMAGE           NULL,\r\n    [AccountingStatusId]     INT             NULL,\r\n    [PurchaseOrSale]         NVARCHAR (20)   NULL,\r\n    PRIMARY KEY CLUSTERED ([Id] ASC)\r\n);";


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

            #endregion
            #region Creating AccountingStatuses Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                #region Creating the Table

                string Dbstr = "CREATE TABLE [dbo].[AccountingStatuses] (\r\n    [Id]               INT        NOT NULL,\r\n    [AccountingStatus] NCHAR (15) NOT NULL,\r\n    PRIMARY KEY CLUSTERED ([Id] ASC)\r\n);";


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
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
                #region Adding Info in the table
                string Addstr = "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (1, 'Нови');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (2, 'Неосчетоводени');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (3, 'ГотовиЗаЕкспорт');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (4, 'Експортирани');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO AccountingStatuses(Id, AccountingStatus) VALUES (5, 'Осчетоводени');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
                con.Close();
            }

            #endregion
            #region Creating ImportList Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                string Dbstr = "CREATE TABLE [dbo].[ImportList] (\r\n    [Id]                 INT             IDENTITY (1, 1) NOT NULL,\r\n    [KontragentiId]      BIGINT          NULL,\r\n    [Date]               DATE            NULL,\r\n    [Number]             BIGINT          NULL,\r\n    [DO]                 DECIMAL (18, 2) NULL,\r\n    [DDS]                DECIMAL (18, 2) NULL,\r\n    [FullValue]          DECIMAL (18, 2) NULL,\r\n    [AccountingStatusId] INT             NULL,\r\n    [NameAndEik]         NVARCHAR (50)   NULL,\r\n    PRIMARY KEY CLUSTERED ([Id] ASC)\r\n);";


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
            #endregion
            #region Creating Kontragenti Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                string Dbstr = "CREATE TABLE [dbo].[Kontragenti] (\r\n    [Id]             INT           IDENTITY (1, 1) NOT NULL,\r\n    [Name]           NVARCHAR (50) NULL,\r\n    [EIK]            NVARCHAR (12) NULL,\r\n    [DDSNumber]      NVARCHAR (50) NULL,\r\n    [LastUsedDataId] INT           NULL,\r\n    PRIMARY KEY CLUSTERED ([Id] ASC)\r\n);";


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
            #endregion
            #region Creating KindOfDeals Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                #region Creating the Table
                string Dbstr = "CREATE TABLE [dbo].[KindOfDeals] (\r\n    [Id]         INT           NOT NULL,\r\n    [Percentage] NVARCHAR (5)  NULL,\r\n    [DealName]   NVARCHAR (50) NULL,\r\n    PRIMARY KEY CLUSTERED ([Id] ASC)\r\n);";


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
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
                #region Adding Info in the table
                string Addstr = "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (21, '20%','Продажба със ставка 20%');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (24, '9%','Продажба със ставка 9%');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (25, '0%','Продажба със ставка 0%');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO KindOfDeals(Id, Percentage, DealName) VALUES (12, '20%','Покупка с пълен ДК');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Creating DocumentTypes Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                #region Creating the Table
                string Dbstr = "CREATE TABLE [dbo].[DocumentTypes] (\r\n    [Id]       INT           NOT NULL,\r\n    [TypeName] NVARCHAR (30) NULL,\r\n    PRIMARY KEY CLUSTERED ([Id] ASC)\r\n);";


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
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
                #region Adding Info in the table
                string Addstr = "INSERT INTO DocumentTypes(Id, TypeName) VALUES (1, 'Фактура');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO DocumentTypes(Id, TypeName) VALUES (2, 'Дебитно известие');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO DocumentTypes(Id, TypeName) VALUES (3, 'Кредитно известие');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO DocumentTypes(Id, TypeName) VALUES (4, 'Протокол или друг документ');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Creating Accounts Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                string Dbstr = "CREATE TABLE [dbo].[Accounts] (\r\n    [Account]     INT           NOT NULL,\r\n    [AccountName] NVARCHAR (65) NULL,\r\n    PRIMARY KEY CLUSTERED ([Account] ASC)\r\n);";


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
            #endregion
            #region Creating DefaultValues Table

            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                #region Creating the Table
                string Dbstr = "CREATE TABLE [dbo].[DefaultValues]\r\n(\r\n\t[Id] INT NOT NULL PRIMARY KEY, \r\n    [Name] NVARCHAR(50) NULL, \r\n    [Value] NVARCHAR(50) NULL\r\n)";


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
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
                #region Adding Info in the table
                string Addstr = "INSERT INTO DefaultValues(Id, Name, Value) VALUES (1, 'Покупки Сметка', 602);";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO DefaultValues(Id, Name, Value) VALUES (2, 'Продажби Сметка', 703);";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO DefaultValues(Id, Name, Value) VALUES (3, 'Каса Сметка', 503);";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                Addstr = "INSERT INTO DefaultValues(Id, Name) VALUES (4, 'Бележка');";
                using (var myCommand = new SqlCommand(Addstr, con))
                {
                    try
                    {
                        con.Open();
                        myCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "ForceTools", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
                #endregion
            }


            #endregion
            #region Creating LastUsedKontragentData Table
            using (var con = new SqlConnection(ConStringForTheNewlyCreatedDB))
            {
                string Dbstr = "CREATE TABLE [dbo].[LastUsedKontragentData]\r\n(\r\n\t[Id] INT NOT NULL PRIMARY KEY, \r\n    [PurchaseAcc] INT NULL, \r\n    [SaleAcc] INT NULL, \r\n    [PurchaseNote] NVARCHAR(50) NULL, \r\n    [SaleNote] NVARCHAR(50) NULL\r\n)";


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
            #endregion
            #endregion
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
