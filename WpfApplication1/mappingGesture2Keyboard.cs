﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.ComponentModel;
using Microsoft.Kinect;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private const double ArmRaisedThreadhold = 0.1;
        private const double ArmStretchedThreadhold = 0.4;
        private const double JumpDiffThreadhold = 0.05;
        private const double ArmExtendedThreadhold = 0.35;
        private const double HandkneeThreadhold = 0.05;
        private const double FootRaisedTheadhold = 0.2;
        private const double FootStretchedTheadhold = 0.3;
        //private double headPreviousPosition = 1.6;

        //private bool isRightActive = false;
        //private bool isLeftActive = false;
        private bool isUpActive = false;
        private bool isDownActive = false;
        private bool isRightHandExtendActive = false;
        private bool isLeftHandExtendActive = false;
        //private bool isRightFootExtendActive = false;
        //private bool isLeftFootExtendActive = false;
        private bool isRightFootRiseActive = false;
        private bool isLeftFootRiseActive = false;

        private void mappingGesture2Keyboard(Skeleton s, bool isLeft)
        {
            SkeletonPoint head = s.Joints[JointType.Head].Position;
            SkeletonPoint leftshoulder = s.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint rightshoulder = s.Joints[JointType.ShoulderRight].Position;
            SkeletonPoint centerHip = s.Joints[JointType.HipCenter].Position;
            SkeletonPoint leftHand = s.Joints[JointType.HandLeft].Position;
            SkeletonPoint rightHand = s.Joints[JointType.HandRight].Position;
            SkeletonPoint leftfoot = s.Joints[JointType.FootLeft].Position;
            SkeletonPoint rightfoot = s.Joints[JointType.FootRight].Position;
            SkeletonPoint leftknee = s.Joints[JointType.AnkleLeft].Position;
            SkeletonPoint rightknee = s.Joints[JointType.AnkleRight].Position;
            SkeletonPoint hipleft = s.Joints[JointType.HipLeft].Position;
            SkeletonPoint hipright = s.Joints[JointType.HipRight].Position;

            bool isRightHandRaised = (rightHand.Y - head.Y) > ArmRaisedThreadhold;
            bool isLeftHandRaised = (leftHand.Y - head.Y) > ArmRaisedThreadhold;
            bool isRightHandStretched = (rightHand.X - rightshoulder.X) > ArmStretchedThreadhold;
            bool isLeftHandStretched = (leftshoulder.X - leftHand.X) > ArmStretchedThreadhold;
            bool isRightHandExtended = (rightshoulder.Z - rightHand.Z) > ArmExtendedThreadhold;
            bool isLeftHandExtended = (leftshoulder.Z - leftHand.Z) > ArmExtendedThreadhold;
            bool isLeftKneeDown = (leftHand.Y - leftknee.Y) < HandkneeThreadhold;
            bool isRightKneeDown = (rightHand.Y - rightknee.Y) < HandkneeThreadhold;
            bool isLeftFootRised = (leftfoot.Y - rightfoot.Y) > FootRaisedTheadhold;
            bool isRightFootRised = (rightfoot.Y - leftfoot.Y) > FootRaisedTheadhold;
            bool isLeftFootStretched = (hipleft.X - leftfoot.X) > FootStretchedTheadhold;
            bool isRightFootStretched = (rightfoot.X - hipright.X) > FootStretchedTheadhold;


            /*          if (isRightHandStretched)
                            KeyboardToolkit.Keyboard.Type(Key.Right);

                        if (isLeftHandStretched)
                            KeyboardToolkit.Keyboard.Type(Key.Left);

                        if (isLeftHandRaised && isRightHandRaised)
                            KeyboardToolkit.Keyboard.Type(Key.Down);
            */
            if (isRightHandStretched)
            {
                if (isLeft)
                    KeyboardToolkit.Keyboard.Type(Key.D);
                //System.Windows.Forms.SendKeys.SendWait("{D}");
                else
                    System.Windows.Forms.SendKeys.SendWait("{Right}");
            }

            if (isLeftHandStretched)
            {
                if (isLeft)
                    //System.Windows.Forms.SendKeys.SendWait("{A}");
                    KeyboardToolkit.Keyboard.Type(Key.A);
                else
                    System.Windows.Forms.SendKeys.SendWait("{Left}");
            }

            if (isLeftHandRaised || isRightHandRaised)
            {
                if (!isUpActive)
                {
                    isUpActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{W}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{Up}");
                }
            }
            else
            {
                isUpActive = false;
            }
            // A new motion needed
            if (isLeftKneeDown && isRightKneeDown)
            {
                if (!isDownActive)
                {
                    isDownActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{D}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{Down}");
                }
            }
            else
            {
                isDownActive = false;
            }

            if (isLeftHandExtended)
            {
                if (!isRightHandExtendActive && !isLeftHandExtendActive)
                {
                    isLeftHandExtendActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{J}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{1}");
                }
            }
            else
            {
                isLeftHandExtendActive = false;
            }

            if (isRightHandExtended)
            {
                if (!isRightHandExtendActive && !isLeftHandExtendActive)
                {
                    isRightHandExtendActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{U}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{2}" + "{6}" + "{N}");
                }
            }
            else
            {
                isRightHandExtendActive = false;
            }

            if (isLeftFootRised)
            {
                if (!isLeftFootRiseActive && !isRightFootRiseActive)
                {
                    isLeftFootRiseActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{I}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{2}" + "{4}" + "{N}");
                }
            }
            else
            {
                isLeftFootRiseActive = false;
            }

            if (isRightFootRised)
            {
                if (!isLeftFootRiseActive && !isRightFootRiseActive)
                {
                    isRightFootRiseActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{K}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{6}" + "{2}" + "{N}");
                }
            }
            else
            {
                isRightFootRised = false;
            }

            if (isLeftFootStretched)
            {
                if (!isLeftFootRiseActive && !isRightFootRiseActive)
                {
                    isLeftFootRiseActive = true;
                    if (isLeft)
                        System.Windows.Forms.SendKeys.SendWait("{K}");
                    else
                        System.Windows.Forms.SendKeys.SendWait("{2}" + "{2}" + "{N}");
                }
            }
            else
            {
                isLeftFootRiseActive = false;
            }
        }
    }

}