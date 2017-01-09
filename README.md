# Navio SDK for Windows IoT

This is the Windows IoT SDK for Emlid's Navio autopilot shield. It enables support for Microsoft Windows 10 IoT running on Raspberry Pi 2, 3 and compatible boards. All Navio models are supported.

**IMPORTANT:** Whilst we encourage users to play with the samples and test programs, this project has not yet reached a stable state. Developers could start using the framework for their own applications. However, development contributions to the framework and test applications themselves are best avoided until core SDK is finished.

## Disclaimer

All source, documentation, instructions and products of this project are provided as-is without warranty. No liability is accepted for any damage or costs incurred by its use. Laws restrict or prevent use remote control and autonomous hardware and telecommunications in different areas of the world. The public availability of the hardware and this SDK does not imply carefree usage. Inform yourself before about the necessary safety and legal requirements before planning and implementing any kind of solution.

## License & Copyright

**Copyright &copy; Tony Wall & Emlid. All rights reserved.**

The framework and tools design, architecture and source code were created by Tony Wall (full name Anthony Brian Wall, web site http://tonywall.com) as a personal project for Emlid (full company name Emlid Limited, web site http://emlid.com) who host the project on their official GitHub repository.

**Licensed under the Apache 2.0 License.**

The source code and software packages are licensed under the Apache 2.0 license. Full details are available here: http://www.apache.org/licenses/LICENSE-2.0

This is a commonly known as an "open source" public license for "free use" by both private and commercial purposes, however it does not come without restrictions! For example:

* Copying and redistribution in part or whole is only allowed with attribution to the creator and project contributors and reference to the license.
* You can sell solutions created with the SDK only with attribution and license.
* You can use parts of the source in your own development only with attribution and license.
* You can use the source or products as part of a training course only as free content with attribution and license.
* You cannot sell the SDK itself (which is already free) or any of its components, only products and services created with it.
* You cannot sell a service to produce code which is just copied from the SDK, just your time on your own parts and for integration of the SDK.

Do not continue to use the source, download or work with the products unless you have read and agree to the license and terms above.

## Known Issues

1. The "Insider Preview" (beta) IoT images, development tools and workstation OS are required because the Microsoft tools and APIs from those versions are required. Once stable RTM releases are available of the Microsoft framework and IoT images it is preferred to keep the SDK running only on RTM versions.
2. Software RC input, required on older Navio and Navio+ models, is not currently usable because of limitations of both the standard Microsoft "inbox" and "lightning" GPIO providers. Microsoft are providing new features in an upcoming release which may make this possible. Remote control should be achieved via other methods, e.g. wireless joystick or ground station commands.
3. Hardware RC input, possible on Navio 2, is not currently implemented. So RC input can only be tested on Navio and Navio+ models.
4. Driver development is currently lowest priority as it is no longer necessary to achieve a first working solution. For the Navio and Navio+ models Microsoft will provide buffered IO to solve the RC input problem. On the current Navio 2 model, it doesn't need faster IO because it has a dedicated co-processor to remove the burden off the CPU for RC input decoding, PWM encoding and ADC. The WDK required to develop drivers also has limitations which hinder development. Hence the goal is to focus on core SDK functionality before re-working into lower level code.

## Documentation

* [Change Log](Documentation/Emlid/Changes.md)
* [Project History & Roadmap](Documentation/Emlid/Project.md)
* [Requirements](Documentation/Emlid/Requirements.md)
* [User Guide](Documentation/Emlid/Usage.md)
* [Developer Guide](Documentation/Emlid/Development.md)
* [Coding Standards](Documentation/Emlid/Coding-Standards.md)
