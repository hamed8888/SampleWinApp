using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SampleWinApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoadXMLDocument_Click(object sender, EventArgs e)
        {
            SQLiteConnection sqlite_conn = CreateConnection();
            DropTableIfExists(sqlite_conn);
            CreateTable(sqlite_conn);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            string nodeName = string.Empty;
            using (var fileStream = File.OpenText("sample.xml"))
            using (XmlReader reader = XmlReader.Create(fileStream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine($"Start Element: {reader.Name}. Has Attributes? : {reader.HasAttributes}");
                            nodeName = reader.Name;
                            break;
                        case XmlNodeType.Text:
                            Console.WriteLine($"Inner Text: {reader.Value}");
                            InsertData(sqlite_conn, nodeName, reader.Value);
                            break;
                        case XmlNodeType.EndElement:
                            Console.WriteLine($"End Element: {reader.Name}");
                            break;
                        default:
                            Console.WriteLine($"Unknown: {reader.NodeType}");
                            break;
                    }
                }
            }
            MessageBox.Show("Content stored successfully! Click other button to load stored Data");
        }
        #region Database Actions
        SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                throw;
            }
            return sqlite_conn;
        }
        void DropTableIfExists(SQLiteConnection conn)
        {
            try
            {

                SQLiteCommand sqlite_cmd;
                string Createsql = "Drop TABLE XMLNode";
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                
            }
        }
        void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE XMLNode(AttributeName VARCHAR(100), AttributeValue VARCHAR(400))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }
        void InsertData(SQLiteConnection conn, string attributeName, string attributeValue)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO XMLNode (AttributeName, AttributeValue) VALUES('{attributeName}', '{attributeValue}')";
            sqlite_cmd.ExecuteNonQuery();
        }
        void ReadData(SQLiteConnection conn)
        {
            listBox1.Items.Clear();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM XMLNode";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string val = sqlite_datareader.GetString(0) + ": " + sqlite_datareader.GetString(1);
                listBox1.Items.Add(val);
            }
            conn.Close();
        }
        #endregion Database Actions

        private void button1_Click(object sender, EventArgs e)
        {
            SQLiteConnection connection = CreateConnection();
            ReadData(connection);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
