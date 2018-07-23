## Project

In 2015 Microsoft announced official support for the Raspberry Pi 2 running Windows 10 on their new "IoT" (Internet of Things) platform. Soon after work started on an open source SDK for Navio with the following goals:

 - Enable access to all the great sensors of the Navio HAT for Windows DIY projects.
 - Prove the concept of an autopilot running a Windows OS (quadcopter in stabilized flight mode).
 - Provide the tools necessary for the community to extend existing autopilots to Windows and/or create an entirely new system.

This document describes how to develop your own code for Navio running on Windows 10 IoT, or just try-out the test and sample apps for yourself. You can follow the progress at the [Emlid forum thread](http://community.emlid.com/t/windows-10-iot-image-for-navio/381/last) or the [Hackster.io project](https://www.hackster.io/team-code-for-robots/navio-sdk-for-windows-iot). If you like it please "respect" the Hackster project to return some kudos to the creator ;-)

## Roadmap

The current framework release is known as the "user mode" framework. Later it will be converted into a Windows Runtime Component so it can be consumed by other languages than .NET, such as native C++ and NodeJS. The development language will also switch to lower-level C++ runtime components which will work in tandem with new Navio specific device drivers for performance and reliability. Once device drivers are available, a custom IoT image will also be produced to ease installation. OEM build instructions will also be included so you can integrate the individual drivers into your own IoT image.

The framework will remain, but the Navio*Device classes will be redirected to call the new lower level components. However it is inevitable that some breaking changes will occur. At least the user mode framework will continue to run on standard IoT images (without drivers installed) so you have to possibility to stay with the older NuGet package versions and upgrade when it suits you.

It remains to be seen what Microsoft will provide for provisioning and deployment of end user applications, and if there will be any other editions than "core". There could even be Windows Store support, some other kind of one-click installation. We can already provide manual download links to "side-load" standard packages onto the device via the web interface. The goal is to achieve "plug and play" installation and configuration of Navio and Navio enabled apps running Windows IoT.

### Milestone 1: "Core SDK"

Mission statement: "Full hardware support for Navio on Windows IoT."

1. LED & PWM (PCA9685) device, sample and test app (100% complete).
2. RC Input (PPM over GPIO) device, sample and test app (60% complete).
3. IMU (MPU9250) device, sample and test app.
4. Barometer (MS5611) device, sample and test app (100% complete).
5. GPS (U-Blox NEO-MN8, possibly NEO7M, NEO-6T too) device, sample and test app.
6. ADC (ADS1115) device, sample and test app.
7. FRAM (MB85RC04) device, sample and test app (100% complete).

### Milestone 2: "Windows Drone PoC"

Mission statement: "Answer the question; can you really fly a drone with Windows IoT?"

1. Motor control and PID logic for a quadcopter.
2. FMU test app, enable raw flight with RC input in stabilize mode.
3. Manual (acrobatic) mode and switch capability.
3. RTL failsafe mode and switch.
4. GPS fence failsafe with auto-correction.
5. MAVLink proxy, including Ground Control Station support.

### Future Milestones:

Mission statement: "Expand support and add new devices."

1. Support other multi-rotor types in the PoC application.
2. FrSky telemetry.
3. FrSky SPort (sensors) device(s) and test app(s).
4. U-Center GPS proxy for easy configuration / upgrade of GPS.
5. RTKLib integration for highly accurate GPS.
6. Emlid REACH integration, similar to Windows Remote Arduino or as close to full IoT Intel Galileo (Intel Edison) support as possible (depends on how good Microsoft's support is in this area).
7. Expand the Windows drone sample, possibly spinning-off a new autopilot (includes support for other vehicle types, e.g. planes and rovers).
8. Experiment with Windows HAL for APM (requires a good Linux code bridge like Cygwin, but for IoT).
9. Expand samples for other (than drone) projects, e.g. general robotics, trackers and DIY solutions.
