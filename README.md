# Navio SDK for Windows IoT

Windows IoT SDK for Emlid's hardware devices.
Supports the Navio+ autopilot shield for Raspberry Pi 2 running the standard Microsoft Windows 10 IoT Core image. 


*Work in Progress!*

We encourage users to play with the samples and test programs. Developers can also start using the framework for their own applications. However, development contributions to the framework and test apps themselves are best avoided until we are finished with the core SDK.


# Usage

1. Install Visual Studio 2015 Community edition or greater (not Express).
2. Install the IoT tools for Visual Studio.
3. Create a Universal Windows Platform project and add the NuGet package "Emlid.WindowsIoT.Hardware" (https://www.nuget.org/packages/Emlid.WindowsIoT.Hardware).
4. Add a reference to the "Emlid.WindowsIoT.Hardware" namespace and start using the framework!


# Roadmap

## Milestone 1: "Core SDK"

Mission statement: "Full hardware support for Navio on Windows IoT."

1. LED & PWM (PCA9685) device, sample and test app (100% complete).
2. RC Input (PPM over GPIO) device, sample and test app (60% complete).
3. IMU (MPU9250) device, sample and test app.
4. Barometer (MS5611) device, sample and test app.
5. GPS (U-Blox NEO-MN8, possibly NEO7M, NEO-6T too) device, sample and test app.
6. ADC (ADS1115) device, sample and test app.
7. FRAM (MB85RC04) device, sample and test app.
 

## Milestone 2: "Windows Drone PoC"

Mission statement: "Answer the question; can you really fly a drone with Windows IoT?"

1. Motor control and PID logic for a quadcopter.
2. FMU test app, enable raw flight with RC input in stabilize mode.
3. Manual (acrobatic) mode and switch capability.
3. RTL failsafe mode and switch.
4. GPS fence failsafe with auto-correction.
5. MAVLink proxy, including Ground Control Station support.


## Future Milestones:

Mission statement: "Expand support and add new devices."

1. Support other multirotor types in the PoC application.
2. FrSky telemetry.
3. FrSky SPort (sensors) device(s) and test app(s).
4. U-Center GPS proxy for easy configuration / upgrade of GPS.
5. RTKLib integration for highly accurate GPS.
6. Emlid REACH integration, similar to Windows Remote Arduino or as close to full IoT Intel Galileo (Intel Edison) support as possible (depends on how good Microsoft's support is in this area).
7. Expand the Windows drone sample, possibly spinning-off a new autopilot (includes support for other vehicle types, e.g. planes and rovers).
8. Experiment with Windows HAL for APM (requires a good Linux code bridge like Cygwin, but for IoT).
9. Expand samples for other (than drone) projects, e.g. general robotics, trackers and DIY solutions.


# Change Log

*2015.10.06*

1. RC input C# proof of concept complete (so far as worthwhile). Only accurate to ~1 second due to "floating GPIO" limitation (see notes below). 
2. Completed RC Input in hardware test app and added new RC Input C# sample.
2. Added NuGet package. No need to download source and compile to use the hardware from now on! The package URL is: https://www.nuget.org/packages/Emlid.WindowsIoT.Hardware

Due to a Microsoft IoT image limitation, specifically with the GPIO #4 pin hard-wired to RC Input on the Navio, we cannot achieve any kind of acceptable performance in "user mode" code.
Furthermore, the current UWP API has a lack of support for time critical threads in "user mode". It is necessary to bring forwards the conversion to C++ and device drivers.
The next release will include only drivers and provide access via a C++/CX coded Windows Runtime Component. C# code and any other language can then be used as normal for user applications.


*2015.08.25*

1. RC input progress. Test app enables use of RC Input, but no GUI interaction yet. For now use the Output window of Visual Studio to see debug messages repoting decoded PPM values. They appear to be correct, but some tuning of rounding and frame error detection is required.


*2015.08.14*

1. RC Input hardware classes done in their first untested form.
2. RC Input model and preliminary view added to hardware test app, but not enabled yet.
3. Minor clean-up and coding standards in other classes and projects.


*2015.07.31*

1. Updated solution to Visual Studio 2015 RTM.
2. Hardware Test App - Fixed theme/resource errors and applied "Emlid green" instead of default "IoT pink".
3. SetLED - Fixed error in buffer write routine.
