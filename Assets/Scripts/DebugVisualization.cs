using UnityEngine;

public class DebugVisualization : MonoBehaviour
{
    [Header("References")]
    public HeadTracker headTracker;
    
    [Header("Visualization Settings")]
    public GameObject headCube;
    public Material trackingMaterial;
    public Material notTrackingMaterial;
    public bool showTrackingData = true;
    public bool showCoordinateAxes = true;
    
    [Header("UI Debug Info")]
    public UnityEngine.UI.Text debugText;
    public Canvas debugCanvas;
    
    [Header("Coordinate System")]
    public Transform kinectOrigin;
    public float axisLength = 1.0f;
    
    private LineRenderer[] coordinateAxes = new LineRenderer[3];
    private Renderer cubeRenderer;
    
    void Start()
    {
        SetupVisualization();
        
        if (headTracker == null)
            headTracker = FindObjectOfType<HeadTracker>();
        
        if (headTracker != null)
        {
            headTracker.OnHeadDataUpdated += OnHeadDataUpdated;
            headTracker.OnTrackingStateChanged += OnTrackingStateChanged;
        }
        
        CreateCoordinateAxes();
        SetupDebugUI();
    }
    
    private void SetupVisualization()
    {
        // Create head visualization cube if not assigned
        if (headCube == null)
        {
            headCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            headCube.name = "HeadTrackingCube";
            headCube.transform.localScale = Vector3.one * 0.2f;
            
            // Remove collider as we don't need physics
            Destroy(headCube.GetComponent<Collider>());
        }
        
        cubeRenderer = headCube.GetComponent<Renderer>();
        
        // Set initial material
        if (notTrackingMaterial != null)
            cubeRenderer.material = notTrackingMaterial;
    }
    
    private void CreateCoordinateAxes()
    {
        if (!showCoordinateAxes || kinectOrigin == null) return;
        
        // Create parent for axes
        GameObject axesParent = new GameObject("KinectCoordinateAxes");
        axesParent.transform.parent = kinectOrigin;
        axesParent.transform.localPosition = Vector3.zero;
        
        // Create X, Y, Z axes
        string[] axisNames = { "X-Axis", "Y-Axis", "Z-Axis" };
        Color[] axisColors = { Color.red, Color.green, Color.blue };
        Vector3[] axisDirections = { Vector3.right, Vector3.up, Vector3.forward };
        
        for (int i = 0; i < 3; i++)
        {
            GameObject axisObject = new GameObject(axisNames[i]);
            axisObject.transform.parent = axesParent.transform;
            
            LineRenderer lr = axisObject.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = axisColors[i];
            lr.endColor = axisColors[i];
            lr.startWidth = 0.02f;
            lr.endWidth = 0.02f;
            lr.positionCount = 2;
            lr.useWorldSpace = false;
            
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, axisDirections[i] * axisLength);
            
            coordinateAxes[i] = lr;
        }
    }
    
    private void SetupDebugUI()
    {
        if (debugCanvas == null) return;
        
        // Create debug text if not assigned
        if (debugText == null)
        {
            GameObject textObj = new GameObject("DebugText");
            textObj.transform.parent = debugCanvas.transform;
            
            debugText = textObj.AddComponent<UnityEngine.UI.Text>();
            debugText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            debugText.fontSize = 16;
            debugText.color = Color.white;
            
            RectTransform rectTransform = debugText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, -10);
            rectTransform.sizeDelta = new Vector2(400, 200);
        }
    }
    
    private void OnHeadDataUpdated(Vector3 headPosition, Quaternion headRotation, float confidence)
    {
        if (!showTrackingData) return;
        
        // Update cube position and rotation
        if (headCube != null)
        {
            headCube.transform.position = headPosition;
            headCube.transform.rotation = headRotation;
        }
        
        // Update debug text
        UpdateDebugText(headPosition, headRotation, confidence);
    }
    
    private void OnTrackingStateChanged(bool isTracking)
    {
        // Update cube material based on tracking state
        if (cubeRenderer != null)
        {
            if (isTracking && trackingMaterial != null)
                cubeRenderer.material = trackingMaterial;
            else if (!isTracking && notTrackingMaterial != null)
                cubeRenderer.material = notTrackingMaterial;
        }
        
        // Show/hide cube based on tracking
        if (headCube != null)
            headCube.SetActive(isTracking);
    }
    
    private void UpdateDebugText(Vector3 position, Quaternion rotation, float confidence)
    {
        if (debugText == null) return;
        
        debugText.text = $"HEAD TRACKING DEBUG\n" +
                        $"Position: ({position.x:F2}, {position.y:F2}, {position.z:F2})\n" +
                        $"Rotation: ({rotation.eulerAngles.x:F1}°, {rotation.eulerAngles.y:F1}°, {rotation.eulerAngles.z:F1}°)\n" +
                        $"Confidence: {confidence:F2}\n" +
                        $"Tracking: {(headTracker != null ? headTracker.IsTracking : false)}\n" +
                        $"Frame: {Time.frameCount}";
    }
    
    void Update()
    {
        // Update debug info even when not tracking
        if (debugText != null && (headTracker == null || !headTracker.IsTracking))
        {
            debugText.text = $"HEAD TRACKING DEBUG\n" +
                            $"Status: No tracking data\n" +
                            $"Kinect Manager: {(KinectManager.Instance != null ? "Found" : "Not Found")}\n" +
                            $"Head Tracker: {(headTracker != null ? "Found" : "Not Found")}\n" +
                            $"Frame: {Time.frameCount}";
        }
    }
    
    public void ToggleVisualization()
    {
        showTrackingData = !showTrackingData;
        
        if (headCube != null)
            headCube.SetActive(showTrackingData);
    }
    
    public void ToggleCoordinateAxes()
    {
        showCoordinateAxes = !showCoordinateAxes;
        
        foreach (LineRenderer axis in coordinateAxes)
        {
            if (axis != null)
                axis.gameObject.SetActive(showCoordinateAxes);
        }
    }
    
    void OnDestroy()
    {
        if (headTracker != null)
        {
            headTracker.OnHeadDataUpdated -= OnHeadDataUpdated;
            headTracker.OnTrackingStateChanged -= OnTrackingStateChanged;
        }
    }
}