using Microsoft.AspNet.SignalR.Client;
using MongoDB.Bson;
using MongoDB.Driver;
using PCControllerClient.Helper;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PCControllerClient
{
    public partial class Form1 : Form
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public HubConnection Connection { get; set; }
        public IHubProxy Hub { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string username)
        {
            InitializeComponent();
            Username = username;
        }

        [DllImport("user32")]
        public static extern void LockWorkStation();

        private void Form1_Load(object sender, EventArgs e)
        {
            Url = "http://controller.enfivecore.com/signalr";
            Connection = new HubConnection(Url, useDefaultUrl: false);
            Hub = Connection.CreateHubProxy("ServiceStatusHub");
            Connection.Start().Wait();
            ConnectUser();
            Hub.On("sendOperation", delegate (string operationName)
            {
                switch ((int)Enum.Parse(typeof(Operations), operationName))
                {
                    case 0:
                        Process.Start(new ProcessStartInfo("shutdown", "/s /t 0"));
                        break;
                    case 1:
                        LockWorkStation();
                        break;
                    case 2:
                        Process.Start(new ProcessStartInfo("notepad.exe"));
                        break;
                    case 3:
                        Process.Start(new ProcessStartInfo("calc.exe"));
                        break;
                    case 4:
                        Process.Start(new ProcessStartInfo("cmd.exe"));
                        break;
                    case 5:
                        Process.Start(new ProcessStartInfo("appwiz.cpl"));
                        break;
                    case 6:
                        Process.Start(new ProcessStartInfo("services.msc"));
                        break;
                    case 7:
                        Process.Start(new ProcessStartInfo("mstsc"));
                        break;
                }
            });

        }

        public async void ConnectUser()
        {
            IMongoCollection<BsonDocument> loggedUsers = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://anurag:anurag123456@cluster0-shard-00-00.nbbmj.mongodb.net:27017,cluster0-shard-00-01.nbbmj.mongodb.net:27017,cluster0-shard-00-02.nbbmj.mongodb.net:27017/myFirstDatabase?ssl=true&replicaSet=atlas-3gospu-shard-0&authSource=admin&retryWrites=true&w=majority")).GetDatabase("ControllerDB").GetCollection<BsonDocument>("LoggedUsers");
            FilterDefinition<BsonDocument> usernameFilter = Builders<BsonDocument>.Filter.Eq((FieldDefinition<BsonDocument, string>)"Username", Username);
            if (loggedUsers.Find(usernameFilter).FirstOrDefault() != null)
            {
                BsonDocument updateConnection = new BsonDocument();
                updateConnection.Add(new BsonElement("Username", Username));
                updateConnection.Add(new BsonElement("ConnectionId", Connection.ConnectionId));
                BsonDocument oldConnection = new BsonDocument(new BsonElement("Username", Username));
                await IMongoCollectionExtensions.FindOneAndReplaceAsync(loggedUsers, (FilterDefinition<BsonDocument>)oldConnection, updateConnection, (FindOneAndReplaceOptions<BsonDocument, BsonDocument>)null, default(CancellationToken));
            }
            else
            {
                BsonDocument newConnection = new BsonDocument();
                newConnection.Add(new BsonElement("Username", Username));
                newConnection.Add(new BsonElement("ConnectionId", Connection.ConnectionId));
                await loggedUsers.InsertOneAsync(newConnection);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Connection.Stop();
            this.DeleteConnection();
            new Login().Show();
            base.Hide();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Connection.Stop();
            DeleteConnection();
            new Login().Show();
            Hide();
        }

        public async void DeleteConnection()
        {
            IMongoCollection<BsonDocument> collection = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://anurag:anurag123456@cluster0-shard-00-00.nbbmj.mongodb.net:27017,cluster0-shard-00-01.nbbmj.mongodb.net:27017,cluster0-shard-00-02.nbbmj.mongodb.net:27017/myFirstDatabase?ssl=true&replicaSet=atlas-3gospu-shard-0&authSource=admin&retryWrites=true&w=majority")).GetDatabase("ControllerDB").GetCollection<BsonDocument>("LoggedUsers");
            BsonDocument deleteConnection = new BsonDocument(new BsonElement("Username", Username));
            await IMongoCollectionExtensions.FindOneAndDeleteAsync(collection, (FilterDefinition<BsonDocument>)deleteConnection, (FindOneAndDeleteOptions<BsonDocument, BsonDocument>)null, default(CancellationToken));
        }

        private void lnkDisconnect_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (btnDisconnect.Text.ToString() == "Connected")
            {
                Connection.Stop();
                ConnectUser();
                btnDisconnect.Text = "Disconnected";
            }
            else if (btnDisconnect.Text.ToString() == "Disconnected")
            {
                Connection.Start();
                DeleteConnection();
                btnDisconnect.Text = "Connected";
            }
        }
    }
}