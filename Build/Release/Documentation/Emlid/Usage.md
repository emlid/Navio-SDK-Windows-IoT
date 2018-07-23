# User Guide

This is an end-user guide for parts of the SDK which are deployed with your solutions built on the SDK. You may integrate this documentation into your product's own documentation or reference it here.

## IoT Configuration

Default IoT images require no modification. Optionally complete the steps below to check the settings.

1. Open the IoT device web portal and login, either directly in a browser or via the IoT Dashboard.
2. Click the "Devices" tab.
3. Select the "inbox" controller driver.
4. Reboot if the controller had to be changed.

## Hardware Test Tool

1. Launch the application.
2. Click on "Detect" to detect the Navio hardware model. The text box next to it should be filled with the correct model name within a few seconds, and the individual component test buttons will then be active for supported devices.
3. Click on any component test button to open the test screen for that component.
4. Click the "Close" button to exit a test screen.
5. Click the "Exit" button to exit the application.

## Driver Test

This is not required because no custom drivers are integrated in the current framework.

1. Enter the DEVCON command on the IoT device to remove any previous driver then reboot: To-Do...
2. Copy the SYS, INF and CER files to the "C:\Windows\System32" folder on the IoT device.
3. Copy one of the ACPITABL.DAT files to "C:\Windows\System32" folder on the IoT device. The correct file must be used according to the Navio hardware model. For Navio and Navio+ use ACPITABL1.DAT, for Navio 2 use ACPITABL2.DAT. The file name must be exactly ACPITABL.DAT in the System32 folder on the IoT device (rename during copy or afterwards).
4. Enter the DEVCON command on the IoT device to install the new driver: To-Do...
