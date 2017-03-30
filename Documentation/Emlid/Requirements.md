## Requirements

### IoT Hardware

IoT devices are supported with the following hardware:

+ [Raspberry Pi 3 Model B](https://www.raspberrypi.org/products/raspberry-pi-3-model-b/), or the older [Raspberry Pi 2 Model B](https://www.raspberrypi.org/products/raspberry-pi-2-model-b/).
+ [Emlid Navio 2 HAT](https://emlid.com/shop/navio2/), the older Navio+ or Navio.
+ Optional [WiFi dongle](https://developer.microsoft.com/en-us/windows/iot/docs/hardwarecompatlist#WiFi-Dongles) for easier communication with the device. The Pi 3 has built-in wireless, but the Pi 2 does not so a dongle is recommended with that.

Compatible IoT boards may also work, but are not directly supported.

### Development Hardware:

* Workstation or laptop powerful enough to use as a [Visual Studio development system](https://www.visualstudio.com/en-us/productinfo/vs2015-sysrequirements-vs).
* Optional WiFi network adapter or LAN connection to WiFi router (recommended).

### Software

This section describes the software tools and applications required to deploy images and applications, develop applications or even the SDK itself.

#### Running Applications

The IoT device must be running the "Insider Preview" build **15063** or later is required to run the current SDK, which is rolled-out by Microsoft as a update to the latest available image, e.g. 15063 from early 2017.

The "IoT Dashboard" tool eases the process of downloading the OS image to the Raspberry Pi 2, then helps you connect and configure devices over the network. End-users need only install the dashboard, or you could provide them with a preloaded SD card.

Alternatively you can download the ISO image and release notes here:

[Other Downloads](http://ms-iot.github.io/content/en-US/Downloads.htm)

#### Developing Applications with the SDK

Adding the SDK to your solution is easy as it is officially released and updated as a standard Microsoft Visual Studio NuGet package. Search for "Emlid.WindowsIot.Hardware" in the NuGet Package Manager with the "pre-release" option set.

The development PC must be running Visual Studio 2017 (Community edition or greater) on top of the latest "Insider Preview" OS release, greater than build 10.0.**15063**. Similarly, the latest Insider Preview SDK is required, currently build **15063** from early 2017.

Links to download the development tools and basic instructions what to install are provided by Microsoft here:

[Getting Started with Windows IoT](http://ms-iot.github.io/content/en-US/GetStarted.htm)

#### Building or Developing the SDK

In addition to the software required for application development, the following are required to build or develop the SDK itself.

Since the solution contains device driver source code, you will also need to install the latest "Insider Preview" Windows Driver Kit (WDK). This is available in the "Install WDK 10" link on this page:

[Driver Kit Downloads](https://msdn.microsoft.com/en-US/windows/hardware/dn913721%28v=vs.8.5%29.aspx?f=255&MSPPError=-2147217396)
