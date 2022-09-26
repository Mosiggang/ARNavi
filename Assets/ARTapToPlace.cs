using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using TMPro;
public class ARTapToPlace : MonoBehaviour
{
    public GameObject tt;
    private TextMeshProUGUI textP;
    private AREarthManager earthManager;
    private ARCoreExtensionsConfig config;
    public ARSession ARSession;
    public ARSessionOrigin ARSessionOrigin;
    public ARAnchorManager ARAnchorM;
    public FeatureSupported f;
    private GameObject[] a = new GameObject[16];
    private ARGeospatialAnchor[] anchor = new ARGeospatialAnchor[16];
    private LineRenderer[] lines = new LineRenderer[15];

    private GameObject p1, p2;
    private ARGeospatialAnchor a1, a2, a3;
    //private LineRenderer lr;
    public Material m, m2;
    public GameObject test;
    // Start is called before the first frame update
    void Start()
    {
        textP = tt.GetComponent<TextMeshProUGUI>();
        config = new ARCoreExtensionsConfig();
        config.GeospatialMode = GeospatialMode.Enabled;
        earthManager = FindObjectOfType<AREarthManager>();
        //ARAnchorM = FindObjectOfType<ARAnchorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Working 0");
        GeoAR();
        Debug.Log("Working 1");
    }
    private void GeoAR()
    {
        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            Debug.Log("Working 2");
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;
            /*for(int i = 0; i < 16; i++)
            {
                DestroyImmediate(anchor[i]);
                //DestroyImmediate(a[i]);
                anchor[i] = null;
            }
            for (int i = 0; i < 15; i++)
            {
                lines[i] = null;
            }*/
            var alti = 0;
            Debug.Log("Working 2-0");
            for (int i = 0; i < 16; i++)
            {
                DestroyImmediate(anchor[i]);
                DestroyImmediate(a[i]);
                
            }
            Debug.Log("Working 2-1");
            for (int i = 0; i < 15; i++)
            {
                if(lines[i] != null)
                {
                    DestroyImmediate(a[i].GetComponent<LineRenderer>());
                }
                lines[i] = null;
            }
            Debug.Log("Working 2-2");
            anchor[0] = ARAnchorM.ResolveAnchorOnTerrain(37.88398138901406, 127.73343466933132, alti, Quaternion.identity);
            anchor[1] = ARAnchorM.ResolveAnchorOnTerrain(37.8839871194505, 127.73343749702332, alti, Quaternion.identity);
            anchor[2] = ARAnchorM.ResolveAnchorOnTerrain(37.883998228750364, 127.73340972144396, alti, Quaternion.identity);
            anchor[3] = ARAnchorM.ResolveAnchorOnTerrain(37.88425375637654, 127.73350692722941, alti, Quaternion.identity);
            anchor[4] = ARAnchorM.ResolveAnchorOnTerrain(37.88454816862451, 127.73361801948475, alti, Quaternion.identity);
            anchor[5] = ARAnchorM.ResolveAnchorOnTerrain(37.88463427012807, 127.73364023712753, alti, Quaternion.identity);
            anchor[6] = ARAnchorM.ResolveAnchorOnTerrain(37.884703706282956, 127.73362912496584, alti, Quaternion.identity);
            anchor[7] = ARAnchorM.ResolveAnchorOnTerrain(37.88481480337433, 127.73357079364295, alti, Quaternion.identity);
            anchor[8] = ARAnchorM.ResolveAnchorOnTerrain(37.88502033277832, 127.73345135396714, alti, Quaternion.identity);
            anchor[9] = ARAnchorM.ResolveAnchorOnTerrain(37.8854452817081, 127.73336523807478, alti, Quaternion.identity);
            anchor[10] = ARAnchorM.ResolveAnchorOnTerrain(37.88545084371525, 127.7337457588266, alti, Quaternion.identity);
            anchor[11] = ARAnchorM.ResolveAnchorOnTerrain(37.885456403131876, 127.73398740333147, alti, Quaternion.identity);
            anchor[12] = ARAnchorM.ResolveAnchorOnTerrain(37.88545363572912, 127.73452624325257, alti, Quaternion.identity);
            anchor[13] = ARAnchorM.ResolveAnchorOnTerrain(37.8854814148854, 127.73477344214757, alti, Quaternion.identity);
            anchor[14] = ARAnchorM.ResolveAnchorOnTerrain(37.885606404741395, 127.73500952805584, alti, Quaternion.identity);
            anchor[15] = ARAnchorM.ResolveAnchorOnTerrain(37.88570084151001, 127.73518728684762, alti, Quaternion.identity);
            /*anchor[0] = ARAnchorM.AddAnchor(37.88398138901406, 127.73343466933132, alti, Quaternion.identity);
            anchor[1] = ARAnchorM.AddAnchor(37.8839871194505, 127.73343749702332, alti, Quaternion.identity);
            anchor[2] = ARAnchorM.AddAnchor(37.883998228750364, 127.73340972144396, alti, Quaternion.identity);
            anchor[3] = ARAnchorM.AddAnchor(37.88425375637654, 127.73350692722941, alti, Quaternion.identity);
            anchor[4] = ARAnchorM.AddAnchor(37.88454816862451, 127.73361801948475, alti, Quaternion.identity);
            anchor[5] = ARAnchorM.AddAnchor(37.88463427012807, 127.73364023712753, alti, Quaternion.identity);
            anchor[6] = ARAnchorM.AddAnchor(37.884703706282956, 127.73362912496584, alti, Quaternion.identity);
            anchor[7] = ARAnchorM.AddAnchor(37.88481480337433, 127.73357079364295, alti, Quaternion.identity);
            anchor[8] = ARAnchorM.AddAnchor(37.88502033277832, 127.73345135396714, alti, Quaternion.identity);
            anchor[9] = ARAnchorM.AddAnchor(37.8854452817081, 127.73336523807478, alti, Quaternion.identity);
            anchor[10] = ARAnchorM.AddAnchor(37.88545084371525, 127.7337457588266, alti, Quaternion.identity);
            anchor[11] = ARAnchorM.AddAnchor(37.885456403131876, 127.73398740333147, alti, Quaternion.identity);
            anchor[12] = ARAnchorM.AddAnchor(37.88545363572912, 127.73452624325257, alti, Quaternion.identity);
            anchor[13] = ARAnchorM.AddAnchor(37.8854814148854, 127.73477344214757, alti, Quaternion.identity);
            anchor[14] = ARAnchorM.AddAnchor(37.885606404741395, 127.73500952805584, alti, Quaternion.identity);
            anchor[15] = ARAnchorM.AddAnchor(37.88570084151001, 127.73518728684762, alti, Quaternion.identity);*/
            Debug.Log("Working 2-3");
            for (int i =0; i < 16; i++)
            {
                LineRenderer lr;
                a[i] = Instantiate(ARAnchorM.anchorPrefab, anchor[i].transform);
                if(i < 15)
                {
                    lr = a[i].AddComponent<LineRenderer>();
                    lr.positionCount = 15;
                    lr.material = m;
                    lr.startWidth = 0.2f;
                    lr.endWidth = 0.2f;
                    lines[i] = lr;
                }
            }
            
            textP.text = "IS TRACKING: " + anchor[15].terrainAnchorState + ", " + anchor[5].terrainAnchorState + ", " + anchor[6].terrainAnchorState;
            Debug.Log("Working 2-4");
            for (int i = 0; i < 14; i++)
            {
                
                if(anchor[i].terrainAnchorState.Equals(TerrainAnchorState.TaskInProgress) && anchor[i+1].terrainAnchorState.Equals(TerrainAnchorState.TaskInProgress))
                {
                    //textP.text = "IS TRACKING: " + anchor[i].terrainAnchorState + ", " + anchor[i+1].terrainAnchorState;
                    lines[i].SetPosition(0, anchor[i].pose.position);
                    lines[i + 1].SetPosition(1, anchor[i+1].transform.position);
                }
            }
            /*for (int i = 0; i < 15; i++)
            {
                LineRenderer lr = anchor[i].gameObject.AddComponent<LineRenderer>();
                lr.startWidth = 0.5f;
                lr.endWidth = 0.5f;
                lr.material = m;
                //a[i] = Instantiate(ARAnchorManager.anchorPrefab, anchor[i].transform);
                lr.SetPosition(0, anchor[i].transform.position);
                lr.SetPosition(1, anchor[i + 1].pose.position);
                lines[i] = lr;
            }*/
            Debug.Log("Working 3");
           /* if (lr != null)
            {
                DestroyImmediate(a1.gameObject.GetComponent<LineRenderer>());
                
            }
            if (lr2 != null)
            {
                DestroyImmediate(a2.gameObject.GetComponent<LineRenderer>());
            }
            Debug.Log("Working 4");

            DestroyImmediate(a1);
            DestroyImmediate(a2);
            DestroyImmediate(a3);
            Debug.Log("Working 5");
            lr = null;
            lr2 = null;
            
            a1 = ARAnchorManagerExtensions.ResolveAnchorOnTerrain(ARAnchorManager, 37.88469111426681, 127.73365083624829, -10, Quaternion.identity);
            a2 = ARAnchorManagerExtensions.ResolveAnchorOnTerrain(ARAnchorManager, 37.8832687036883, 127.73254634647736, -10, Quaternion.identity);
            a3 = ARAnchorManagerExtensions.ResolveAnchorOnTerrain(ARAnchorManager, 37.882950388719394, 127.73143209685556, -10, Quaternion.identity);

            
            lr = a1.gameObject.AddComponent<LineRenderer>();
            lr.positionCount = 3;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.material = m;

            lr.SetPosition(0, a1.pose.position);
            lr.SetPosition(1, a2.transform.position);
            lr.SetPosition(2, a3.transform.position);
            textP.text = "IS TRACKING" + cameraGeospatialPose.Latitude + ", " + cameraGeospatialPose.Longitude;
            Debug.Log("Working 6");*/
        }
        else
        {
            Debug.Log("Working 11");
            textP.text = "NO TRACKING";
        }
    }
  
}
