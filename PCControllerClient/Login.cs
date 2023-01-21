using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCControllerClient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            IMongoCollection<BsonDocument> collection = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://anurag:anurag123456@cluster0-shard-00-00.nbbmj.mongodb.net:27017,cluster0-shard-00-01.nbbmj.mongodb.net:27017,cluster0-shard-00-02.nbbmj.mongodb.net:27017/myFirstDatabase?ssl=true&replicaSet=atlas-3gospu-shard-0&authSource=admin&retryWrites=true&w=majority")).GetDatabase("ControllerDB", null).GetCollection<BsonDocument>("Users", null);
            if ((this.txtPassword.Text == "") || (this.txtUsername.Text == ""))
            {
                MessageBox.Show("Password and Confirm Password didn't matched.");
            }
            else
            {
                CancellationToken cancellationToken = new CancellationToken();
                if (collection.Find<BsonDocument>((Builders<BsonDocument>.Filter.Eq<string>("Username", this.txtUsername.Text) & Builders<BsonDocument>.Filter.Eq<string>("Password", this.txtPassword.Text)), null).FirstOrDefault<BsonDocument, BsonDocument>(cancellationToken) == null)
                {
                    MessageBox.Show("Username or Email already exists.");
                }
                else
                {
                    new Form1(this.txtUsername.Text).Show();
                    base.Hide();
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Register().Show();
            base.Hide();
        }
    }
}
