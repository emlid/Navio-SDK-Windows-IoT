# Navio SDK for Windows IoT

Windows IoT SDK for Emlid's hardware devices.
Supports the Navio+ autopilot shield for Raspberry Pi 2 running the standard Microsoft Windows 10 IoT Core image. 


*Work in Progress!*

We encourage users to play with the samples and test programs. Developers can also start using the framework for their own applications. However, development contributions to the framework and test apps themselves are best avoided until we are finished with the core SDK.


# Usage

Normal "consumer" (developer) use is made easy via the standard NuGet packages, publically hosted and distributed by nuget.org. You do not need GitHub or any of the source code to just use the libraries. So if you just want to run the samples it may be easier to use the ZIP download option on GitHub.com.

1. Install Visual Studio 2015 Community edition or greater (but not Express) with developer tools (required for IoT). https://go.microsoft.com/fwlink/p/?LinkId=534599
2. Create your preferred Universal Windows Platform project (various languages and program/component types).
3. On the project(s) directly using the Emlid hardwre, right click then "Manage NuGet Packages", select "All" and enable "pre-release", then search for "Emlid.WindowsIot.Hardware" (https://www.nuget.org/packages/Emlid.WindowsIot.Hardware).
4. Enter some code which uses the hardware classes (see samples for ideas).
5. On the toolbar select "Debug" build configuration and "ARM" processor architecture, then click "Remote Machine..." to build, deploy and run on your IoT device. See the Microsoft IoT samples for instructions how to connect.


# Building

Developers interested in how the whole SDK and Framework works itself need to install the GitHub Visual Studio extension and clone the repository.

1. Install Visual Studio 2015 Community edition or greater (but not Express) with developer tools (required for IoT). https://go.microsoft.com/fwlink/p/?LinkId=534599
2. Add/Update GitHub extension in Visual Studio. https://visualstudio.github.com
3. Connect to source control and download (select Team Explorer, click manage connections icon/green plug, under "Local Git Repositories" click "Clone", enter the URL for this repository, choose your working folder then "Clone").
4. On the project(s) directly using the Emlid hardwre, right click then "Manage NuGet Packages", select "All" and enable "pre-release", then search for "Emlid.WindowsIot.Hardware" (https://www.nuget.org/packages/Emlid.WindowsIot.Hardware).
5. Run the "\Common\TemporaryKey Setup.cmd" (from file explorer or command prompt) and enter a blank password as instructed.
6. On the toolbar select "Debug" build configuration and "ARM" processor architecture, then click "Remote Machine..." to build, deploy and run on your IoT device. See the Microsoft IoT samples for instructions how to connect.


# Roadmap

## Milestone 1: "Core SDK"

Mission statement: "Full hardware support for Navio on Windows IoT."

1. LED & PWM (PCA9685) device, sample and test app (100% complete).
2. RC Input (PPM over GPIO) device, sample and test app (60% complete).
3. IMU (MPU9250) device, sample and test app.
4. Barometer (MS5611) device, sample and test app (100% complete).
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

*2015.12.28* v1.0.6

1. Barometer (pressure and temperature sensor) implemented.
2. LED cycle feature added to LED & PWM test app.

Continued development of the existing C# sensor library. Note the barometer temperature is always about hotter that then environment because of heating by surrounding components, e.g. the Raspberry Pi processor is sitting underneath it. For that reason you will also see the temperature rise in the hardware test app, because the processor is being taxed with the GUI functionality.


*2015.12.22*
 
1. Device driver "capability" proof of concept. Working skeleton RC input kernel mode WDF driver which successfully deploys and can be communicated with from a user mode Universal application.
2. Various patches and documentation updates to existing source code and samples.
3. Updated solution to work with latest IoT December 2015 OS build 10586, SDK/WDK 1511, Visual Studio 2015 Update 1.
 
This is an interim check-in which does not add any end-user (consumer developer) functionality, but solves the technical requirement to be able to write a kernel mode driver and communicate from user mode.
Because of the difficulties with the VS2015 tools and lack of improvement in update 1 (which actually got worse in some aspects) focus will temporarily shift back to the user mode framework for all other features than RC Input.
Also with the release of Navio 2 which has RC Input hardware, the device driver can wait a little while, so we can first gain the full chipset functionality in user mode and maybe see if a simple quadcopter can hover with it (and a joystick instead of RC input).
Commitment is still there to produce high performance (real-time) and robust drivers shortly after a successful user mode PoC.


*2015.10.06*

1. RC input C# proof of concept complete (so far as worthwhile). Only accurate to ~1 second due to "floating GPIO" limitation (see notes below). 
2. Completed RC Input in hardware test app and added new RC Input C# sample.
2. Added NuGet package. No need to download source and compile to use the hardware from now on! The package URL is: https://www.nuget.org/packages/Emlid.WindowsIot.Hardware

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
