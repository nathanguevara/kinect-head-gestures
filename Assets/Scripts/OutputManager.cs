using UnityEngine;

public class OutputManager : MonoBehaviour
{
    [Header("Output Settings")]
    public Camera outputCamera;
    public RenderTexture outputTexture;
    public int outputWidth = 1920;
    public int outputHeight = 1080;
    
    [Header("Virtual Camera")]
    public bool enableVirtualCamera = true;
    public string virtualCameraName = "Kinect Head Tracker";
    
    [Header("Debug")]
    public bool showOutputInfo = true;
    
    private bool isInitialized = false;
    
    // Unity Capture Card integration points
    // TODO: Add Unity Capture Card specific components when package is imported
    
    void Start()
    {
        InitializeOutput();
    }
    
    private void InitializeOutput()
    {
        try
        {
            // Create render texture if not assigned
            if (outputTexture == null)
            {
                outputTexture = new RenderTexture(outputWidth, outputHeight, 24);
                outputTexture.name = "KinectOutput";
                outputTexture.Create();
                
                if (showOutputInfo)
                    Debug.Log($"OutputManager: Created render texture {outputWidth}x{outputHeight}");
            }
            
            // Setup output camera
            if (outputCamera != null)
            {
                outputCamera.targetTexture = outputTexture;
                
                if (showOutputInfo)
                    Debug.Log($"OutputManager: Camera '{outputCamera.name}' targeting output texture");
            }
            else
            {
                Debug.LogWarning("OutputManager: No output camera assigned");
            }
            
            // Initialize virtual camera
            if (enableVirtualCamera)
            {
                InitializeVirtualCamera();
            }
            
            isInitialized = true;
            
            if (showOutputInfo)
                Debug.Log("OutputManager: Initialization complete");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"OutputManager: Initialization failed - {e.Message}");
        }
    }
    
    private void InitializeVirtualCamera()
    {
        // TODO: Initialize Unity Capture Card virtual camera
        // This will be implemented when Unity Capture Card package is imported
        
        if (showOutputInfo)
            Debug.Log($"OutputManager: Virtual camera '{virtualCameraName}' setup pending Unity Capture Card");
    }
    
    public void StartOutput()
    {
        if (!isInitialized)
        {
            InitializeOutput();
            return;
        }
        
        if (outputCamera != null)
        {
            outputCamera.enabled = true;
        }
        
        // TODO: Start virtual camera streaming
        
        if (showOutputInfo)
            Debug.Log("OutputManager: Output started");
    }
    
    public void StopOutput()
    {
        if (outputCamera != null)
        {
            outputCamera.enabled = false;
        }
        
        // TODO: Stop virtual camera streaming
        
        if (showOutputInfo)
            Debug.Log("OutputManager: Output stopped");
    }
    
    public void UpdateOutputResolution(int width, int height)
    {
        if (width == outputWidth && height == outputHeight) return;
        
        outputWidth = width;
        outputHeight = height;
        
        // Recreate render texture
        if (outputTexture != null)
        {
            outputTexture.Release();
            outputTexture.width = width;
            outputTexture.height = height;
            outputTexture.Create();
            
            if (showOutputInfo)
                Debug.Log($"OutputManager: Resolution updated to {width}x{height}");
        }
    }
    
    public Texture2D CaptureFrame()
    {
        if (outputTexture == null) return null;
        
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = outputTexture;
        
        Texture2D capture = new Texture2D(outputTexture.width, outputTexture.height, TextureFormat.RGB24, false);
        capture.ReadPixels(new Rect(0, 0, outputTexture.width, outputTexture.height), 0, 0);
        capture.Apply();
        
        RenderTexture.active = currentRT;
        
        return capture;
    }
    
    public void SaveFrame(string filename = "")
    {
        Texture2D frame = CaptureFrame();
        if (frame == null) return;
        
        if (string.IsNullOrEmpty(filename))
        {
            filename = $"KinectCapture_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        }
        
        byte[] bytes = frame.EncodeToPNG();
        string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
        System.IO.File.WriteAllBytes(path, bytes);
        
        Destroy(frame);
        
        if (showOutputInfo)
            Debug.Log($"OutputManager: Frame saved to {path}");
    }
    
    void OnDestroy()
    {
        if (outputTexture != null)
        {
            outputTexture.Release();
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            StopOutput();
        else
            StartOutput();
    }
}