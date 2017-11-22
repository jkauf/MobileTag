﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Data.SqlClient;
using System.Data;
using MobileTag.Models;
using Android.Graphics;

namespace MobileTag
{
    public static class Database
    {
        private static string CONNECTION_STRING = "";
        private static bool initialized = false;

        private static void Init_()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "mobiletag.database.windows.net";
            builder.UserID = "eallgood";
            builder.Password = "orangeChicken17";
            builder.InitialCatalog = "MobileTagDB";

            CONNECTION_STRING = builder.ConnectionString.ToString();
            initialized = true;
        }

        public static int AddUser(string username, string password, int teamID)
        {
            if (!initialized) Init_();
            int available = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand("AddPlayer", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                    cmd.Parameters.Add("@teamID", SqlDbType.Int).Value = teamID;

                    try
                    {
                        SqlDataReader reader;
                        cmd.Connection.Open();
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        available = (int)reader["Available"];
                        reader.Close();
                        cmd.Connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        return 2;
                    }
                }
            }
            return available;
        }

        public static int ValidateLoginCredentials(string username, string password)
        {
            if (!initialized) Init_();

            // userValidity 1 if valid, 0 if invalid
            int userValidity = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand("LookUpUsername", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

                    try
                    {
                        SqlDataReader reader;
                        cmd.Connection.Open();

                        reader = cmd.ExecuteReader();
                        reader.Read();
                        userValidity = (int)reader["Valid"];
                        reader.Close();
                        cmd.Connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            return userValidity;
        }

        public static int GetCellTeam(int cellID)
        {
            if (!initialized) Init_();
            int teamID = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand("GetCellTeam", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@cellID", SqlDbType.Int).Value = cellID;                    

                    try
                    {
                        SqlDataReader reader;
                        cmd.Connection.Open();

                        reader = cmd.ExecuteReader();
                        reader.Read();
                        teamID = (int)reader["TeamID"];
                        reader.Close();
                        cmd.Connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            return teamID;
        }

        public static Player GetPlayer(string username)
        {
            if (!initialized) Init_();

            int playerID = 0;
            int teamID = 0;
            string teamName = "";
            int cellID = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                using (SqlCommand cmd = new SqlCommand("GetPlayer", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                    
                    try
                    {
                        SqlDataReader reader;
                        cmd.Connection.Open();
                        
                        reader = cmd.ExecuteReader();
                        reader.Read();
                        playerID = (int)reader["PlayerID"];                        
                        teamID = (int)reader["TeamID"];
                        teamName = (string)reader["TeamName"];
                        cellID = (int)reader["CellID"];
                        reader.Close();
                        cmd.Connection.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            Team team = new Team(teamID, teamName);
            Player player = new Player(playerID, team, cellID);
            return player;
        }       


    }  
}
