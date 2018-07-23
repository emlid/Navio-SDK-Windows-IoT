// ============================================================================
// Navio.asl
// ----------------------------------------------------------------------------
// ACPI Machine Language (AML) source (ASL) for the Emlid Navio 2 HAT,
// when attached (as designed) to the Raspberry Pi 2 or 3 motherboard.
// ----------------------------------------------------------------------------

// TODO: Update pin-outs and hardware IDs

DefinitionBlock ("Navio.aml", "SSDT", 1, "EMLID", "NAVIO", 1)
{
    Scope (\_SB)
    {
        //
        // Navio peripheral device node
        //
        Device(NAVI)
        {
            Name(_HID, "NAVI0002")
            Name(_CID, "NAVI0002")
            Name(_UID, 1)
            Name(_CRS, ResourceTemplate()
            {
                // Index 4 - GPIO 0
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 0 }

                // Index 6 - GPIO 1
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 1 }

                // Index ? - GPIO 4 = Navio RC Input
                GpioIo(Shared, PullDefault, 0, 0, IoRestrictionInputOnly, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 4 }

                // Index 8 - GPIO 5 - Not connected
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 5 }

                // Index 10 - GPIO 6 - Not connected
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 6 }

                // Index 12 - GPIO 12
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 12 }

                // Index 14 - GPIO 13 - Not connected
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 13 }

                // Index 16 - GPIO 16 - Not connected
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 16 }

                // Index 18 - GPIO 18
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 18 }

                // Index 20 - GPIO 22
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 22 }

                // Index 22 - GPIO 23
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 23 }

                // Index 24 - GPIO 24
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 24 }

                // Index 26 - GPIO 25
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 25 }

                // Index 28 - GPIO 26
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 26 }

                // Index 30 - GPIO 27
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 27 }

                // Index 32 - GPIO 35
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 35 }

                // Index 34 - GPIO 47
                //GpioIo(Shared, PullDefault, 0, 0, IoRestrictionNoneAndPreserve, "\\_SB.GPI0", 0, ResourceConsumer, , ) { 47 }
            })
        }
    }
}
