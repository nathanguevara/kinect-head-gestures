using UnityEngine;

public class TestSceneSetup : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool useMockData = true;
    public bool enableDebugVisualization = true;
    public bool enableWebcamTest = false;
    
    [Header("Prefab References")]
    public GameObject kinectManagerPrefab;
    public GameObject headTrackerPrefab;
    public GameObject debugVisualizationPrefab;
    
    [Header("Materials")]
    public Material trackingMaterial;
    public Material notTrackingMaterial;
    
    private KinectManager kinectManager;
    private HeadTracker headTracker;
    private MockKinectData mockData;
    private DebugVisualization debugViz;
    private WebcamController webcamController;
    
    void Awake()
    {
        SetupTestEnvironment();
    }
    
    private void SetupTestEnvironment()
    {
        Debug.Log("TestSceneSetup: Initializing test environment...");
        
        // Create KinectManager
        SetupKinectManager();
        
        // Create HeadTracker
        SetupHeadTracker();
        
        // Create Debug Visualization
        if (enableDebugVisualization)
            SetupDebugVisualization();
        
        // Create Mock Data Generator
        if (useMockData)
            SetupMockData();
        
        // Create Webcam Controller for testing
        if (enableWebcamTest)
            SetupWebcamController();
        
        Debug.Log("TestSceneSetup: Test environment ready!");
    }
    
    private void SetupKinectManager()
    {
        GameObject kinectObj;
        
        if (kinectManagerPrefab != null)
        {
            kinectObj = Instantiate(kinectManagerPrefab);
        }
        else
        {
            kinectObj = new GameObject("KinectManager");
            kinectObj.AddComponent<KinectManager>();
        }
        
        kinectManager = kinectObj.GetComponent<KinectManager>();
        
        if (kinectManager == null)
        {
            Debug.LogError("TestSceneSetup: Failed to create KinectManager");
            return;
        }
        
        Debug.Log("TestSceneSetup: KinectManager created");
    }
    
    private void SetupHeadTracker()
    {
        GameObject headTrackerObj;
        
        if (headTrackerPrefab != null)
        {
            headTrackerObj = Instantiate(headTrackerPrefab);
        }
        else
        {
            headTrackerObj = new GameObject("HeadTracker");
            headTrackerObj.AddComponent<HeadTracker>();
        }
        
        headTracker = headTrackerObj.GetComponent<HeadTracker>();
        
        if (headTracker == null)
        {
            Debug.LogError("TestSceneSetup: Failed to create HeadTracker");
            return;
        }
        
        Debug.Log("TestSceneSetup: HeadTracker created");
    }
    
    private void SetupDebugVisualization()
    {
        GameObject debugObj;
        
        if (debugVisualizationPrefab != null)
        {
            debugObj = Instantiate(debugVisualizationPrefab);
        }
        else
        {
            debugObj = new GameObject("DebugVisualization");
            debugObj.AddComponent<DebugVisualization>();
        }
        
        debugViz = debugObj.GetComponent<DebugVisualization>();
        
        if (debugViz != null)
        {
            debugViz.headTracker = headTracker;
            debugViz.trackingMaterial = trackingMaterial;
            debugViz.notTrackingMaterial = notTrackingMaterial;
            
            // Create debug canvas
            CreateDebugCanvas();
            
            Debug.Log("TestSceneSetup: Debug visualization created");
        }
    }
    
    private void CreateDebugCanvas()
    {
        // Create canvas for debug UI
        GameObject canvasObj = new GameObject("DebugCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        if (debugViz != null)
        {
            debugViz.debugCanvas = canvas;
        }
    }
    
    private void SetupMockData()
    {
        GameObject mockDataObj = new GameObject("MockKinectData");
        mockData = mockDataObj.AddComponent<MockKinectData>();
        
        if (mockData != null && headTracker != null)
        {
            mockData.headTracker = headTracker;
            mockData.enableMockData = true;
            
            Debug.Log("TestSceneSetup: Mock data generator created");
        }
    }
    
    private void SetupWebcamController()
    {
        GameObject webcamObj = new GameObject("WebcamController");
        webcamController = webcamObj.AddComponent<WebcamController>();
        
        if (webcamController != null)
        {
            Debug.Log("TestSceneSetup: Webcam controller created");
        }
    }
    
    public void ToggleMockData()
    {
        if (mockData != null)
        {
            mockData.enableMockData = !mockData.enableMockData;
            Debug.Log($"TestSceneSetup: Mock data {(mockData.enableMockData ? "enabled" : "disabled")}");
        }
    }
    
    public void ToggleDebugVisualization()
    {
        if (debugViz != null)
        {
            debugViz.ToggleVisualization();
        }
    }
    
    public void CalibrateHeadTracker()
    {
        if (headTracker != null)
        {
            headTracker.CalibrateCenter();
            Debug.Log("TestSceneSetup: Head tracker calibrated");
        }
    }
    
    void Update()
    {
        // Hot keys for testing
        if (Input.GetKeyDown(KeyCode.F1))
            ToggleMockData();
        
        if (Input.GetKeyDown(KeyCode.F2))
            ToggleDebugVisualization();
        
        if (Input.GetKeyDown(KeyCode.F3))
            CalibrateHeadTracker();
        
        if (Input.GetKeyDown(KeyCode.F4) && webcamController != null)
        {
            if (webcamController.IsPlaying)
                webcamController.StopWebcam();
            else
                webcamController.StartWebcam();
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 200));
        GUILayout.Label("TEST CONTROLS:");
        GUILayout.Label("F1 - Toggle Mock Data");
        GUILayout.Label("F2 - Toggle Debug Viz");
        GUILayout.Label("F3 - Calibrate Tracker");
        if (enableWebcamTest)
            GUILayout.Label("F4 - Toggle Webcam");
        
        GUILayout.Space(10);
        GUILayout.Label("STATUS:");
        GUILayout.Label($"Mock Data: {(mockData != null && mockData.enableMockData ? "ON" : "OFF")}");
        GUILayout.Label($"Tracking: {(headTracker != null && headTracker.IsTracking ? "ON" : "OFF")}");
        if (webcamController != null)
            GUILayout.Label($"Webcam: {(webcamController.IsPlaying ? "ON" : "OFF")}");
        GUILayout.EndArea();
    }
}