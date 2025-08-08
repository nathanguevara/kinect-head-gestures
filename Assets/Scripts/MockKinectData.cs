using UnityEngine;

public class MockKinectData : MonoBehaviour
{
    [Header("Mock Data Settings")]
    public bool enableMockData = true;
    public HeadTracker headTracker;
    
    [Header("Simulation Parameters")]
    public float movementSpeed = 1.0f;
    public float rotationSpeed = 30f;
    public Vector3 movementRange = new Vector3(1.0f, 0.5f, 1.0f);
    public float confidenceLevel = 0.8f;
    public bool simulateTrackingLoss = false;
    public float trackingLossInterval = 5.0f;
    
    [Header("Movement Patterns")]
    public bool useKeyboardInput = true;
    public bool useAutomaticMovement = false;
    public float automaticMovementSpeed = 0.5f;
    
    private Vector3 basePosition = new Vector3(0, 1.5f, 2.0f); // Typical head position
    private Vector3 currentTargetPosition;
    private Quaternion currentTargetRotation = Quaternion.identity;
    private float trackingLossTimer = 0f;
    private bool isCurrentlyTracking = true;
    
    void Start()
    {
        if (headTracker == null)
            headTracker = FindObjectOfType<HeadTracker>();
        
        currentTargetPosition = basePosition;
        
        if (enableMockData)
            Debug.Log("MockKinectData: Enabled - Use WASD/QE for movement, Space to toggle tracking");
    }
    
    void Update()
    {
        if (!enableMockData || headTracker == null) return;
        
        HandleTrackingSimulation();
        
        if (isCurrentlyTracking)
        {
            if (useKeyboardInput)
                HandleKeyboardInput();
            
            if (useAutomaticMovement)
                HandleAutomaticMovement();
            
            // Send mock data to head tracker
            headTracker.UpdateHeadData(currentTargetPosition, currentTargetRotation, confidenceLevel);
        }
    }
    
    private void HandleTrackingSimulation()
    {
        // Simulate tracking loss
        if (simulateTrackingLoss)
        {
            trackingLossTimer += Time.deltaTime;
            if (trackingLossTimer >= trackingLossInterval)
            {
                isCurrentlyTracking = !isCurrentlyTracking;
                trackingLossTimer = 0f;
                
                Debug.Log($"MockKinectData: Simulated tracking {(isCurrentlyTracking ? "acquired" : "lost")}");
            }
        }
        
        // Manual tracking toggle
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCurrentlyTracking = !isCurrentlyTracking;
            Debug.Log($"MockKinectData: Manual tracking toggle - {(isCurrentlyTracking ? "ON" : "OFF")}");
        }
        
        // Send low confidence when not tracking
        if (!isCurrentlyTracking)
        {
            headTracker.UpdateHeadData(currentTargetPosition, currentTargetRotation, 0.0f);
        }
    }
    
    private void HandleKeyboardInput()
    {
        Vector3 movement = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        
        // Position controls (WASD + QE)
        if (Input.GetKey(KeyCode.W)) movement.z += movementSpeed * Time.deltaTime; // Forward
        if (Input.GetKey(KeyCode.S)) movement.z -= movementSpeed * Time.deltaTime; // Backward
        if (Input.GetKey(KeyCode.A)) movement.x -= movementSpeed * Time.deltaTime; // Left
        if (Input.GetKey(KeyCode.D)) movement.x += movementSpeed * Time.deltaTime; // Right
        if (Input.GetKey(KeyCode.Q)) movement.y -= movementSpeed * Time.deltaTime; // Down
        if (Input.GetKey(KeyCode.E)) movement.y += movementSpeed * Time.deltaTime; // Up
        
        // Rotation controls (Arrow keys)
        if (Input.GetKey(KeyCode.LeftArrow)) rotation.y -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) rotation.y += rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) rotation.x -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) rotation.x += rotationSpeed * Time.deltaTime;
        
        // Apply movement with limits
        currentTargetPosition += movement;
        currentTargetPosition = new Vector3(
            Mathf.Clamp(currentTargetPosition.x, basePosition.x - movementRange.x, basePosition.x + movementRange.x),
            Mathf.Clamp(currentTargetPosition.y, basePosition.y - movementRange.y, basePosition.y + movementRange.y),
            Mathf.Clamp(currentTargetPosition.z, basePosition.z - movementRange.z, basePosition.z + movementRange.z)
        );
        
        // Apply rotation
        currentTargetRotation *= Quaternion.Euler(rotation);
        
        // Reset to center
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentTargetPosition = basePosition;
            currentTargetRotation = Quaternion.identity;
            Debug.Log("MockKinectData: Reset to center position");
        }
    }
    
    private void HandleAutomaticMovement()
    {
        // Simple figure-8 movement pattern
        float time = Time.time * automaticMovementSpeed;
        
        float x = Mathf.Sin(time) * movementRange.x;
        float y = Mathf.Sin(time * 2) * movementRange.y * 0.5f;
        float z = Mathf.Cos(time * 0.5f) * movementRange.z * 0.3f;
        
        currentTargetPosition = basePosition + new Vector3(x, y, z);
        
        // Gentle head rotation
        float rotY = Mathf.Sin(time * 0.3f) * 15f;
        currentTargetRotation = Quaternion.Euler(0, rotY, 0);
    }
    
    public void SetMockDataEnabled(bool enabled)
    {
        enableMockData = enabled;
        
        if (enabled)
            Debug.Log("MockKinectData: Enabled");
        else
            Debug.Log("MockKinectData: Disabled");
    }
    
    public void SetConfidence(float confidence)
    {
        confidenceLevel = Mathf.Clamp01(confidence);
    }
    
    public void SimulateTrackingLoss(float duration)
    {
        StartCoroutine(TemporaryTrackingLoss(duration));
    }
    
    private System.Collections.IEnumerator TemporaryTrackingLoss(float duration)
    {
        bool wasTracking = isCurrentlyTracking;
        isCurrentlyTracking = false;
        
        Debug.Log($"MockKinectData: Simulating tracking loss for {duration} seconds");
        
        yield return new WaitForSeconds(duration);
        
        isCurrentlyTracking = wasTracking;
        Debug.Log("MockKinectData: Tracking restored");
    }
    
    void OnGUI()
    {
        if (!enableMockData) return;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 300, 140));
        GUILayout.Label("MOCK KINECT CONTROLS:");
        GUILayout.Label("WASD/QE - Move head position");
        GUILayout.Label("Arrow Keys - Rotate head");
        GUILayout.Label("Space - Toggle tracking");
        GUILayout.Label("R - Reset to center");
        GUILayout.Label($"Tracking: {(isCurrentlyTracking ? "ON" : "OFF")}");
        GUILayout.EndArea();
    }
}