using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace CameraCapture
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DBConnect()
        {
            Initialize();
        }

        private void Initialize()
        {
            //parametrage des logins
            server = "localhost";
            database = "location";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + 
                "DATABASE=" + database + ";" + 
                "UID=" + uid + ";" + 
                "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            // ouverture de connexion sur la DB
            try
            {
                connection.Open();
                return true;
            }catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Impossible de se connecter à la DB.");
                        break;
                    case 1045:
                        MessageBox.Show("Le MDP/Nom d'utilisateur est incorrect.");
                        break;
                }
                return false;
            }
        }

        public bool CloseConnection()
        {
            //fermeture de la connexion à la DB
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("ex.Message");
                return false;
            }
        }

        public void Insert(Bitmap face, String nom, String prenom)
        {
            //insertion d'un visage dans la DB
            string query = "INSERT INTO face (nom, prenom, imgFace, horodatage) VALUE(@Nom, @Prenom, @ImgFace ,@Horodatage)";

            //ouverture de la connexion
            if(this.OpenConnection() == true)
            {
                //creation de la commande avec la query et les parametre de connexion
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.Add("@nom", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@prenom", MySqlDbType.VarChar, 45);
                cmd.Parameters.Add("@imgFace", MySqlDbType.MediumBlob);
                cmd.Parameters.Add("@Horodatage", MySqlDbType.DateTime);

                //permet la convertion de l'image en binaire pour l'inserer dans la DB
                ImageConverter converter = new ImageConverter();
                cmd.Parameters["@Nom"].Value = nom;
                cmd.Parameters["@Prenom"].Value = prenom;
                cmd.Parameters["@ImgFace"].Value = (byte[])converter.ConvertTo(face, typeof(byte[])); ;
                cmd.Parameters["@Horodatage"].Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //execution de la command
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Votre visage est sauvegardé!");
                }catch(Exception ex)
                {
                    MessageBox.Show("Votre visage n'a pas été sauvegardé!");
                }

                //fermeture de la connexion
                this.CloseConnection();
            }
        }
    }
}
