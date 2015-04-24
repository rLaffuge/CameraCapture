using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Drawing.Imaging;

namespace CameraCapture
{
    public partial class CameraCapture : Form
    {
        //déclaration des variables globales
        private Capture capture;                                                //prend les images depuis la camera
        private bool captureInProgress;                                         //boolean si la cam est en marche
        private CascadeClassifier detector;                                     //permet la detection
        private Rectangle face;
        private Image<Bgr, Byte> ImageFrame;
        private Image<Bgr, Byte> ImageFrameWithoutRect;
        private Bitmap faceBmp = new Bitmap(340, 235);
        private Boolean faceDetected;


        public CameraCapture()
        {
            InitializeComponent();
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            ImageFrame = capture.QueryFrame();                 //sauvegarde les images depuis la cam
            if (ImageFrame != null)
            {
                ImageFrameWithoutRect = ImageFrame.Copy();
            }
                
            if(ImageFrame != null){
                //conversion en noir et blanc
                Image<Gray, byte> grayframe = ImageFrame.Convert<Gray, byte>();

                //definition de la taille min des visages detectés
                Size min = new Size(250, 250);

                //on enregistre les visages dans un tableau
                var faces = detector.DetectMultiScale(grayframe, 1.1, 3,
                min,
                Size.Empty);

                if(faces.Length != 0)
                {
                    face = faces[0];
                    face.Inflate(0,50);
                    //on encadre le visages detectés
                    ImageFrame.Draw(face, new Bgr(Color.Green), 4);
                    faceDetected = true;
                }else
                {
                    faceDetected = false;
                }
            }

            CamImageBox.Image = ImageFrame;                                     //on le place dans l'imageBox pour l'afficher
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
            if(capture != null)
            {
                if (captureInProgress)
                {
                    if (!face.IsEmpty)
                    {

                        try{
                            faceBmp = ImageFrameWithoutRect.Bitmap.Clone(face, System.Drawing.Imaging.PixelFormat.DontCare);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Veuillez centrer votre visage");
                        }
                        CaptureImageBox.Image = faceBmp;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DBConnect db = new DBConnect();
            db.Insert(faceBmp, textBoxNom.Text, textBoxPrenom.Text);

            //faceBmp.Save("C:\\Users\\Rémy\\Desktop\\Photos\\" + textBoxNom.Text + "_" + textBoxPrenom.Text + ".png", ImageFormat.Png);
        }
    }
}
