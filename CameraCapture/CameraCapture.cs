using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //déclaration des variables globales
        private Capture capture;                                                //prend les images depuis la camera
        private bool captureInProgress;                                         //boolean si la cam est en marche
        private CascadeClassifier detector;                                     //permet la detection
        public CameraCapture()
        {
            InitializeComponent();
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame();                 //sauvegarde les images depuis la cam

            if(ImageFrame != null){
                //conversion en noir et blanc
                Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();

                //definition de la taille min des visages detectés
                Size min = new Size(250, 250);

                //on enregistre les visages dans un tableau
                var faces = detector.DetectMultiScale(grayframe, 1.1, 3,
                    min,
                    Size.Empty);

                foreach(var face in faces)
                {
                    //on encadre les visages detectés
                    ImageFrame.Draw(face, new Bgr(Color.Green), 3);
                }
            }

            CamImageBox.Image = ImageFrame;                                     //on le place dans l'imageBox pour l'afficher
            //Pour sauvegarder l'image > ImageFrame.Save(@"E:\MyPic.jpg");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // si capture n'existe pas, on le créer
            if(capture == null)
            {
                try
                {
                    capture = new Capture();
                }
                catch(NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
            
            if(capture != null)
            {
                if (captureInProgress)
                {
                    //si la camera est en marche on la stop et change le text du button
                    btnStart.Text = "Start";
                    Application.Idle -= ProcessFrame;
                }
                else
                {
                    //si la camera n'est pas en marche, demarrage de la cam et changement du text du button
                    btnStart.Text = "Stop";
                    Application.Idle += ProcessFrame;
                }
                captureInProgress = !captureInProgress;
            }
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        private void CameraCapture_Load(object sender, EventArgs e)
        {
            detector = new CascadeClassifier("C:\\Users\\Rémy\\Documents\\visual studio 2015\\Projects\\CameraCapture\\CameraCapture\\bin\\Debug\\haarcascade_frontalface_default.xml");
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            Image<Bgr, Byte> ImageFrameToCapture = capture.QueryFrame();

            if (ImageFrameToCapture != null)
            {
                //conversion en noir et blanc
                Image<Gray, byte> grayframe = ImageFrameToCapture.Convert<Gray, byte>();

                CaptureImageBox.Image =  ImageFrameToCapture;
                ImageFrameToCapture.Save(@"C:\Users\Rémy\Desktop\Photos\face" + DateTime.Now.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
