# Kinect Head Tracking Webcam Controller

An interactive streaming tool that uses Kinect v2 head tracking to control webcam feed positioning and scaling in real-time for streaming applications (OBS, Discord, Zoom, etc.).

## Project Overview

**MVP Goal:** Track head movement with Kinect v2 to control webcam feed position - tilt left/right moves feed left/right, move closer/farther scales feed size.

**Tech Stack:**
- Unity 2022.3 LTS (Windows)
- Microsoft Kinect SDK 2.0
- Unity Capture Card for output
- C# scripting

## Prerequisites

### Hardware Requirements
- Kinect v2 sensor with USB 3.0 adapter
- Windows 10/11 machine (Kinect SDK requirement)
- Webcam for video feed
- USB 3.0 port

### Development Environment
- **Development:** WSL Ubuntu environment on Windows PC
- **Testing:** Windows host environment (required for Kinect SDK)
- **Workflow:** Develop in WSL → Push to GitHub → Pull to Windows for testing

### Software Setup Checklist

1. **Install Unity Hub & Unity**
   ```
   Download Unity Hub from unity.com
   Install Unity 2022.3 LTS through Hub
   Include Windows Build Support module
   ```

2. **Install Kinect v2 SDK**
   ```
   Download "Kinect for Windows SDK 2.0" from Microsoft
   Install SDK (includes drivers and runtime)
   Test with "SDK Browser v2.0" sample app
   ```

3. **Verify Kinect Setup**
   - Connect Kinect v2 to USB 3.0 port
   - Run Kinect Studio v2.0 to test sensor
   - Confirm depth, color, and body tracking work

4. **Install Unity Capture Card**
   ```
   Download from Unity Asset Store or GitHub
   Follow installation instructions for virtual camera setup
   ```

## Project Structure

```
KinectWebcamController/
├── Assets/
│   ├── Scripts/
│   │   ├── KinectManager.cs          # Main Kinect interface
│   │   ├── HeadTracker.cs            # Head pose processing
│   │   ├── WebcamController.cs       # Video feed management
│   │   ├── FeedPositioner.cs         # Position/scale calculations
│   │   └── OutputManager.cs          # Virtual camera output
│   ├── Scenes/
│   │   ├── MainScene.unity           # Primary scene
│   │   └── CalibrationScene.unity    # Setup/testing scene
│   ├── Materials/                    # Video materials
│   ├── Prefabs/                      # Reusable components
│   └── Plugins/
│       └── KinectSDK/                # Kinect Unity wrapper
├── Packages/                         # Unity package dependencies
├── ProjectSettings/                  # Unity project configuration
└── README.md
```

## Development Phases

### Phase 1: Basic Setup (MVP Foundation)
- [ ] Create Unity project with proper structure
- [ ] Import Kinect Unity wrapper package
- [ ] Implement basic head position detection
- [ ] Create simple UI cube that follows head movement
- [ ] Test coordinate mapping (Kinect 3D → Unity 2D)

### Phase 2: Video Integration
- [ ] Add webcam input to Unity scene
- [ ] Apply head tracking data to webcam feed positioning
- [ ] Implement scaling based on head depth (Z-axis)
- [ ] Add smoothing/filtering for stable movement
- [ ] Basic calibration system (set center position)

### Phase 3: Output Pipeline
- [ ] Integrate Unity Capture Card
- [ ] Test output in OBS/streaming software
- [ ] Optimize performance (target 30fps minimum)
- [ ] Add debugging visualization overlay
- [ ] Create user configuration interface

### Phase 4: Polish & Features
- [ ] Background removal using Kinect depth data
- [ ] Multiple gesture controls (head tilt for rotation?)
- [ ] Save/load user calibration settings
- [ ] Performance monitoring and optimization
- [ ] Error handling and recovery

## Key Technical Concepts

