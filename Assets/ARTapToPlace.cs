using Google.XR.ARCoreExtensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARTapToPlace : MonoBehaviour
{
    private const long RADIUS_OF_EARTH = 6371000;//�߰� ��ǥ ��꿡 �ʿ��� ������ ������
    private double[,] androidCoordi = new double[,] { {37.88398138901406, 127.73343466933132  },{ 37.8839871194505, 127.73343749702332 },{  37.883998228750364, 127.73340972144396 },{ 37.88425375637654, 127.73350692722941 },
                                                        {37.88454816862451, 127.73361801948475  },{  37.88463427012807, 127.73364023712753},{  37.884703706282956, 127.73362912496584 }, {37.88481480337433, 127.73357079364295  },
                                                        { 37.88502033277832, 127.73345135396714 },{37.8854452817081, 127.73336523807478}, {37.88545084371525, 127.7337457588266 } ,{37.885456403131876, 127.73398740333147 } ,
                                                        { 37.88545363572912, 127.73452624325257} ,{ 37.8854814148854, 127.73477344214757 } ,{ 37.885606404741395, 127.73500952805584}, { 37.88570084151001, 127.73518728684762} };//�ȵ���̵� �����κ��� �޾ƿ� ��ǥ�� ���� (�������� -> ���а�)
    // private double[,] androidCoordi = new double[,] { {37.88610080930872, 127.73595664937827}, { 37.886336901334275, 127.7364066014071 },{ 37.88641189540352, 127.73655658552666},{ 37.88652577502739, 127.73677045156494},{ 37.88679519873534, 127.73733983617593},{ 37.886839640131726, 127.7374537133786 },{ 37.8869090796549, 127.73762314033763},
    // {37.887072961906796, 127.73828974145636},{37.887072966156346, 127.73851749850172},{37.887070195490864 , 127.73888135435189},{37.88693410701028, 127.73924521416241}};//����� 6�� -> ���к���
    /*private double[,] androidCoordi = new double[,] { { 37.61265527436122, 126.84577904476235 },{ 37.6128635831008, 126.84575959621507 },{ 37.61285524552988, 126.84546795640372 } ,{ 37.612999672964925, 126.84545684223166},
                                                        {37.61299689466733, 126.84540962439775},{37.61299411632014, 126.84535962903959} ,{37.61298021669888,126.84466802589303},{37.612944087871085, 126.8434403611937},
                                                        {37.61294964079645, 126.84332926006708},{37.6129857465642, 126.84326537599071}, {37.61303296244744, 126.8432181567464} ,{37.61364955709515, 126.84320425172919},
                                                        {37.61392730254348, 126.84320424389298},{37.61438002757444, 126.8432014535955},{37.615382688591836, 126.84319864778104}, {37.61539103042753, 126.8437291546757}, 
                                                        {37.61539103166738, 126.84379859278171},{37.615418810526826, 126.84404023660692}, {37.615449364609056, 126.84415689176294}, {37.61557435621035, 126.84450130124203},
                                                        {37.615638239696764, 126.8446151779333}, {37.6157049001417, 126.84470127930378}, {37.61579099949465, 126.84460406352575}}; ���ſ� 1�� �ⱸ -> �Ҹ��������� 10���� ����Ʈ �� */
    private String[] description = { "3m �̵�", "��ȸ�� �� 167m �̵� ", "��ȸ�� �� �����ڵ��� �� ���� 169m �̵� ", "�����ڵ���, 169m", "����" };
    private List<double[]> allCoordi = new List<double[]>();//�Ϲ� ��ǥ�� �߰� ��ǥ�� ������ ����Ʈ
    private int interval = 1;//�Ϲ� ��ǥ�� ���̿� �ִ� �߰� ��ǥ���� ����
    private TextMeshProUGUI textP;//�ؽ�Ʈ�� ǥ���� GUI
    public AREarthManager earthManager;//ARSessionOrigin�� �ִ� ��ġ �ľ��� ����� ���� AREarthManager ������Ʈ
    private ARCoreExtensionsConfig config;//ARCoreExtensionConfig�� Geospatial ����� Ȯ���� ���� ������Ʈ(���� Ȯ�ο�)
    public ARSession ARSession;//ARCore�� ����ϱ� ���� �ʿ��� AR Session
    public ARSessionOrigin ARSessionOrigin;//ARCore�� ����ϱ� ���� �ʿ��� ARSessionOrigin
    public ARAnchorManager ARAnchorM;//AR ������Ʈ(AR ��Ŀ)�� �����ϴ� ARAnchorManager ������Ʈ
    private ARGeospatialAnchor[] anchor;
    // Start is called before the first frame update
    void Start()
    {
        //config = new ARCoreExtensionsConfig();
        //config.GeospatialMode = GeospatialMode.Enabled;
        earthManager = ARSessionOrigin.GetComponent<AREarthManager>();//ARSessionOrigin�� ��ϵ� AREarthManager ������Ʈ�� ������
        ARAnchorM = ARSessionOrigin.GetComponent<ARAnchorManager>();//ARSessionOrigin�� ��ϵ� ARAnchorManager ������Ʈ�� ������
        Debug.Log("LEN : " + androidCoordi.GetLength(0));//�Ϲ� ��ǥ�� ���� Ȯ��
        for (int i = 0; i < androidCoordi.GetLength(0) - 1; i++)//�Ϲ� ��ǥ���� 2���� �������� ������ ���� - 1 ������ �ݺ��� ����
        {
            MockLocation start = new MockLocation(androidCoordi[i, 0], androidCoordi[i, 1],0);//�Ϲ� ��ǥ ��ü1 ����
            MockLocation end = new MockLocation(androidCoordi[i + 1, 0], androidCoordi[i + 1, 1],0);//�Ϲ� ��ǥ ��ü2 ����

            double azimuth = calculateBearing(start, end);//�� ��ǥ ���� ���� ���
            //Debug.Log("Bearing: " + azimuth + ", sLAT: " + start.lat + ", sLONG: " + start.lng + ", eLAT: " + end.lat + ", eLONG: " + end.lng);
            List<MockLocation> coords = getLocations(interval, azimuth, start, end);//�� �Ϲ� ��ǥ ���̿� �ִ� �߰� ��ǥ�� interval ���� �������� ����� ����Ʈ�� ����
            foreach (MockLocation mockLocation in coords)
            {
                Debug.Log(mockLocation.lat + ", " + mockLocation.lng + ", " + mockLocation.rot);
                allCoordi.Add(new double[] { mockLocation.lat, mockLocation.lng, mockLocation.rot });//�Ϲ� ��ǥ 1, �߰���ǥ��, �Ϲ� ��ǥ2�� ������� ������ ���� ����Ʈ�� ���� 
            }
        }

        Debug.Log("ARR LENG: " + allCoordi.Count);
        anchor = new ARGeospatialAnchor[allCoordi.Count];//AR ������Ʈ(AR ��Ŀ)�� ������ �迭 ���� (���� == ��� ��ǥ���� ����)
    }

    // Update is called once per frame
    void Update()
    {
        GeoAR();
    }
    private void GeoAR()
    {
        var earthTrackingState = earthManager.EarthTrackingState;//����Ʈ���� ��ġ ���� ���¸� ����
        if (earthTrackingState == TrackingState.Tracking)//��ġ ������ ������ ���¸� AR ��� �۵�
        {
            //Debug.Log("Working 1" + (earthManager == null));
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;//����Ʈ���� ���� ��ġ ���� ����
            //Debug.Log("Working 1-1");
            for (int i = 0; i < allCoordi.Count; i++)//��� ��ǥ���� ������ŭ �ݺ�
            {
                if (getPathLength(new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude,0), new MockLocation(allCoordi[i][0], allCoordi[i][1],0)) < 20.0)// ����Ʈ ���� ��ǥ�� ����Ʈ���� 20���� �ȿ� �ִٸ� ����
                {
                    DestroyImmediate(anchor[i]);//�� ��ġ�� �������� AR ������Ʈ(AR ��Ŀ)�� ����°� �����ϱ� ���� ���� ��Ŀ ����
                    anchor[i] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, allCoordi[i][0], allCoordi[i][1], cameraGeospatialPose.Altitude - 2, Quaternion.AngleAxis(-((float)allCoordi[i][2] + 40.0f),Vector3.up));//������ ��ǥ�� ������� ���̺��� -2 ��ŭ ���� ���̿� AR ������Ʈ(AR ��Ŀ)����, AR ������Ʈ�� ���� �Ϲ� ��ǥ������ ȸ��
                }
                textP.text = "IS TRACKING:" + cameraGeospatialPose.Latitude + ", " + cameraGeospatialPose.Longitude + ", " + cameraGeospatialPose.Altitude;//�ؽ�Ʈ�� ���� ��ġ ��ǥ�� �� ǥ��
            }
        }
        else//��ġ ��ô �Ұ��� �� �ؽ�Ʈ�� ����
        {
            //Debug.Log("Working 11");
            textP.text = "NO TRACKING";
        }
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
}
