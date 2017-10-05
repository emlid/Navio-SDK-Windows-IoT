## Change Log

*2017.10.05* v1.0.14

Full Visual Studio 2017 compatibility (drivers too).

* Updated for preview WSDK/SDK supporting Device Driver development in Visual Studio 2017.
* Windows Insider Preview SDK and WSDK build 16288 are now required, until the WDK is finally released.
* Setup script to make first time development setup more intuitive.


*2017.05.01* v1.0.13

Minor code update but significant in that all development tools are now release builds.

1. Updated solution to Visual Studio 2017 Update 1 and Windows Creators Update SDK/WDK 10.0.15063 release.
2. Microsoft .NET Core package/dependency updated to v5.3.3.
3. Application UI model initialization is no longer asynchronous by default, instead hardware initialization code is invoked on a background thread only where required.
4. RCIO test tool initialization work and clean error message for wrong model.


*2017.03.29* v1.0.12

1. Updated solution to Windows Insider Preview "Creators Update" SDK/WDK 10.0.15063.0.
2. Avoid hardware detection after initialization (return running hardware model). Also fixes errors in test GUI after clicking "Detect" a second time.
3. All composite devices clean-up when failures occur during initialization. Boards and devices with more than one pin/chip must close them should a failure occur in their constructor, e.g. corrupt data or use of wrong hardware model.
4. Integrated VSWhere tool dependency and created VSWhereDev.cmd script to correctly initialize Visual Studio 2017 environment variables after the new limitation that Microsoft no longer set the %VS###COMNTOOLS% variable during installation. See: https://developercommunity.visualstudio.com/content/problem/26780/vsdevcmdbat-changes-the-current-working-directory.html


*2017.03.21* v1.0.11

1. Updated solution to Visual Studio 2017 RTM + "Update Preview", Windows Insider Preview/SDK/WDK 10.0.15052.0.
2. Removed Lightning providers. Microsoft clearly stopped developing and supporting it. It does not work properly anymore. Either we get a new converged driver from them soon or finish writing our own.
3. Second SPI bus access solved. Works with new transisional "HardwarePlus" C++ Windows Runtime Component (doesn't work with .NET component).
4. Navio 2 RCIO device and it's PX4IO based protocol implemented. Still integrating RC support as the first tested use case.
5. Updated .NET Core SDK package to latest stable version 5.3.1.
6. C++/WinRT headers updated to latest 10.0.15042.0 "Creators Update" release (most likely to change again until final "Creators Update" build number is fixed by Microsoft).
7. Hardware detection less obtrusive, i.e. try not throw in normal paths.

Start of the migration to C++ low level components. Drivers not necessary at this time, they will continue development as soon as a functioning WDK toolset (with VS2017) is released.
Visual Studio 2015 and Windows RTM are no longer supported. The SDK and WDK build 15052 are required and the OS must be the same build to open any of the XAML projects.


*2017.01.09* v1.0.9

1. Tested with latest insider preview 10.0.14393.1000.
2. Updated and cleaned various solution and project issues caused by previous updates and betas.
3. Updated .NET Core SDK package to latest preview 5.3.0-beta2.
4. Major refactor of the API to support multiple hardware models (Navio, Navio+ and Navio 2) with hardware detection.
5. New component independent interfaces for boards and chips, allows same code to work with any model. This is not final!
6. Updated and fixed various bugs in samples and test tool.

Major update and refactoring to prepare for final development of outstanding device support in the core SDK.
Only works with the "inbox" (non-lightning) provider in current Insider Preview builds because Microsoft are consolidating it into one faster API which supports buffered GPIO (what we need for RC input on Navio and Navio+).


*2016.08.24* v1.0.8

1. Updated solution to Visual Studio 2015 Update 3 and Windows 10 SDK and WDK version 1607 build 10.0.14393.
2. Support new Microsoft Lightning provider v1.1.0 for faster hardware access. Now referenced via NuGet package, removing the need for manually built dependencies.
3. Updated .NET Core SDK package to current 5.2.2.
4. Refactored hardware source into separate subdirectories/namespaces so it is easier to manage and use.
5. Addressed some minor bugs in the GUI.
6. General refactoring and tidy-up.
7. Improved setup scripts and instructions.


*2016.01.02* v1.0.7

1. FRAM memory device implemented, both the MB85RC256V of Navio+ and MB85RC04V of Navio.
2. General test app GUI display, usability and multi-threading improvements.

Continued development of the core SDK. FRAM complete and fully tested on Navio+ device. Support for the original Navio's FRAM chip had been coded but has not been fully tested.


*2015.12.28* v1.0.6

1. Barometer (pressure and temperature sensor) implemented.
2. LED cycle feature added to LED & PWM test app.

Continued development of the existing C# sensor library. Note the barometer temperature is always about hotter that then environment because of heating by surrounding components, e.g. the Raspberry Pi processor is sitting underneath it. For that reason you will also see the temperature rise in the hardware test app, because the processor is being taxed with the GUI functionality.


*2015.12.22* v1.0.5
 
1. Device driver "capability" proof of concept. Working skeleton RC input kernel mode WDF driver which successfully deploys and can be communicated with from a user mode Universal application.
2. Various patches and documentation updates to existing source code and samples.
3. Updated solution to work with latest IoT December 2015 OS build 10586, SDK/WDK 1511, Visual Studio 2015 Update 1.
 
This is an interim check-in which does not add any end-user (consumer developer) functionality, but solves the technical requirement to be able to write a kernel mode driver and communicate from user mode.
Because of the difficulties with the VS2015 tools and lack of improvement in update 1 (which actually got worse in some aspects) focus will temporarily shift back to the user mode framework for all other features than RC Input.
Also with the release of Navio 2 which has RC Input hardware, the device driver can wait a little while, so we can first gain the full chipset functionality in user mode and maybe see if a simple quadcopter can hover with it (and a joystick instead of RC input).
Commitment is still there to produce high performance (real-time) and robust drivers shortly after a successful user mode PoC.


*2015.10.06* v1.0.4

1. RC input C# proof of concept complete (so far as worthwhile). Only accurate to ~1 second due to "floating GPIO" limitation (see notes below). 
2. Completed RC Input in hardware test app and added new RC Input C# sample.
3. Added NuGet package. No need to download source and compile to use the hardware from now on! The package URL is: https://www.nuget.org/packages/Emlid.WindowsIot.Hardware

Due to a Microsoft IoT image limitation, specifically with the GPIO #4 pin hard-wired to RC Input on the Navio, we cannot achieve any kind of acceptable performance in "user mode" code.
Furthermore, the current UWP API has a lack of support for time critical threads in "user mode". It is necessary to bring forwards the conversion to C++ and device drivers.
The next release will include only drivers and provide access via a C++/CX coded Windows Runtime Component. C# code and any other language can then be used as normal for user applications.


*2015.08.25*

1. RC input progress. Test app enables use of RC Input, but no GUI interaction yet. For now use the Output window of Visual Studio to see debug messages reporting decoded PPM values. They appear to be correct, but some tuning of rounding and frame error detection is required.


*2015.08.14*

1. RC Input hardware classes done in their first untested form.
2. RC Input model and preliminary view added to hardware test app, but not enabled yet.
3. Minor clean-up and coding standards in other classes and projects.


*2015.07.31*

1. Updated solution to Visual Studio 2015 RTM.
2. Hardware Test App - Fixed theme/resource errors and applied "Emlid green" instead of default "IoT pink".
3. SetLED - Fixed error in buffer write routine.
