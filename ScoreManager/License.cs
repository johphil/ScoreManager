using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Management;
using System.Collections;
using System.Configuration;

namespace ScoreManager
{
    class License
    {
        //activate ang license key for first time use or lost key and update isactivated = 1
        public void Activate(string License, string ProcessorID)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(Globals.LicenseFile))
                {
                    //hash muna
                    file.WriteLine(License);
                }

                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "UPDATE tblUsers " +
                        "SET ISACTIVATED = 1, " +
                        "PROCESSOR_ID = @processorid " +
                        "WHERE LICENSE = @license;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;
                        cmd.Parameters.Add("@processorid", DbType.String).Value = ProcessorID;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //check kung existing ang license.txt
        public bool IsLicenseFileExists()
        {
            if (File.Exists(Globals.LicenseFile))
                return true;
            else
                return false;
        }

        //get license file key
        public string GetLicenseKey()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Globals.LicenseFile))
                {
                    // Read the stream to a string, and write the string to the console.
                    return sr.ReadToEnd().Trim();
                }
            }
            catch
            {
                return Globals.ERROR.ToString();
            }
        }

        //kunin ang name at email ng licensed user
        public void GetLicenseInfo(string License, out string Name, out string Email)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT NAME, EMAIL FROM tblUsers WHERE LICENSE = @license AND ISACTIVATED = 1;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;

                        con.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Name = reader[0].ToString();
                                Email = reader[1].ToString();
                            }
                            else
                            {
                                Name = Globals.ERROR.ToString();
                                Email = Globals.ERROR.ToString();
                            }
                        }
                    }
                }
            }
            catch
            {
                Name = Globals.ERROR.ToString();
                Email = Globals.ERROR.ToString();
            }
        }

        //check if isactivated == 1 and check device if same
        public bool IsActivated(string License, string ProcessorID)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT COUNT(*) FROM tblUsers WHERE LICENSE = @license AND ISACTIVATED = 1 AND PROCESSOR_ID = @processorid;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;
                        cmd.Parameters.Add("@processorid", DbType.String).Value = ProcessorID;

                        con.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (Convert.ToInt32(reader[0]) == 1)
                                    return true;
                                else
                                    return false;
                            }
                            else
                                return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        //check if isactivated with another device
        public bool IsActivatedNotSameDevice(string License)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT PROCESSOR_ID FROM tblUsers WHERE LICENSE = @license AND ISACTIVATED = 1;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;

                        con.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (!string.IsNullOrEmpty(reader[0].ToString()) && reader[0].ToString() != GetProcessorID())
                                    return true;
                                else
                                    return false;
                            }
                            else
                                return true;
                        }
                    }
                }
            }
            catch
            {
                return true;
            }
        }

        //check if isactivated == 0
        public bool ForActivation(string License)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT COUNT(*) FROM tblUsers WHERE LICENSE = @license AND ISACTIVATED = 0;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;

                        con.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (Convert.ToInt32(reader[0]) == 1)
                                    return true;
                                else
                                    return false;
                            }
                            else
                                return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        //get processor id
        public string GetProcessorID()
        {
            var processorID = "";
            var query = "SELECT ProcessorId FROM Win32_Processor";
            
            var oManagementObjectSearcher = new ManagementObjectSearcher(query);

            foreach (var oManagementObject in oManagementObjectSearcher.Get())
            {
                processorID = (string)oManagementObject["ProcessorId"];
                break;
            }
            return processorID.Trim();
        }

        //deactivate license
        public void Deactivate(string License)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "UPDATE tblUsers " +
                        "SET ISACTIVATED = 0, " +
                        "PROCESSOR_ID = 0 " +
                        "WHERE LICENSE = @license;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("The action could not be completed. Sorry.");
            }
        }
    }
}
