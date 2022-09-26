using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//ARFoundationとARCoreExtensions関連を使用する
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
namespace AR_Fukuoka
{
    public class SampleScript : MonoBehaviour
    {
        public AREarthManager EarthManager;
        public VpsInitializer Initializer;
        public Text OutputText;
        // Start is called before the first frame update
        void Start()
        {

        }
        // Update is called once per frame
        void Update()
        {
            string status = "";
            if(!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
            {
                return;
            }
            GeospatialPose pose = EarthManager.CameraGeospatialPose;
            ShowTrackingInfo(status, pose);
        }       
        void ShowTrackingInfo(string status, GeospatialPose pose)
        {
            OutputText.text = string.Format("Latitude/Longtitude: {0},{1}\n" + "Horizon Accuracy: {2}m\n" + "Altitude: {3}m\n" + "Vertical Accuracy: {4}m\n" + "Heading: {5}\n" + "Heading Accuracy:{6}\n" + "{7}\n",
                                            pose.Latitude.ToString("F6"), pose.Longitude.ToString("F6"), pose.HorizontalAccuracy.ToString("F6"), pose.Altitude.ToString("F2"), pose.VerticalAccuracy.ToString("F2"),
                                             pose.Heading.ToString("F1"), pose.HeadingAccuracy.ToString("F1"), status);
        }
    }
}


