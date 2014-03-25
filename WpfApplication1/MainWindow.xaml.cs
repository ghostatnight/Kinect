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
            labelIsSkeletonTracked1.Visibility = System.Windows.Visibility.Hidden;
            labelIsSkeletonTracked2.Visibility = System.Windows.Visibility.Hidden;

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
            kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
            kinect.Start();

        }

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

        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            labelIsSkeletonTracked1.Visibility = System.Windows.Visibility.Hidden;
            labelIsSkeletonTracked2.Visibility = System.Windows.Visibility.Hidden;

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
                countText.Text = string.Format("{0}",count);

                if (count == 2)
                {
                    Skeleton leftSkeleton = skeletons[idSkeleton[0]].Position.X < skeletons[idSkeleton[1]].Position.X
                        ? skeletons[idSkeleton[0]] : skeletons[idSkeleton[1]];
                    Skeleton rightSkeleton = skeletons[idSkeleton[0]].Position.X >= skeletons[idSkeleton[1]].Position.X
                        ? skeletons[idSkeleton[0]] : skeletons[idSkeleton[1]];

                    if (leftSkeleton != null && rightSkeleton == null)
                    {
                        labelIsSkeletonTracked1.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (leftSkeleton == null && rightSkeleton != null)
                    {
                        labelIsSkeletonTracked2.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (leftSkeleton != null && rightSkeleton != null)
                    {
                        mappingGesture2Keyboard(leftSkeleton, true);
                        mappingGesture2Keyboard(rightSkeleton, false);
                    }
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            kinect.Stop();
        }
    }
}
