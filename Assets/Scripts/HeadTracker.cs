using UnityEngine;

public class HeadTracker : MonoBehaviour
{
    [Header("Tracking Settings")]
    public float smoothingFactor = 0.1f;
    public float confidenceThreshold = 0.5f;
    public bool useSmoothing = true;
    
    [Header("Coordinate Mapping")]
    public float positionScale = 1.0f;
    public Vector3 offsetPosition = Vector3.zero;
    
    [Header("Debug")]
    public bool showDebugData = true;
    public Transform debugVisualization;
    
    // Current head data
    private Vector3 currentHeadPosition = Vector3.zero;
    private Quaternion currentHeadRotation = Quaternion.identity;
    private float currentConfidence = 0f;
    private bool isTracking = false;
    
    // Smoothed data
    private Vector3 smoothedPosition = Vector3.zero;
    private Quaternion smoothedRotation = Quaternion.identity;
    
    // Events for other components
    public System.Action<Vector3, Quaternion, float> OnHeadDataUpdated;
    public System.Action<bool> OnTrackingStateChanged;
    
    public Vector3 HeadPosition => useSmoothing ? smoothedPosition : currentHeadPosition;
    public Quaternion HeadRotation => useSmoothing ? smoothedRotation : currentHeadRotation;
    public float Confidence => currentConfidence;
    public bool IsTracking => isTracking;
    
    void Start()
    {
        // Subscribe to KinectManager if available
        if (KinectManager.Instance != null)
        {
            // TODO: Subscribe to Kinect head tracking events
        }
    }
    
    void Update()
    {
        if (isTracking && useSmoothing)
        {
            ApplySmoothing();
        }
        
        UpdateDebugVisualization();
    }
    
    public void UpdateHeadData(Vector3 position, Quaternion rotation, float confidence)
    {
        // Update raw data
        currentHeadPosition = position;
        currentHeadRotation = rotation;
        currentConfidence = confidence;
        
        // Check if tracking is valid
        bool wasTracking = isTracking;
        isTracking = confidence >= confidenceThreshold;
        
        if (wasTracking != isTracking)
        {
            OnTrackingStateChanged?.Invoke(isTracking);
            
            if (showDebugData)
                Debug.Log($"HeadTracker: Tracking state changed to {isTracking}");
        }
        
        // Apply coordinate transformations
        Vector3 transformedPosition = TransformKinectToUnity(position);
        
        // Update smoothed data immediately if not using smoothing
        if (!useSmoothing)
        {
            smoothedPosition = transformedPosition;
            smoothedRotation = rotation;
        }
        
        // Notify listeners
        OnHeadDataUpdated?.Invoke(transformedPosition, rotation, confidence);
        
        if (showDebugData)
        {
            Debug.Log($"Head: Pos({transformedPosition:F2}) Rot({rotation.eulerAngles:F1}) Conf({confidence:F2})");
        }
    }
    
    private void ApplySmoothing()
    {
        if (!isTracking) return;
        
        Vector3 targetPosition = TransformKinectToUnity(currentHeadPosition);
        
        smoothedPosition = Vector3.Lerp(smoothedPosition, targetPosition, smoothingFactor);
        smoothedRotation = Quaternion.Lerp(smoothedRotation, currentHeadRotation, smoothingFactor);
    }
    
    private Vector3 TransformKinectToUnity(Vector3 kinectPosition)
    {
        // Convert Kinect coordinate system to Unity coordinate system
        // Kinect: X-right, Y-up, Z-forward (meters)
        // Unity: X-right, Y-up, Z-forward
        
        Vector3 unityPosition = new Vector3(
            kinectPosition.x * positionScale,
            kinectPosition.y * positionScale,
            kinectPosition.z * positionScale
        );
        
        return unityPosition + offsetPosition;
    }
    
    private void UpdateDebugVisualization()
    {
        if (debugVisualization != null && isTracking)
        {
            debugVisualization.position = HeadPosition;
            debugVisualization.rotation = HeadRotation;
        }
    }
    
    public void CalibrateCenter()
    {
        if (isTracking)
        {
            offsetPosition = -TransformKinectToUnity(currentHeadPosition);
            
            if (showDebugData)
                Debug.Log($"HeadTracker: Calibrated center offset to {offsetPosition}");
        }
    }
    
    public void ResetCalibration()
    {
        offsetPosition = Vector3.zero;
        
        if (showDebugData)
            Debug.Log("HeadTracker: Calibration reset");
    }
}