### Coordinate System Mapping
- **Kinect Space:** 3D coordinates in meters, origin at sensor
- **Unity Space:** 3D world coordinates, customizable origin
- **Screen Space:** 2D pixel coordinates for final output

### Head Tracking Data
```csharp
// Key data points from Kinect
Vector3 headPosition;      // X, Y, Z in Kinect space
Quaternion headRotation;   // Head orientation
float confidenceLevel;     // Tracking reliability (0-1)
```

### Movement Mapping Strategy
- **X-axis:** Head left/right → Feed left/right movement
- **Z-axis:** Head closer/farther → Feed scale increase/decrease
- **Y-axis:** Future use (head up/down movement)

### Performance Considerations
- Target 30fps minimum for smooth streaming
- Kinect processes at 30fps max
- Use smoothing algorithms to reduce jitter
- Efficient video processing pipeline critical

## Testing & Debugging

### Test Scenarios
1. **Basic Tracking:** Head movement registers correctly
2. **Coordinate Mapping:** 3D head position maps to correct 2D screen position
3. **Scaling:** Distance changes affect feed size appropriately
4. **Smoothing:** Movement is fluid, not jittery
5. **Edge Cases:** Lost tracking, re-detection, multiple people

### Debug Tools
- Unity Scene view for visualizing Kinect data
- Real-time position/rotation display UI
- Kinect Studio for recording/playback test sessions
- Performance profiler for optimization

## Deployment Notes

### Build Requirements
- Windows x64 build target only
- Include Kinect SDK runtime with distribution
- Unity Capture Card components
- Administrative privileges may be required (virtual camera drivers)

### Distribution Checklist
- [ ] Kinect SDK 2.0 Runtime installer
- [ ] Unity Capture Card setup instructions
- [ ] Calibration guide for users
- [ ] Troubleshooting documentation
- [ ] System requirements documentation

## Known Limitations & Future Enhancements

### Current Limitations
- Windows-only (Kinect SDK requirement)
- Single user tracking
- Requires good lighting for optimal head tracking
- USB 3.0 requirement for Kinect v2

### Future Enhancement Ideas
- Multi-user support with user switching
- Custom gesture definitions
- Green screen integration with depth data
- Mobile app for remote control/calibration
- Machine learning for improved gesture recognition

## Development Commands

### WSL Development Environment
```bash
# Initial setup in WSL Ubuntu
git init
git add .
git commit -m "Initial project setup"
git remote add origin <repository-url>
git push -u origin main

# Development workflow in WSL
# Make changes, commit, and push to GitHub
git add .
git commit -m "Development changes"
git push origin main
```

### Windows Testing Environment
```bash
# Clone to Windows environment for testing
git clone <repository-url>
cd KinectWebcamController

# Pull latest changes from WSL development
git pull origin main

# Unity project setup (Windows only)
# Open Unity Hub
# Add project from folder
# Open in Unity 2022.3 LTS

# Build commands (within Unity)
# File → Build Settings → Windows x64
# Build and Run for testing
```

## Support & Resources

- **Kinect SDK Documentation:** Microsoft Developer Network
- **Unity Documentation:** docs.unity3d.com
- **Kinect Unity Wrapper:** GitHub repositories (TBD - select best option)
- **Unity Capture Card:** Asset Store or GitHub

## Quick Start Checklist

1. ✅ Install Unity 2022.3 LTS
2. ✅ Install Kinect SDK 2.0
3. ✅ Test Kinect with SDK Browser
4. ✅ Create new Unity 3D project
5. ✅ Import Kinect Unity wrapper package
6. ✅ Create basic scene with Kinect manager
7. ✅ Test head tracking with debug cube
8. ✅ Add webcam input
9. ✅ Connect head tracking to webcam positioning
10. ✅ Setup Unity Capture output

---

**Development Status:** Planning Phase
**Target Platform:** Windows 10/11
**Development Environment:** Windows (required for Kinect SDK)