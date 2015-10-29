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
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;

namespace EmguFaceDetection
{
    public partial class Form1 : Form
    {
        //Variables, listas y Metodos de Deteccion de Rostros
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        public Form1()
        {
            InitializeComponent();
            //Cargar la detección de Rostros Metodo de Viola-Jones
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            eye = new HaarCascade("haarcascade_eye.xml");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Inicializar el dispositivo de Captura
            grabber = new Capture();
            grabber.QueryFrame();
            //Controlar el Evento de la Camara
            Application.Idle += new EventHandler(FrameGrabber);
            button1.Enabled = false;
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            //Numero inicial de rostros detectados
            label3.Text = "0";

            //Obtener el frame actual desde el disposiivo
            currentFrame = grabber.QueryFrame().Resize(425, 322, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convertir el frame a escala de grises
            gray = currentFrame.Convert<Gray, Byte>();

            //Detector de Rostros
            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
          face,
          1.2,
          10,
          Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
          new Size(20, 20));

            //Dibujar el ROI para cada rostro detectado
            foreach (MCvAvgComp f in facesDetected[0])
            {
                //Dibujar el ROI de color para identificar el rostro detectado
                currentFrame.Draw(f.rect, new Bgr(Color.OrangeRed), 2);

                //Colocar el numero actual de rostros detectados
                label3.Text = facesDetected[0].Length.ToString();

                //Ajustar el ROI para la detección de los ojos
                        
                gray.ROI = f.rect;
                MCvAvgComp[][] eyesDetected = gray.DetectHaarCascade(
                   eye,
                   1.1,
                   10,
                   Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                   new Size(20, 20));
                gray.ROI = Rectangle.Empty;

                foreach (MCvAvgComp ey in eyesDetected[0])
                {
                    Rectangle eyeRect = ey.rect;
                    eyeRect.Offset(f.rect.X, f.rect.Y);
                    currentFrame.Draw(eyeRect, new Bgr(Color.Green), 2);
                }
            }

            //Mostrar el video procesado
            imageBoxFrameGrabber.Image = currentFrame;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }
    }
}
