using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using TMPro;
public class ARTapToPlace : MonoBehaviour
{
    private const long RADIUS_OF_EARTH = 6371000;
    private double[,] androidCoordi = new double[,] { { 37.883981, 127.733434 },{ 37.883987, 127.733437 }, { 37.883998, 127.733409 }, { 37.884253, 127.733506 } ,{ 37.884548, 127.733618 } ,{ 37.884634, 127.733640 },
                                                       {37.884703, 127.733629 },{37.884814, 127.733570 },{37.885020, 127.733451 },{37.885445, 127.733365 },{37.885450, 127.733745 },{37.885456, 127.733987 },{37.885453, 127.734526 },
                                                        {37.885481, 127.734773 },{37.885606, 127.735009 },{37.885700, 127.735187 }};
    private List<double[]> allCoordi = new List<double[]>();
    private int interval = 1;
    public GameObject tt;
    private TextMeshProUGUI textP;
    public AREarthManager earthManager;
    private ARCoreExtensionsConfig config;
    public ARSession ARSession;
    public ARSessionOrigin ARSessionOrigin;
    public ARAnchorManager ARAnchorM;
    private GameObject[] placeObject;
    private ARGeospatialAnchor[] anchor;
    private LineRenderer[] lines = new LineRenderer[15];
    private bool setAnchor = true;
    public GameObject t1, test;
    double[] len;
    // Start is called before the first frame update
    void Start()
    {
        textP = tt.GetComponent<TextMeshProUGUI>();
        config = new ARCoreExtensionsConfig();
        config.GeospatialMode = GeospatialMode.Enabled;
        earthManager = ARSessionOrigin.GetComponent<AREarthManager>();
        ARAnchorM = ARSessionOrigin.GetComponent<ARAnchorManager>();

        for(int i = 0; i < androidCoordi.GetLength(0) - 1; i++)
        {
            MockLocation start = new MockLocation(androidCoordi[i, 0], androidCoordi[i, 1]);
            MockLocation end = new MockLocation(androidCoordi[i + 1, 0], androidCoordi[i + 1, 1]);
            
            double azimuth = calculateBearing(start, end);
            List<MockLocation> coords = getLocations(interval, azimuth, start, end);
            foreach (MockLocation mockLocation in coords)
            {
                Debug.Log(mockLocation.lat + ", " + mockLocation.lng);
                allCoordi.Add(new double[]{mockLocation.lat ,mockLocation.lng});
            }
            //allCoordi.Add(new double[] { androidCoordi[i,0], androidCoordi[i,1] });
        }
        
        Debug.Log("ARR LENG: " + allCoordi.Count);
        anchor = new ARGeospatialAnchor[allCoordi.Count];
        placeObject = new GameObject[allCoordi.Count];
        len = new double[allCoordi.Count];
    }

