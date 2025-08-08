using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    [Header("Webcam Settings")]
    public string deviceName = "";
    public int requestedWidth = 1920;
    public int requestedHeight = 1080;
    public int requestedFPS = 30;
    
    [Header("Display")]
    public RawImage displayImage;
    public Material webcamMaterial;
    
    [Header("Debug")]
    public bool showWebcamInfo = true;
    
    private WebCamTexture webcamTexture;
    private bool isInitialized = false;
    private bool isPlaying = false;
    
    public WebCamTexture WebcamTexture => webcamTexture;
    public bool IsInitialized => isInitialized;
    public bool IsPlaying => isPlaying;
    public Vector2 TextureSize => webcamTexture != null ? new Vector2(webcamTexture.width, webcamTexture.height) : Vector2.zero;
    
    void Start()
    {
        InitializeWebcam();
    }
    
    public bool InitializeWebcam()
    {
        if (isInitialized) return true;
        
        try
        {
            // Find available webcams
            WebCamDevice[] devices = WebCamTexture.devices;
            
            if (devices.Length == 0)
            {
                Debug.LogError("WebcamController: No webcam devices found");
                return false;
            }
            
            // Select device
            string selectedDevice = deviceName;
            if (string.IsNullOrEmpty(selectedDevice) || !DeviceExists(selectedDevice))
            {
                selectedDevice = devices[0].name;
                
                if (showWebcamInfo)
                    Debug.Log($"WebcamController: Using default device: {selectedDevice}");
            }
            
            // Create webcam texture
            webcamTexture = new WebCamTexture(selectedDevice, requestedWidth, requestedHeight, requestedFPS);
            
            // Set up display
            if (displayImage != null)
            {
                displayImage.texture = webcamTexture;
            }
            
            if (webcamMaterial != null)
            {
                webcamMaterial.mainTexture = webcamTexture;
            }
            
            isInitialized = true;
            
            if (showWebcamInfo)
            {
                Debug.Log($"WebcamController: Initialized with device: {selectedDevice}");
                Debug.Log($"WebcamController: Requested resolution: {requestedWidth}x{requestedHeight} @ {requestedFPS}fps");
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"WebcamController: Initialization failed - {e.Message}");
            return false;
        }
    }
    
    public void StartWebcam()
    {
        if (!isInitialized)
        {
            if (!InitializeWebcam()) return;
        }
        
        if (webcamTexture != null && !webcamTexture.isPlaying)
        {
            webcamTexture.Play();
            isPlaying = true;
            
            if (showWebcamInfo)
            {
                // Log actual resolution after starting
                StartCoroutine(LogActualResolution());
            }
        }
    }
    
    public void StopWebcam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
            isPlaying = false;
            
            if (showWebcamInfo)
                Debug.Log("WebcamController: Webcam stopped");
        }
    }
    
    private bool DeviceExists(string deviceName)
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices)
        {
            if (device.name.Equals(deviceName))
                return true;
        }
        return false;
    }
    
    private System.Collections.IEnumerator LogActualResolution()
    {
        // Wait a frame for webcam to initialize properly
        yield return new WaitForEndOfFrame();
        
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            Debug.Log($"WebcamController: Actual resolution: {webcamTexture.width}x{webcamTexture.height}");
            Debug.Log($"WebcamController: Device name: {webcamTexture.deviceName}");
        }
    }
    
    public void ListAvailableDevices()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        
        Debug.Log($"WebcamController: Found {devices.Length} webcam device(s):");
        
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"  [{i}] {devices[i].name} - Front: {devices[i].isFrontFacing}");
        }
    }
    
    public void SwitchToDevice(string newDeviceName)
    {
        if (newDeviceName == deviceName) return;
        
        StopWebcam();
        
        if (webcamTexture != null)
        {
            Destroy(webcamTexture);
        }
        
        deviceName = newDeviceName;
        isInitialized = false;
        
        InitializeWebcam();
        StartWebcam();
    }
    
    void OnDestroy()
    {
        StopWebcam();
        
        if (webcamTexture != null)
        {
            Destroy(webcamTexture);
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            StopWebcam();
        else
            StartWebcam();
    }
}