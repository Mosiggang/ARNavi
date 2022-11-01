using Google.XR.ARCoreExtensions;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARTapToPlace : MonoBehaviour
{
    private const long RADIUS_OF_EARTH = 6371000;//추가 좌표 계산에 필요한 지구의 반지름
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
    private List<double[]> testAllPoint = new List<double[]> { new double[] { 37.88402044854169, 127.73341805335902 },new double[] { 37.88425375637654, 127.73350692722941 },
                                                               new double[] {37.88454816862451, 127.73361801948475},new double[] {37.88463427012807, 127.73364023712753},
                                                               new double[] {37.884703706282956, 127.73362912496584},new double[] {37.88481480337433 , 127.73357079364295},
                                                               new double[] {37.88502033277832, 127.73345135396714},new double[] {37.8854452817081, 127.73336523807478},
                                                               new double[] {37.88545084371525, 127.7337457588266},new double[] {37.885456403131876, 127.73398740333147},
                                                               new double[] {37.88545363572912, 127.73452624325257},new double[] {37.8854814148854, 127.73477344214757},
                                                               new double[] {37.885606404741395, 127.73500952805584},new double[] {37.885667394970724, 127.73510534595289},
                                                               new double[] {37.88571326146236, 127.73518287846719},new double[] {37.88582849235655, 127.73537085682028},
                                                               new double[] {37.88589489268473, 127.73547965765567} };
    private List<double[]> testP = new List<double[]>();
    private List<double[]> testDescriptionPoints = new List<double[]>{ new double[] { 37.88402044854169, 127.73341805335902 },new double[] { 37.8854452817081, 127.73336523807478 },
                                                                                           new double[] {37.885606404741395, 127.73500952805584},new double[] {37.88589489268473, 127.73547965765567}  };
    private List<MockLocation> descriptionPoints = new List<MockLocation>();
    private List<String> testDescriptions = new List<String> { "출발", "우회전 후 150m 이동", " 직진 112m 이동", "도착" };
    private double[,] androidCoordi = new double[,] { {37.88398138901406, 127.73343466933132  },{ 37.8839871194505, 127.73343749702332 },{  37.883998228750364, 127.73340972144396 },{ 37.88425375637654, 127.73350692722941 },
                                                        {37.88454816862451, 127.73361801948475  },{  37.88463427012807, 127.73364023712753},{  37.884703706282956, 127.73362912496584 }, {37.88481480337433, 127.73357079364295  },
                                                        { 37.88502033277832, 127.73345135396714 },{37.8854452817081, 127.73336523807478}, {37.88545084371525, 127.7337457588266 } ,{37.885456403131876, 127.73398740333147 } ,
                                                        { 37.88545363572912, 127.73452624325257} ,{ 37.8854814148854, 127.73477344214757 } ,{ 37.885606404741395, 127.73500952805584}, { 37.88570084151001, 127.73518728684762} };//안드로이드 앱으로부터 받아올 좌표들 예시 (동보빌라 -> 공학관)
    // private double[,] androidCoordi = new double[,] { {37.88610080930872, 127.73595664937827}, { 37.886336901334275, 127.7364066014071 },{ 37.88641189540352, 127.73655658552666},{ 37.88652577502739, 127.73677045156494},{ 37.88679519873534, 127.73733983617593},{ 37.886839640131726, 127.7374537133786 },{ 37.8869090796549, 127.73762314033763},
    // {37.887072961906796, 127.73828974145636},{37.887072966156346, 127.73851749850172},{37.887070195490864 , 127.73888135435189},{37.88693410701028, 127.73924521416241}};//기숙사 6관 -> 대학본부
    /*private double[,] androidCoordi = new double[,] { { 37.61265527436122, 126.84577904476235 },{ 37.6128635831008, 126.84575959621507 },{ 37.61285524552988, 126.84546795640372 } ,{ 37.612999672964925, 126.84545684223166},
                                                        {37.61299689466733, 126.84540962439775},{37.61299411632014, 126.84535962903959} ,{37.61298021669888,126.84466802589303},{37.612944087871085, 126.8434403611937},
                                                        {37.61294964079645, 126.84332926006708},{37.6129857465642, 126.84326537599071}, {37.61303296244744, 126.8432181567464} ,{37.61364955709515, 126.84320425172919},
                                                        {37.61392730254348, 126.84320424389298},{37.61438002757444, 126.8432014535955},{37.615382688591836, 126.84319864778104}, {37.61539103042753, 126.8437291546757}, 
                                                        {37.61539103166738, 126.84379859278171},{37.615418810526826, 126.84404023660692}, {37.615449364609056, 126.84415689176294}, {37.61557435621035, 126.84450130124203},
                                                        {37.615638239696764, 126.8446151779333}, {37.6157049001417, 126.84470127930378}, {37.61579099949465, 126.84460406352575}}; 강매역 1번 출구 -> 소만마을동성 10단지 아파트 상가 */
    private List<MockLocation> allCoordi = new List<MockLocation>();//일반 좌표와 추가 좌표를 저장할 리스트
    private int interval = 1;//일반 좌표들 사이에 있는 추가 좌표들의 간격
    public TextMeshProUGUI textP;//텍스트를 표시할 GUI
    public AREarthManager earthManager;//ARSessionOrigin에 있는 위치 파악을 기능을 위한 AREarthManager 컴포넌트
    private ARCoreExtensionsConfig config;//ARCoreExtensionConfig의 Geospatial 기능을 확인할 가진 컴포넌트(설정 확인용)
    public ARSession ARSession;//ARCore를 사용하기 위해 필요한 AR Session
    public ARSessionOrigin ARSessionOrigin;//ARCore를 사용하기 위해 필요한 ARSessionOrigin
    public ARAnchorManager ARAnchorM;//AR 오브젝트(AR 앵커)를 관리하는 ARAnchorManager 컴포넌트
    private ARGeospatialAnchor[] anchor;

    public GameObject pre;

    // Start is called before the first frame update
    void Start()
    {
        earthManager = ARSessionOrigin.GetComponent<AREarthManager>();//ARSessionOrigin에 등록된 AREarthManager 컴포넌트를 가져옴
        ARAnchorM = ARSessionOrigin.GetComponent<ARAnchorManager>();//ARSessionOrigin에 등록된 ARAnchorManager 컴포넌트를 가져옴
        
        
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
        Debug.Log("URP: " + realAllPoints.Count + ", " + inputAllPlen);
        Debug.Log("UDP: " + realDescriptionPoints.Count + ", " + inputDescPlen);
        Debug.Log("UDS: " + realDescriptions.Count + ", " + inputDesclen);
        if (realAllPoints.Count == inputAllPlen && realDescriptionPoints.Count == inputDescPlen && realDescriptions.Count == inputDesclen && initCoordinates)
        {
            Debug.Log("R P: " + realAllPoints.Count);
            Debug.Log("D P: " + realDescriptionPoints.Count);
            Debug.Log("D S: " + realDescriptions.Count);

            for (int i = 0; i < realAllPoints.Count; i++)
            {
                Debug.Log("Real Point " + i + ": " + realAllPoints[i][0] + ", " + realAllPoints[i][1]);
            }
            for (int i = 0; i < realDescriptionPoints.Count; i++)
            {
                Debug.Log("Desc Point " + i + ": " + realDescriptionPoints[i][0] + ", " + realDescriptionPoints[i][1]);
                Debug.Log("Desc String " + i + ": " + realDescriptions[i]);
            }
            calculateAllCoordinates();
            initCoordinates = false;
        }
        var earthTrackingState = earthManager.EarthTrackingState;//스마트폰의 위치 추적 상태를 저장
        if (earthTrackingState == TrackingState.Tracking && !initCoordinates)//위치 추적이 가능한 상태면 AR 기능 작동
        {
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;//스마트폰의 현재 위치 정보 저장
            var isDescript = false;
            var isInPath = false;
            MockLocation user = new MockLocation(cameraGeospatialPose.Latitude, cameraGeospatialPose.Longitude, 0);
            for (int i = 0; i < descriptionPoints.Count; i++)
            {
                if (getPathLength(user, descriptionPoints[i]) < 10.0)
                {
                    textP.text = realDescriptions[i];//텍스트로 현재 위치의 안내 출력
                    isDescript = true;
                    break;
                }
            }
            for (int j = 0; j < allCoordi.Count; j++)//모든 좌표들의 개수만큼 반복
            {
                var pathLength = getPathLength(user, allCoordi[j]);
                if (pathLength < 10)
                {
                    isInPath = true;
                }
                if (pathLength < 15.0)// 리스트 안의 좌표가 스마트폰과 20미터 안에 있다면 실행
                {
                    DestroyImmediate(anchor[j]);//한 위치에 여러개의 AR 오브젝트(AR 앵커)가 생기는걸 방지하기 위해 기존 앵커 삭제
                    anchor[j] = ARAnchorM.AddAnchor(allCoordi[j].lat, allCoordi[j].lng, cameraGeospatialPose.Altitude - 2, Quaternion.AngleAxis(-((float)allCoordi[j].rot + 40.0f), Vector3.up));
                    //anchor[j] = ARAnchorManagerExtensions.AddAnchor(ARAnchorM, allCoordi[j].lat, allCoordi[j].lng, cameraGeospatialPose.Altitude - 2, Quaternion.AngleAxis(-((float)allCoordi[j].rot + 40.0f),Vector3.up));//정해진 좌표에 사용자의 높이보다 -2 만큼 낮은 높이에 AR 오브젝트(AR 앵커)생성, AR 오브젝트를 다음 일반 좌표쪽으로 회전
                }
                if (!isDescript)
                {
                    textP.text = "안내중";//텍스트로 정상작동 출력
                }
            }
            if (!isInPath)
            {
                textP.text = "경로 이탈 다시 돌아가주세요";
            }

        }
        else//위치 추척 불가능 시 텍스트만 변경
        {
            textP.text = "NO TRACKING";
        }
       
    }

    private void calculateAllCoordinates()
    {
        for (int i = 0; i < realAllPoints.Count - 1; i++)
        {
            MockLocation start = new MockLocation(realAllPoints[i][0], realAllPoints[i][1], 0);//일반 좌표 객체1 생성
            MockLocation end = new MockLocation(realAllPoints[i + 1][0], realAllPoints[i + 1][1], 0);//일반 좌표 객체2 생성
            double azimuth = calculateBearing(start, end);//두 좌표 사이 각도 계산
            //Debug.Log("Bearing: " + azimuth + ", sLAT: " + start.lat + ", sLONG: " + start.lng + ", eLAT: " + end.lat + ", eLONG: " + end.lng);
            List<MockLocation> coords = getLocations(interval, azimuth, start, end);//두 일반 좌표 사이에 있는 추가 좌표를 interval 미터 간격으로 계산해 리스트에 저장
            foreach (MockLocation mockLocation in coords)
            {
                Debug.Log("REAL MOCKPOINTS" + mockLocation.lat + ", " + mockLocation.lng + ", " + mockLocation.rot);
                allCoordi.Add(mockLocation);//일반 좌표 1, 추가좌표들, 일반 좌표2의 방법으로 순서를 지켜 리스트에 저장 
            }

        }

        Debug.Log("P LEN: " + allCoordi.Count);
        for (int i = 0; i < realDescriptionPoints.Count; i++)
        {
            descriptionPoints.Add(new MockLocation(realDescriptionPoints[i][0], realDescriptionPoints[i][1], 0));
        }
        anchor = new ARGeospatialAnchor[allCoordi.Count];//AR 오브젝트(AR 앵커)를 저장할 배열 생성 (길이 == 모든 좌표들의 개수)
    }



    private class MockLocation//좌표 객체 클래스
    {
        public double lat;//좌표 객체의 위도
        public double lng;//좌표 객체의 경도
        public double rot;//다음 좌표 객체 사이의 방위각
        public MockLocation(double lat, double lng, double rot)//좌표 객체 생성자
        {
            this.lat = lat;//좌표 객체의 위도 설정
            this.lng = lng;//좌표 객체의 경도 설정
            this.rot = rot;//다음 좌표 객체 사이의 방위각 설정
        }

        public string toString()
        {
            return "(" + lat + "," + lng + ")";
        }
    }
    private static List<MockLocation> getLocations(int interval, double azimuth, MockLocation start, MockLocation end)// 두 일반 좌표 사이의 추가 좌표를 모두 계산하는 함수
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
    private static double getPathLength(MockLocation start, MockLocation end)// 두 일반 좌표 사이의 거리를 계산하는 함수
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
    private static MockLocation getDestinationLatLng(double lat, double lng, double azimuth, double distance)// 두 일반 좌표 사이 중 시작 좌표에서 azimuth 각도로 distance 거리만큼 떨어진 추가 좌표 1개를 계산하는 함수
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

    private static double calculateBearing(MockLocation start, MockLocation end)//두 일반 좌표 사이의 방위각을 계산하는 함수
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
            realAllPoints.Add(new double[] { tempAllPoints[tempAllPoints.Count - 1][0] , tempAllPoints[tempAllPoints.Count - 1][1] });
            cntAllPInput = 0;
            tempAllPoint[cntAllPInput] = Double.Parse(point);
            cntAllPInput++;
        }

    }
}
