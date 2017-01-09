## Developer Guide

This section describes how to start development by installing Windows IoT on your device, Visual Studio on your PC, then finally how to clone, build and run the code.

The current Navio SDK "framework" is a Windows Universal library, so can be consumed by any Windows Universal application written in any .NET language supported by IoT. Simply use the NuGet Package Manager of Visual Studio to add the [Emlid.WindowsIot.Hardware](https://www.nuget.org/packages/Emlid.WindowsIoT.Hardware) package, then write some code with the various Navio*Device classes to utilize the hardware.

Deployment is easy with the Visual Studio produced Windows Universal packages. They are precompiled and load directly onto the IoT device. You can deploy them via Visual Studio during development, via the web interface for end users, script them with PowerShell or include them on a preloaded SD card image.


### Preparation

It is strongly recommended you complete some basic training before attempting to build and deploy your first IoT project. Even experienced Windows developers will have to get to grips with some of the special build and deployment requirements for the ARM processor and remote IoT device connections. Also the IoT system is still under early development, even though the desktop Windows 10 builds are finished.

 1. Follow instructions to build, deploy and debug Microsoft's [IoT "Hello World" sample](http://ms-iot.github.io/content/en-US/win10/samples/HelloWorld.htm).
 2. In Visual Studio open the "Tools - Extensions and Updates..." dialog.
 3. Select the "Updates" tab then update the GitHub extension and "Windows IoT Core Project Templates".
 4. Optionally update all others items (multiple starts will be necessary).
Update the GitHub extension (menu "Tools - Extensions and Updates...").
 5. Select the "Online" tab, search for then install the "Productivity Power Tools 2015" extension.


### Building the SDK from Source

1. Click "Team Explorer" then the "Plug" icon (manage connections).
2. Under "Local Git Repositories" click the "Clone" link then enter the URL of the GitHub project "[https://github.com/emlid/Navio-SDK-Windows-IoT](https://github.com/emlid/Navio-SDK-Windows-IoT)", choose a local path into which the files will be downloaded then the "Clone" button.
3. Open the solution by selecting the "File - Open - Project/Solution..." menu option then the solution (.sln) file from the local repository directory you just cloned.
4. In the Solution Explorer window, right-click the solution root then select "Power Commands - Open Command Prompt" and enter the commands:
  ```
  cd Tools\Scripts
  PowerShell.exe -File "Setup Local.ps1"
  exit
  ```
5. On the build toolbar, ensure that "Debug" configuration and "ARM" platform is selected.
6. In the "Build" menu select "Rebuild Solution". 
7. Some NuGet packages may be loaded, then the build will start.
8. Once per workstation: Visual Studio requires that the temporary signing keys are installed in a per-machine unique "VS_KEY_HHHHHHHHHHHHHHHH" container. Copy the container name from the error message, open a command prompt at the solution folder then run the following commands (with your unique VS_KEY value):
  ```
  cd Common
  "TemporaryKey Install.cmd" VS_KEY_HHHHHHHHHHHHHHHH
  ```
9. Rebuild the solution, it should now complete without any errors.


### Build and Run the Hardware Test App

The Hardware Test app is currently only distributed as source. Later it will also be available for stand-alone use (without any development tools).

1. Ensure the Remote Debugging Tools are running on the IoT device. Use the web interface (easily accessible via the dashboard tool) "Debugging" page to start it and also check the remote name and port, which must match in your deployment configuration.
2. On the build toolbar, ensure the "ARM" processor is selected and the "Navio Hardware Test" startup project.
3. Click the drop-down arrow next to the green "play" icon, then "Remote Machine".
4. If this is the first time you deployed to a remote machine the configuration dialog will appear, then you should enter the "name:port" of the target device and choose no authentication.
5. With the remote machine still selected, click the "Play" icon to build, deploy and run the sample.
6. The solution should compile, deploy and run on the Navio/RasPi. Use the mouse to select "LED & PWM" (the rest is not implemented yet). Check the frequency, set some sliders (best PWM near bottom) then click the Output ON.
7. Play with the sliders to test your LED and servos/ESCs.
8. Monitor the output window for any messages, and look at the physical device for any feedback, e.g. LED colours or servo movement.
9. Click "Close" then "Exit" to finish.


### Build and Run the Samples

Currently the sample applications are "Background Application" projects, which means they run directly from Visual Studio with no UI. All output is sent back via debug messages to the Visual Studio "Output" window.

1. Ensure the Remote Debugging Tools are running on the IoT device. Use the web interface (easily accessible via the dashboard tool) "Debugging" page to start it and also check the remote name and port, which must match in your deployment configuration.
2. On the build toolbar, ensure the "ARM" processor is selected and the "Navio Hardware Test" startup project.
3. Click the drop-down arrow next to the green "play" icon, then "Remote Machine".
4. If this is the first time you deployed to a remote machine the configuration dialog will appear, then you should enter the "name:port" of the target device and choose no authentication.
5. With the remote machine still selected, click the "Play" icon to build, deploy and run the sample.

Monitor the output window for any messages, and look at the physical device for any feedback, e.g. LED colors or servo movement.


### Contributing

If you wish to contribute to the project, it is necessary to understand the following:

1. Until the Core SDK is finished, no pull requests will be considered. That is to save our time as it is extremely likely that your changes are either already implemented or in the progress of being implemented entirely differently. The call API will only be considered a "contract" (which we want to support backwards compatible) after the Core SDK is ready.
2. The standard of coding is very high and robust, common as you will see in professional enterprise class development teams. You may find this a steep learning curve. Don't be upset if your code is rejected for this purpose. If you see an error in the code and do not have time to follow the coding standards it would be better to open an issue and let another developer fix it properly rather than making a pull request.

[The coding standards are here.](Coding-Standards.md)
