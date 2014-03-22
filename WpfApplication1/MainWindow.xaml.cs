using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        KinectSensor kinect;

        bool isWindowsClosing = false;
        const int MaxSkeletonTrackingCount = 6;
        Skeleton[] allSkeletons = new Skeleton[MaxSkeletonTrackingCount];

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelIsSkeletonTracked.Visibility = System.Windows.Visibility.Hidden;

            kinect = KinectSensor.KinectSensors[0];
            if (kinect == null)
            {
                return;
            }

            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };
            kinect.SkeletonStream.Enable(parameters);
            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
            //kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
            kinect.Start();

        }
/*
        private void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;

                image1.Source = BitmapSource.Create(colorFrame.Width,colorFrame.Height,96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }
        }
*/
        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            labelIsSkeletonTracked.Visibility = System.Windows.Visibility.Hidden;

            if (isWindowsClosing)
            {
                return;
            }
            // Skeleton s =Get
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                Skeleton[] skeletons = new Skeleton[0];
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }

                if (skeletons == null)
                {
                    return;
                }

                byte[] idSkeleton = new byte[2];
                byte count = 0;
                for (byte i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        idSkeleton[count] = i;
                        count++;
                        if (count > 1)
                            break;
                    }
                }

                Skeleton leftSkeleton = skeletons[0].Position.X < skeletons[1].Position.X ? skeletons[0] : skeletons[1];
                Skeleton rightSkeleton = skeletons[0].Position.X > skeletons[1].Position.X ? skeletons[0] : skeletons[1];

                if (leftSkeleton != null && rightSkeleton != null)
                {
                    labelIsSkeletonTracked.Visibility = System.Windows.Visibility.Visible;
                    mappingGesture2Keyboard(leftSkeleton,false);
                    mappingGesture2Keyboard(rightSkeleton, true);
                }
            }
        }

        private const double ArmRaisedThreadhold = 0.2;
        private const double ArmStretchedThreadhold = 0.4;
        private const double JumpDiffThreadhold = 0.05;
        private double headPreviousPosition = 1.6;

        private bool isRightActive = false;
        private bool isLeftActive = false;
        private bool isUpActive = false;
        private bool isDownActive = false;

        void mappingGesture2Keyboard(Skeleton s, bool isLeft)
        {
            SkeletonPoint head = s.Joints[JointType.Head].Position;
            SkeletonPoint leftshoulder = s.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint rightshoulder = s.Joints[JointType.ShoulderRight].Position;
            SkeletonPoint centerHip = s.Joints[JointType.HipCenter].Position;
            SkeletonPoint leftHand = s.Joints[JointType.HandLeft].Position;
            SkeletonPoint rightHand = s.Joints[JointType.HandRight].Position;

            bool isRightHandRaised = (rightHand.Y - head.Y) > ArmRaisedThreadhold;
            bool isLeftHandRaised = (leftHand.Y - head.Y) > ArmRaisedThreadhold;
            bool isRightHandStretched = (rightHand.X - rightshoulder.X) > ArmStretchedThreadhold;
            bool isLeftHandStretched = (leftshoulder.X - leftHand.X) > ArmStretchedThreadhold;


/*          if (isRightHandStretched)
                KeyboardToolkit.Keyboard.Type(Key.Right);

            if (isLeftHandStretched)
                KeyboardToolkit.Keyboard.Type(Key.Left);

            if (isLeftHandRaised && isRightHandRaised)
                KeyboardToolkit.Keyboard.Type(Key.Down);
*/
            if (isRightHandStretched)
            {
                if (!isRightActive && !isLeftActive)
                {
                    isRightActive = true;
                    if (!isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{Right}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{D}"); 
                }
            }
            else
            {
                isRightActive = false;
            }

            if (isLeftHandStretched)
            {
                if (!isLeftActive && !isRightActive)
                {
                    isLeftActive = true;
                    if (!isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{Left}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{A}");
                }
            }
            else
            {
                isLeftActive = false;
            }

            if (isLeftHandRaised && isRightHandRaised)
            {
                if (!isUpActive)
                {
                    isUpActive = true;
                    if (!isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{Up}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{W}");
                }
            }
            else
            {
                isUpActive = false;
            }

            if ((head.Y - headPreviousPosition) > JumpDiffThreadhold)
            {
                if (!isDownActive)
                {
                    isDownActive = true;
                    if (!isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{Down}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{S}");
                }
            }
            else
            {
                isDownActive = false;
            }

            headPreviousPosition = head.Y;
            
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            kinect.Stop();
        }
    }
}
