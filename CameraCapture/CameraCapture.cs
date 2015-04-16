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
        public CameraCapture()
        {
            InitializeComponent();
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame();                 //sauvegarde les images depuis la cam
            CamImageBox.Image = ImageFrame;                                     //on le place dans l'imageBox pour l'afficher
            //POur sauvegarder l'image > ImageFrame.Save(@"E:\MyPic.jpg");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            #region if capture is not created, create it now
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
            #endregion

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
    }
}
