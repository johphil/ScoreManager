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
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;

namespace ScoreManager
{
    class License
    {
        /// <summary>
        /// FIREBASE METHODS (READ, WRITE, CHECK CONNECTION)
        /// </summary>
        #region FIREBASE
        public _User _user;
        private IFirebaseClient fbClient;
        private readonly IFirebaseConfig fbConfig = new FirebaseConfig
        {
            AuthSecret = Globals.FIREBASE_SECRET,
            BasePath = Globals.FIREBASE_PATH
        };

        public bool IsFirebaseConnected()
        {
            fbClient = new FireSharp.FirebaseClient(fbConfig);

            if (fbClient != null)
                return true;
            else
                return false;
        }

        public async Task GetRegistrationFirebase(string License)
        {
            if (IsFirebaseConnected())
            {
                FirebaseResponse fbResponse = await fbClient.GetTaskAsync(Globals.FIREBASE_PATH_LICENSE + License);
                _user = null;

                try
                {
                    _user = fbResponse.ResultAs<_User>();
                }
                catch
                { }
                
            }
        }

        public async Task DeactivateFirebase(string License)
        {
            try
            {

                _User _user = new _User();

                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT NAME, EMAIL, USERNAME, PASSWORD FROM tblUsers WHERE LICENSE = @license;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;

                        con.Open();

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _user.NAME = reader[0].ToString();
                                _user.EMAIL = reader[1].ToString();
                                _user.USERNAME = reader[2].ToString();
                                _user.PASSWORD = reader[3].ToString();
                                _user.ISACTIVATED = 0;

                                await fbClient.SetTaskAsync(Globals.FIREBASE_PATH_LICENSE + License, _user);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        /// <summary>
        /// Activates the license, insert the user information into the local database and creates a license file.
        /// </summary>
        /// <param name="License">License key of the user</param>
        /// <param name="ProcessorID">Processor ID of the device. Used to prevent the illegal distribution of the system.</param>
        /// <param name="_user">Hold the informatino of the user</param>
        /// <returns></returns>
        public async Task Activate(string License, string ProcessorID, _User _user)
        {
            try
            {
                _user.ISACTIVATED = 1;
                await fbClient.SetTaskAsync("License/" + License, _user);

                using (StreamWriter file = new StreamWriter(Globals.LicenseFile))
                {
                    //insert hashing
                    file.WriteLine(License);
                }

                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "INSERT OR REPLACE INTO tblUsers VALUES(@license, @username, @password, @name, @email, @isactivated, @processorid);";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;
                        cmd.Parameters.Add("@username", DbType.String).Value = _user.USERNAME;
                        cmd.Parameters.Add("@password", DbType.String).Value = _user.PASSWORD;
                        cmd.Parameters.Add("@name", DbType.String).Value = _user.NAME;
                        cmd.Parameters.Add("@email", DbType.String).Value = _user.EMAIL;
                        cmd.Parameters.Add("@isactivated", DbType.Int32).Value = 1;
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

        /// <summary>
        /// Checks if the license file exists
        /// </summary>
        /// <returns></returns>
        public bool IsLicenseFileExists()
        {
            if (File.Exists(Globals.LicenseFile))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get the license key that was written in the license file
        /// </summary>
        /// <returns></returns>
        public string GetLicenseKey()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(Globals.LicenseFile))
                {
                    // Read the stream to a string
                    // insert dehash
                    return sr.ReadToEnd().Trim();
                }
            }
            catch
            {
                return Globals.ERROR.ToString();
            }
        }

        /// <summary>
        /// Gets the username and password of the currently activated user. (kukunin yung userpass ng activated na license)
        /// </summary>
        /// <param name="License">License key</param>
        /// <param name="Username">Username of the licensed user</param>
        /// <param name="Password">Password of the licensed user</param>
        public void GetLicenseUserPass(string License, out string Username, out string Password)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(Globals.DbConString))
                {
                    string query = "SELECT USERNAME, PASSWORD FROM tblUsers WHERE LICENSE = @license;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        cmd.Parameters.Add("@license", DbType.String).Value = License;

                        con.Open();
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Username = reader[0].ToString();
                                Password = reader[1].ToString();
                            }
                            else
                            {
                                Username = Globals.ERROR.ToString();
                                Password = Globals.ERROR.ToString();
                            }
                        }
                    }
                }
            }
            catch
            {
                Username = Globals.ERROR.ToString();
                Password = Globals.ERROR.ToString();
            }
        }

        /// <summary>
        /// Gets the name and email of the licensed user
        /// </summary>
        /// <param name="License">License key</param>
        /// <param name="Name">Name of the registered user</param>
        /// <param name="Email">Email of the registered user</param>
        public void GetLicenseNameEmail(string License, out string Name, out string Email)
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

        /// <summary>
        /// Checks the database of the user's license is activated, existing, or not 
        /// </summary>
        /// <param name="License">License Key</param>
        /// <param name="ProcessorID">Processor of the device</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Checks if the processor id of the current device is equal to the activated processor id of the user in the database.
        /// </summary>
        /// <param name="License">License key</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if the License Key has already been activated or not
        /// </summary>
        /// <param name="License"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the processor ID of the current device.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Deactivate the license by setting the ISACTIVATED = 0 in the database
        /// </summary>
        /// <param name="License"></param>
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
