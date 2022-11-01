using Google.XR.ARCoreExtensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARNavigation : MonoBehaviour
{
    private const long RADIUS_OF_EARTH = 6371000;//�߰� ��ǥ ��꿡 �ʿ��� ������ ������
    private List<double[]> tempAllPoints = new List<double[]>();
    private List<double[]> realAllPoints = new List<double[]>();
    private List<double[]> tempDescriptionPoints = new List<double[]>();
    private List<double[]> realDescriptionPoints = new List<double[]>();
    private List<String> realDescriptions = new List<String>();
    private double[] tempDescPoint = new double[2];
    private double[] tempAllPoint = new double[2];
    private int cntDescPInput = 0;
    private int cntAllPInput = 0;
    private int inputAllPlen = -1;
    private int inputDescPlen = -1;
    private int inputDesclen = -1;
    private bool initCoordinates = true;
    private List<MockLocation> tMLoc = new List<MockLocation>();
    private List<double[]> testAllPoint = new List<double[]> { new double[] { 37.88402044854169, 127.73341805335902 },new double[] { 37.88425375637654, 127.73350692722941 },
                                                               new double[] {37.88454816862451, 127.73361801948475},new double[] {37.88463427012807, 127.73364023712753},
                                                               new double[] {37.884703706282956, 127.73362912496584},new double[] {37.88481480337433 , 127.73357079364295},
                                                               new double[] {37.88502033277832, 127.73345135396714},new double[] {37.8854452817081, 127.73336523807478},
                                                               new double[] {37.88545084371525, 127.7337457588266},new double[] {37.885456403131876, 127.73398740333147},
                                                               new double[] {37.88545363572912, 127.73452624325257},new double[] {37.8854814148854, 127.73477344214757},
                                                               new double[] {37.885606404741395, 127.73500952805584},new double[] {37.885667394970724, 127.73510534595289},
                                                               new double[] {37.88571326146236, 127.73518287846719},new double[] {37.88582849235655, 127.73537085682028},
                                                               new double[] {37.88589489268473, 127.73547965765567} };
    //private List<double[]> testAllPoint = new List<double[]> { new double[] {37.88532862908614,127.73339023926337 }, new double[] { 37.8854452817081, 127.73336523807478 }, new double[] { 37.88543416805617, 127.73315970155917 }, new double[] { 37.88543138961729, 127.73310692866781 } };
    private List<double[]> testP = new List<double[]>();
    //private List<double[]> testDescriptionPoints = new List<double[]> { new double[] { 37.88532862908614, 127.73339023926337 }, new double[] { 37.8854452817081, 127.73336523807478 }, new double[] { 37.88543138961729, 127.73310692866781 } };
    private List<double[]> testDescriptionPoints = new List<double[]>{ new double[] { 37.88402044854169, 127.73341805335902 },new double[] { 37.8854452817081, 127.73336523807478 },
                                                                                           new double[] {37.885606404741395, 127.73500952805584},new double[] {37.88589489268473, 127.73547965765567}  };
    private List<MockLocation> descriptionPoints = new List<MockLocation>();
    //private List<String> testDescriptions = new List<String> { "���", "�׽�Ʈ �� 1", " �׽�Ʈ ��2", "����" };
    private List<String> testDescriptions = new List<String> { "���", "��ȸ�� �� 150m �̵�", " ���� 112m �̵�", "����" };
    private List<MockLocation> allCoordi = new List<MockLocation>();//�Ϲ� ��ǥ�� �߰� ��ǥ�� ������ ����Ʈ
    private int interval = 1;//�Ϲ� ��ǥ�� ���̿� �ִ� �߰� ��ǥ���� ����
    public TextMeshProUGUI textP;//�ؽ�Ʈ�� ǥ���� GUI
    public AREarthManager earthManager;//ARSessionOrigin�� �ִ� ��ġ �ľ��� ����� ���� AREarthManager ������Ʈ
    private ARCoreExtensionsConfig config;//ARCoreExtensionConfig�� Geospatial ����� Ȯ���� ���� ������Ʈ(���� Ȯ�ο�)
    public ARSession ARSession;//ARCore�� ����ϱ� ���� �ʿ��� AR Session
    public ARSessionOrigin ARSessionOrigin;//ARCore�� ����ϱ� ���� �ʿ��� ARSessionOrigin
    public ARAnchorManager ARAnchorM;//AR ������Ʈ(AR ��Ŀ)�� �����ϴ� ARAnchorManager ������Ʈ
    private ARGeospatialAnchor[] anchor;
    public GameObject goalObject;
    public GameObject pathObject;
    // Start is called before the first frame update
    void Start()
    {
        earthManager = ARSessionOrigin.GetComponent<AREarthManager>();//ARSessionOrigin�� ��ϵ� AREarthManager ������Ʈ�� ������
        ARAnchorM = ARSessionOrigin.GetComponent<ARAnchorManager>();//ARSessionOrigin�� ��ϵ� ARAnchorManager ������Ʈ�� ������

        calculateAllCoordinates();

       
    }

    // Update is called once per frame
    void Update()
    {
        GeoAR();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    private void GeoAR()
    {

        if (realAllPoints.Count == inputAllPlen && realDescriptionPoints.Count == inputDescPlen && realDescriptions.Count == inputDesclen && initCoordinates)
        {

            calculateAllCoordinates();
            initCoordinates = false;
            for (int i = 0; i < realDescriptionPoints.Count - 1; i++)
            {
                MockLocation loc1 = new MockLocation(realDescriptionPoints[i][0], realDescriptionPoints[i][1], 0);
                MockLocation loc2 = new MockLocation(realDescriptionPoints[i + 1][0], realDescriptionPoints[i + 1][1], 0);
                double r = calculateBearing(loc1, loc2);
                MockLocation loc = new MockLocation(realDescriptionPoints[i][0], realDescriptionPoints[i][1], r);
                descriptionPoints.Add(loc);
            }

            MockLocation locD = new MockLocation(realDescriptionPoints[realDescriptionPoints.Count - 1][0], realDescriptionPoints[realDescriptionPoints.Count - 1][1], 40);
            descriptionPoints.Add(locD);
            anchor = new ARGeospatialAnchor[descriptionPoints.Count];
        }
        var earthTrackingState = earthManager.EarthTrackingState;//����Ʈ���� ��ġ ���� ���¸� ����
        if (earthTrackingState == TrackingState.Tracking && !initCoordinates)//��ġ ������ ������ ���¸� AR ��� �۵�
        {
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;//����Ʈ���� ���� ��ġ ���� ����
            var isDescript = false;
            var isInPath = false;
            MockLocation user = new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude, 0);

            for (int i = 0; i < descriptionPoints.Count; i++)
            {
                var pathLength = getPathLength(user, descriptionPoints[i]);
                double aziDiff;
                double userHead;
                if (pathLength < 15.0)// ����Ʈ ���� ��ǥ�� ����Ʈ���� 20���� �ȿ� �ִٸ� ����
                {
                    if (cameraGeospatialPose.Heading > 0)
                    {
                        userHead = cameraGeospatialPose.Heading;
                    }
                    else
                    {
                        userHead = cameraGeospatialPose.Heading + 360;
                    }
                    aziDiff = userHead - calculateBearing(user, descriptionPoints[i]);
                    
                    if (aziDiff < 0)
                    {
                        aziDiff += 360;
                    }

                    if (aziDiff < 30 || aziDiff > 320)
                    {
                        textP.text = "���鿡 ����";
                    }
                    else if (aziDiff > 30 && aziDiff < 160)
                    {
                        textP.text = "���ʿ� ����";//�ؽ�Ʈ�� ���� ��ġ�� �ȳ� ���
                    }
                    else if (aziDiff > 230 && aziDiff < 320)
                    {
                        textP.text = "�����ʿ� ����";
                    }
                    else
                    {
                        textP.text = "�ڿ� ����";
                    }
                    DestroyImmediate(anchor[i]);//�� ��ġ�� �������� AR ������Ʈ(AR ��Ŀ)�� ����°� �����ϱ� ���� ���� ��Ŀ ����
                    if (i == descriptionPoints.Count -1)
                    {
                        ARAnchorM.anchorPrefab = goalObject;
                      
                    }
                    else
                    {
                        ARAnchorM.anchorPrefab = pathObject;
                    }
                    Quaternion rotation = Quaternion.Euler(new Vector3(-45,-1 * (float)descriptionPoints[i].rot, 90));
                    anchor[i] = ARAnchorM.AddAnchor(descriptionPoints[i].lat, descriptionPoints[i].lng, cameraGeospatialPose.Altitude + 1, rotation);
                    ARAnchorM.anchorPrefab.gameObject.transform.GetChild(3).gameObject.transform.GetComponentInChildren<TextMeshPro>().text = realDescriptions[i];
                    ARAnchorM.anchorPrefab.gameObject.transform.GetChild(4).gameObject.transform.GetComponentInChildren<TextMeshPro>().text = realDescriptions[i];
                   
                    isInPath = true;
                    isDescript = true;
                }
            }
            
            for (int j = 0; j < allCoordi.Count; j++)//��� ��ǥ���� ������ŭ �ݺ�
            {
                var pLen = getPathLength(user, allCoordi[j]);
                if (!isDescript && pLen < 15.0)
                {
                    textP.text = "�ȳ���";//�ؽ�Ʈ�� �����۵� ���
                    isInPath = true;
                }
            }
            if (!isInPath)
            {
                textP.text = "��� ���";
            }
        }
        else//��ġ ��ô �Ұ��� �� �ؽ�Ʈ�� ����
        {
            textP.text = "NO TRACKING";
        }

    }

    private void calculateAllCoordinates()
    {
        for (int i = 0; i < realAllPoints.Count - 1; i++)
        {
            MockLocation start = new MockLocation(realAllPoints[i][0], realAllPoints[i][1], 0);//�Ϲ� ��ǥ ��ü1 ����
            MockLocation end = new MockLocation(realAllPoints[i + 1][0], realAllPoints[i + 1][1], 0);//�Ϲ� ��ǥ ��ü2 ����
            double azimuth = calculateBearing(start, end);//�� ��ǥ ���� ���� ���
            //Debug.Log("Bearing: " + azimuth + ", sLAT: " + start.lat + ", sLONG: " + start.lng + ", eLAT: " + end.lat + ", eLONG: " + end.lng);
            List<MockLocation> coords = getLocations(interval, azimuth, start, end);//�� �Ϲ� ��ǥ ���̿� �ִ� �߰� ��ǥ�� interval ���� �������� ����� ����Ʈ�� ����
            foreach (MockLocation mockLocation in coords)
            {
                //Debug.Log("REAL MOCKPOINTS" + mockLocation.lat + ", " + mockLocation.lng + ", " + mockLocation.rot);
                allCoordi.Add(mockLocation);//�Ϲ� ��ǥ 1, �߰���ǥ��, �Ϲ� ��ǥ2�� ������� ������ ���� ����Ʈ�� ���� 
            }

        }

        //Debug.Log("P LEN: " + allCoordi.Count);
        /*for (int i = 0; i < realDescriptionPoints.Count; i++)
        {
            descriptionPoints.Add(new MockLocation(realDescriptionPoints[i][0], realDescriptionPoints[i][1], 0));
        }*/
        //anchor = new ARGeospatialAnchor[allCoordi.Count];//AR ������Ʈ(AR ��Ŀ)�� ������ �迭 ���� (���� == ��� ��ǥ���� ����)
    }



    private class MockLocation//��ǥ ��ü Ŭ����
    {
        public double lat;//��ǥ ��ü�� ����
        public double lng;//��ǥ ��ü�� �浵
        public double rot;//���� ��ǥ ��ü ������ ������
        public MockLocation(double lat, double lng, double rot)//��ǥ ��ü ������
        {
            this.lat = lat;//��ǥ ��ü�� ���� ����
            this.lng = lng;//��ǥ ��ü�� �浵 ����
            this.rot = rot;//���� ��ǥ ��ü ������ ������ ����
        }

        public string toString()
        {
            return "(" + lat + "," + lng + ")";
        }
    }
    private static List<MockLocation> getLocations(int interval, double azimuth, MockLocation start, MockLocation end)// �� �Ϲ� ��ǥ ������ �߰� ��ǥ�� ��� ����ϴ� �Լ�
    {

        double d = getPathLength(start, end);
        int dist = (int)d / interval;
        int coveredDist = interval;
        List<MockLocation> coords = new List<MockLocation>();
        coords.Add(new MockLocation(start.lat, start.lng, azimuth));
        for (int distance = 0; distance < dist; distance += interval)
        {
            MockLocation coord = getDestinationLatLng(start.lat, start.lng, azimuth, coveredDist);
            coveredDist += interval;
            coords.Add(coord);
        }
        return coords;
    }
    private static double getPathLength(MockLocation start, MockLocation end)// �� �Ϲ� ��ǥ ������ �Ÿ��� ����ϴ� �Լ�
    {

        double lat1Rads = Mathf.Deg2Rad * start.lat;
        double lat2Rads = Mathf.Deg2Rad * end.lat;
        double deltaLat = Mathf.Deg2Rad * (end.lat - start.lat);
        double deltaLng = Mathf.Deg2Rad * (end.lng - start.lng);

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2)
                + Math.Cos(lat1Rads) * Math.Cos(lat2Rads) * Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double d = RADIUS_OF_EARTH * c;
        return d;
    }
    private static MockLocation getDestinationLatLng(double lat, double lng, double azimuth, double distance)// �� �Ϲ� ��ǥ ���� �� ���� ��ǥ���� azimuth ������ distance �Ÿ���ŭ ������ �߰� ��ǥ 1���� ����ϴ� �Լ�
    {
        double radiusKm = RADIUS_OF_EARTH / 1000; // Radius of the Earth in km
        double brng = Mathf.Deg2Rad * azimuth; // Bearing is degrees converted to radians.
        double d = distance / 1000; // Distance m converted to km
        double lat1 = Mathf.Deg2Rad * lat; // Current dd lat point converted to radians
        double lon1 = Mathf.Deg2Rad * lng; // Current dd long point converted to radians
        double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(d / radiusKm) + Math.Cos(lat1) * Math.Sin(d / radiusKm) * Math.Cos(brng));
        double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(d / radiusKm) * Math.Cos(lat1),
                Math.Cos(d / radiusKm) - Math.Sin(lat1) * Math.Sin(lat2));
        // convert back to degrees

        lat2 = Mathf.Rad2Deg * (lat2);
        lon2 = Mathf.Rad2Deg * (lon2);
        return new MockLocation(lat2, lon2, azimuth);
    }

    private static double calculateBearing(MockLocation start, MockLocation end)//�� �Ϲ� ��ǥ ������ �������� ����ϴ� �Լ�
    {
        double startLat = Mathf.Deg2Rad * (start.lat);
        double startLong = Mathf.Deg2Rad * (start.lng);
        double endLat = Mathf.Deg2Rad * (end.lat);
        double endLong = Mathf.Deg2Rad * (end.lng);
        double dLong = endLong - startLong;
        double dPhi = Math.Log(Math.Tan((endLat / 2.0) + (Math.PI / 4.0)) / Math.Tan((startLat / 2.0) + (Math.PI / 4.0)));
        if (Math.Abs(dLong) > Math.PI)
        {
            if (dLong > 0.0)
            {
                dLong = -(2.0f * Math.PI - dLong);
            }
            else
            {
                dLong = (2.0f * Math.PI + dLong);
            }
        }
        var bearing = (Mathf.Rad2Deg * (Math.Atan2(dLong, dPhi)) + 360.0) % 360.0;
        return bearing;
    }

    public void getDescLen(String len)
    {
        inputDesclen = int.Parse(len);
    }
    public void getDescPointsLen(String len)
    {
        inputDescPlen = int.Parse(len);
    }
    public void getAllPointsLen(String len)
    {
        inputAllPlen = int.Parse(len);
    }

    public void getDescriptions(String description)
    {
        realDescriptions.Add(description);
    }

    public void getDescriptionPoints(String descriptionPoint)
    {
        if (cntDescPInput < 2)
        {
            tempDescPoint[cntDescPInput] = Double.Parse(descriptionPoint);
            cntDescPInput++;
        }
        else
        {
            tempDescriptionPoints.Add(tempDescPoint);
            realDescriptionPoints.Add(new double[] { tempDescriptionPoints[tempDescriptionPoints.Count - 1][0], tempDescriptionPoints[tempDescriptionPoints.Count - 1][1] });
            cntDescPInput = 0;
            tempDescPoint[cntDescPInput] = Double.Parse(descriptionPoint);
            cntDescPInput++;
        }

    }

    public void getAllPoints(String point)
    {
        if (cntAllPInput < 2)
        {
            tempAllPoint[cntAllPInput] = Double.Parse(point);
            cntAllPInput++;
        }
        else
        {
            tempAllPoints.Add(tempAllPoint);
            realAllPoints.Add(new double[] { tempAllPoints[tempAllPoints.Count - 1][0], tempAllPoints[tempAllPoints.Count - 1][1] });
            cntAllPInput = 0;
            tempAllPoint[cntAllPInput] = Double.Parse(point);
            cntAllPInput++;
        }

    }
}

