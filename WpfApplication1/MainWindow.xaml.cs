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
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

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

            //initialize kinect
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
            actionchooseviavoice(sender,e);

        }
        private SpeechRecognitionEngine speechEngine;
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        private void actionchooseviavoice(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.Sleep(400);
            KinectAudioSource source = kinect.AudioSource;
            source.EchoCancellationMode = EchoCancellationMode.None;
            source.AutomaticGainControlEnabled = false;
            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                MessageBox.Show("could not find kinect speech recognizer.");
                return;
            }

            this.speechEngine = new SpeechRecognitionEngine(ri.Id);

            var action = new Choices();
            action.Add(new SemanticResultValue("shoot", "SHOOT"));
            action.Add(new SemanticResultValue("attack", "ATTACK"));

            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(action);
            var g = new Grammar(gb);
            speechEngine.LoadGrammar(g);

            speechEngine.SpeechRecognized += SpeechRecognized;
            speechEngine.SpeechRecognitionRejected += SpeechRejected;

            speechEngine.SetInputToAudioStream(
                    kinect.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
        }



        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "SHOOT":
                        //System.Windows.Forms.SendKeys.SendWait("sdsdj");
                        KeyboardToolkit.Keyboard.Type(Key.S);
                        KeyboardToolkit.Keyboard.Type(Key.D);
                    //    KeyboardToolkit.Keyboard.Type(Key.S);
                   //     KeyboardToolkit.Keyboard.Type(Key.D);
                        KeyboardToolkit.Keyboard.Type(Key.K);
                        break;

                    case "ATTACK":
                        System.Windows.Forms.SendKeys.SendWait("{W}");
                        break;

                   
                }
            }
        }


        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
           
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

                    if (leftSkeleton != null && rightSkeleton != null)
                    {
                        labelIsSkeletonTracked1.Visibility = System.Windows.Visibility.Visible;
                        mappingGesture2Keyboard(leftSkeleton, true);
                        mappingGesture2Keyboard(rightSkeleton, false);
                    }
                }
                else if (count == 1)
                {
                    Skeleton leftSkeleton = skeletons[idSkeleton[0]];

                    if (leftSkeleton != null)
                    {
                        labelIsSkeletonTracked2.Visibility = System.Windows.Visibility.Visible;
                        mappingGesture2Keyboard(leftSkeleton, true);
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
