using UnityEngine;

public class FeedPositioner : MonoBehaviour
{
    [Header("References")]
    public HeadTracker headTracker;
    public Transform webcamFeed;
    public RectTransform feedRectTransform;
    
    [Header("Position Mapping")]
    public float horizontalSensitivity = 100f;
    public float verticalSensitivity = 50f;
    public Vector2 positionLimits = new Vector2(200f, 100f);
    public bool invertHorizontal = false;
    public bool invertVertical = false;
    
    [Header("Scale Mapping")]
    public float scaleSensitivity = 0.5f;
    public Vector2 scaleRange = new Vector2(0.5f, 2.0f);
    public float baseDistance = 2.0f; // Base distance in meters for scale 1.0
    
    [Header("Smoothing")]
    public bool usePositionSmoothing = true;
    public bool useScaleSmoothing = true;
    public float positionSmoothTime = 0.1f;
    public float scaleSmoothTime = 0.15f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    // Target values
    private Vector2 targetPosition = Vector2.zero;
    private float targetScale = 1.0f;
    
    // Smoothing velocities
    private Vector2 positionVelocity = Vector2.zero;
    private float scaleVelocity = 0f;
    
    // Current smoothed values
    private Vector2 currentPosition = Vector2.zero;
    private float currentScale = 1.0f;
    
    // Reference values for calibration
    private Vector3 calibrationHeadPosition = Vector3.zero;
    private bool isCalibrated = false;
    
    void Start()
    {
        if (headTracker == null)
            headTracker = FindObjectOfType<HeadTracker>();
        
        if (headTracker != null)
        {
            headTracker.OnHeadDataUpdated += OnHeadDataUpdated;
            headTracker.OnTrackingStateChanged += OnTrackingStateChanged;
        }
        
        // Initialize current values
        if (feedRectTransform != null)
        {
            currentPosition = feedRectTransform.anchoredPosition;
        }
        
        if (webcamFeed != null)
        {
            currentScale = webcamFeed.localScale.x;
        }
    }
    
    void Update()
    {
        if (headTracker == null || !headTracker.IsTracking || !isCalibrated) return;
        
        UpdateFeedPosition();
        UpdateFeedScale();
        
        if (showDebugInfo)
        {
            DebugDisplay();
        }
    }
    
    private void OnHeadDataUpdated(Vector3 headPosition, Quaternion headRotation, float confidence)
    {
        if (!isCalibrated) return;
        
        // Calculate relative position from calibration point
        Vector3 relativePosition = headPosition - calibrationHeadPosition;
        
        // Map to screen position
        float horizontalMovement = relativePosition.x * horizontalSensitivity;
        float verticalMovement = relativePosition.y * verticalSensitivity;
        
        if (invertHorizontal) horizontalMovement = -horizontalMovement;
        if (invertVertical) verticalMovement = -verticalMovement;
        
        // Apply limits
        horizontalMovement = Mathf.Clamp(horizontalMovement, -positionLimits.x, positionLimits.x);
        verticalMovement = Mathf.Clamp(verticalMovement, -positionLimits.y, positionLimits.y);
        
        targetPosition = new Vector2(horizontalMovement, verticalMovement);
        
        // Calculate scale based on depth (Z-axis)
        float distance = headPosition.z;
        float scaleMultiplier = baseDistance / distance;
        targetScale = Mathf.Clamp(scaleMultiplier, scaleRange.x, scaleRange.y);
    }
    
    private void OnTrackingStateChanged(bool isTracking)
    {
        if (showDebugInfo)
            Debug.Log($"FeedPositioner: Tracking state changed to {isTracking}");
    }
    
    private void UpdateFeedPosition()
    {
        if (feedRectTransform == null) return;
        
        if (usePositionSmoothing)
        {
            currentPosition = Vector2.SmoothDamp(currentPosition, targetPosition, ref positionVelocity, positionSmoothTime);
        }
        else
        {
            currentPosition = targetPosition;
        }
        
        feedRectTransform.anchoredPosition = currentPosition;
    }
    
    private void UpdateFeedScale()
    {
        if (webcamFeed == null) return;
        
        if (useScaleSmoothing)
        {
            currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, scaleSmoothTime);
        }
        else
        {
            currentScale = targetScale;
        }
        
        webcamFeed.localScale = Vector3.one * currentScale;
    }
    
    public void Calibrate()
    {
        if (headTracker != null && headTracker.IsTracking)
        {
            calibrationHeadPosition = headTracker.HeadPosition;
            isCalibrated = true;
            
            // Reset to center position
            targetPosition = Vector2.zero;
            targetScale = 1.0f;
            
            if (showDebugInfo)
                Debug.Log($"FeedPositioner: Calibrated at position {calibrationHeadPosition}");
        }
        else
        {
            Debug.LogWarning("FeedPositioner: Cannot calibrate - head tracking not available");
        }
    }
    
    public void ResetCalibration()
    {
        isCalibrated = false;
        calibrationHeadPosition = Vector3.zero;
        targetPosition = Vector2.zero;
        targetScale = 1.0f;
        
        if (showDebugInfo)
            Debug.Log("FeedPositioner: Calibration reset");
    }
    
    private void DebugDisplay()
    {
        if (headTracker.IsTracking)
        {
            Vector3 headPos = headTracker.HeadPosition;
            Debug.Log($"Head: {headPos:F2} | Target Pos: {targetPosition:F1} | Target Scale: {targetScale:F2} | Current: Pos({currentPosition:F1}) Scale({currentScale:F2})");
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