    // Update is called once per frame
    void Update()
    {
            GeoAR();
    }
    private void GeoAR()
    {
        int cnt = 0;
        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            Debug.Log("Working 1" + (earthManager == null));
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;
            Debug.Log("Working 1-1");
            /*if (setAnchor)
            {
                for (int i = 0; i < allCoordi.Count; i++)
                {
                    Destroy(anchor[i]);
                    anchor[i] = ARAnchorM.AddAnchor(allCoordi[i][0], allCoordi[i][1], 5, Quaternion.identity);       
                    Debug.Log("Working 1-2 ww ");
                
                }
                setAnchor = false;
            }*/
            /*for (int i = 0; i < allCoordi.Count; i++)
            {

                len[i] = getPathLength(new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude), new MockLocation(allCoordi[i][0], allCoordi[i][1]));

                if (getPathLength(new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude), new MockLocation(allCoordi[i][0], allCoordi[i][1])) < 20.0f)
                {
                    Destroy(anchor[i]);
                    anchor[i] = ARAnchorM.AddAnchor(allCoordi[i][0], allCoordi[i][1], cameraGeospatialPose.Altitude - 2, Quaternion.identity);
                    if (anchor[i] != null)
                    {
                        Destroy(placeObject[i]);
                        placeObject[i] = Instantiate(test, anchor[i].transform);
                        anchor[i].gameObject.SetActive(true);
                        cnt++;
                        textP.text = "IS TRACKING:" + cameraGeospatialPose.Latitude + ", " + cameraGeospatialPose.Longitude + ", " + cameraGeospatialPose.Altitude + ", " + allCoordi.Count + "/" + i + " Instantiated, " + cnt + "anchors";
                    }
                    else
                    {
                        textP.text = "IS TRACKING ANCHOR NOT MARKED";
                    }
                }*/
            /*for (int i = 0; i < allCoordi.Count; i++)
            {
                DestroyImmediate(placeObject[i]);
                DestroyImmediate(anchor[i]);
                anchor.Clear();
            }
            for (int i = 0; i < allCoordi.Count; i++)
            {
                if(getPathLength(new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude), new MockLocation(allCoordi[i][0], allCoordi[i][1])) < 20.0)
                {
                    anchor.Add(ARAnchorManagerExtensions.AddAnchor(ARAnchorM, allCoordi[i][0], allCoordi[i][1], cameraGeospatialPose.Altitude - 2, Quaternion.identity));
                }     
            }
            for(int i = 0; i < anchor.Count; i++)
            {
                placeObject.Add(Instantiate(test, anchor[i].transform));
                anchor[i].gameObject.SetActive(true);
            }*/
            /*for (int i = 0; i < 16; i++)
            {
                DestroyImmediate(placeObject[i]);
                DestroyImmediate(anchor[i]);
            }
            var alti = cameraGeospatialPose.Altitude - 2;
            anchor[0] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM,37.88398138901406, 127.73343466933132, alti, Quaternion.identity);
            anchor[1] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.8839871194505, 127.73343749702332, alti, Quaternion.identity);
            anchor[2] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.883998228750364, 127.73340972144396, alti, Quaternion.identity);
            anchor[3] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88425375637654, 127.73350692722941, alti, Quaternion.identity);
            anchor[4] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88454816862451, 127.73361801948475, alti, Quaternion.identity);
            anchor[5] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88463427012807, 127.73364023712753, alti, Quaternion.identity);
            anchor[6] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.884703706282956, 127.73362912496584, alti, Quaternion.identity);
            anchor[7] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88481480337433, 127.73357079364295, alti, Quaternion.identity);
            anchor[8] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88502033277832, 127.73345135396714, alti, Quaternion.identity);
            anchor[9] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.8854452817081, 127.73336523807478, alti, Quaternion.identity);
            anchor[10] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88545084371525, 127.7337457588266, alti, Quaternion.identity);
            anchor[11] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.885456403131876, 127.73398740333147, alti, Quaternion.identity);
            anchor[12] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88545363572912, 127.73452624325257, alti, Quaternion.identity);
            anchor[13] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.8854814148854, 127.73477344214757, alti, Quaternion.identity);
            anchor[14] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.885606404741395, 127.73500952805584, alti, Quaternion.identity);
            anchor[15] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, 37.88570084151001, 127.73518728684762, alti, Quaternion.identity);
            for(int i = 0; i < 16; i++)
            {
                placeObject[i] = Instantiate(test, anchor[i].transform);
                anchor[i].gameObject.SetActive(false);
            }*/
            for (int i = 0; i < allCoordi.Count; i++)
            {
                if (getPathLength(new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude), new MockLocation(allCoordi[i][0], allCoordi[i][1])) < 20.0f)
                {
                    DestroyImmediate(anchor[i]);
                    anchor[i] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, allCoordi[i][0], allCoordi[i][1], cameraGeospatialPose.Altitude - 2, Quaternion.identity);
                }
                textP.text = "IS TRACKING:" + cameraGeospatialPose.Latitude + ", " + cameraGeospatialPose.Longitude + ", " + cameraGeospatialPose.Altitude;
            }
        }
        else
        {
            Debug.Log("Working 11");
            textP.text = "NO TRACKING";
        }
    }
    private class MockLocation
    {
        public double lat;
        public double lng;

        public MockLocation(double lat, double lng)
        {
            this.lat = lat;
            this.lng = lng;
        }

        public string toString()
        {
            return "(" + lat + "," + lng + ")";
        }
    }
    private static List<MockLocation> getLocations(int interval, double azimuth, MockLocation start, MockLocation end)
    {

        double d = getPathLength(start, end);
        int dist = (int)d / interval;
        int coveredDist = interval;
        List<MockLocation> coords = new List<MockLocation>();
        coords.Add(new MockLocation(start.lat, start.lng));
        for (int distance = 0; distance < dist; distance += interval)
        {
            MockLocation coord = getDestinationLatLng(start.lat, start.lng, azimuth, coveredDist);
            coveredDist += interval;
            coords.Add(coord);
        }
        //coords.Add(new MockLocation(end.lat, end.lng));

        return coords;
    }
    private static double getPathLength(MockLocation start, MockLocation end)
    {
        
        float lat1Rads = Mathf.Deg2Rad * (float)start.lat;
        float lat2Rads = Mathf.Deg2Rad * (float)end.lat;
        float deltaLat = Mathf.Deg2Rad * ((float)end.lat - (float)start.lat);

        float deltaLng = Mathf.Deg2Rad * ((float)end.lng - (float)start.lng);
        
        float a = Mathf.Sin(deltaLat / 2) * Mathf.Sin(deltaLat / 2)
                + Mathf.Cos(lat1Rads) * Mathf.Cos(lat2Rads) * Mathf.Sin(deltaLng / 2) * Mathf.Sin(deltaLng / 2);
        double c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        double d = RADIUS_OF_EARTH * c;
        return d;
    }
    private static MockLocation getDestinationLatLng(double lat, double lng, double azimuth, double distance)
    {
        float radiusKm = RADIUS_OF_EARTH / 1000; // Radius of the Earth in km
        float brng = Mathf.Deg2Rad * (float)azimuth; // Bearing is degrees converted to radians.
        float d = (float)distance / 1000; // Distance m converted to km
        float lat1 = Mathf.Deg2Rad * (float)lat; // Current dd lat point converted to radians
        float lon1 = Mathf.Deg2Rad * (float)lng; // Current dd long point converted to radians
        float lat2 = Mathf.Asin(Mathf.Sin(lat1) * Mathf.Cos(d / radiusKm) + Mathf.Cos(lat1) * Mathf.Sin(d / radiusKm) * Mathf.Cos(brng));
        double lon2 = lon1 + Mathf.Atan2(Mathf.Sin(brng) * Mathf.Sin(d / radiusKm) * Mathf.Cos(lat1),
                Mathf.Cos(d / radiusKm) - Mathf.Sin(lat1) * Mathf.Sin(lat2));
        // convert back to degrees
        
        lat2 = Mathf.Rad2Deg * (lat2);
        lon2 = Mathf.Rad2Deg * (lon2);
        return new MockLocation(lat2, lon2);
    }

    private static double calculateBearing(MockLocation start, MockLocation end)
    {
        float startLat = Mathf.Deg2Rad * (float)(start.lat);
        float startLong = Mathf.Deg2Rad * (float)(start.lng);
        float endLat = Mathf.Deg2Rad * (float)(end.lat);
        float endLong = Mathf.Deg2Rad * (float)(end.lng);
        float dLong = endLong - startLong;
        float dPhi = Mathf.Log(Mathf.Tan((endLat / 2.0f) + (Mathf.PI / 4.0f)) / Mathf.Tan((startLat / 2.0f) + (Mathf.PI / 4.0f)));
        if (Mathf.Abs(dLong) > Mathf.PI)
        {
            if (dLong > 0.0)
            {
                dLong = -(2.0f * Mathf.PI - dLong);
            }
            else
            {
                dLong = (2.0f * Mathf.PI + dLong);
            }
        }
        double bearing = (Mathf.Deg2Rad * (Mathf.Atan2(dLong, dPhi)) + 360.0) % 360.0;
        return bearing;
    }
}
