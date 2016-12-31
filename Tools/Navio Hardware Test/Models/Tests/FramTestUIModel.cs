using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// UI model for testing the <see cref="NavioFramDevice"/>.
    /// </summary>
    public class FramTestUIModel : TestUIModel
    {
        #region Constants

        /// <summary>
        /// Number of bytes to display per line in <see cref="Contents"/>.
        /// </summary>
        public const int ContentBytesPerLine = 32;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public FramTestUIModel(ApplicationUIModel application) : base(application)
        {
            // Initialize members
            Contents = "";
            FillByte = 0xFF;

            // Initialize device
            Device = application.Board.Fram;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Device.
        /// </summary>
        public INavioFramDevice Device { get; private set; }

        /// <summary>
        /// Memory content view.
        /// </summary>
        public string Contents { get; private set; }

        /// <summary>
        /// Value to use during <see cref="Fill"/>.
        /// </summary>
        public byte FillByte { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads all memory and stores in <see cref="Contents"/>.
        /// </summary>
        public void Read()
        {
            RunTest(delegate { UpdateContents(); });
        }

        /// <summary>
        /// Writes zeros to the entire memory area.
        /// </summary>
        public void Erase()
        {
            RunTest(delegate
            {
                // Erase
                WriteOutput("Erase memory (write pages of 0s)...");
                var zeroBlock = new byte[Device.Size];
                Device.WritePage(0, zeroBlock);

                // Display resulting memory
                UpdateContents();

                // Check erased
                WriteOutput("Verify memory has been erased...");
                var testBlock = Device.ReadPage(0, zeroBlock.Length);
                for (var address = 0; address < zeroBlock.Length; address++)
                {
                    var test = testBlock[address];
                    if (test != 0)
                    {
                        // Data error!
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                            "Invalid data, read {0:X2} but expected 00!", address + address));
                    }
                }
            });
        }

        /// <summary>
        /// Writes a single byte value to the entire memory area.
        /// </summary>
        public void Fill()
        {
            RunTest(delegate
            {
                // Write single bytes
                var fillByte = FillByte;
                WriteOutput("Fill memory (all 0x{0:X2}s)...", fillByte);
                for (ushort address = 0; address < Device.Size; address++)
                    Device.WriteByte(address, fillByte);

                // Display resulting memory
                UpdateContents();

                // Read numbers
                WriteOutput("Verify (all 0x{0:X2}s)...", fillByte);
                for (ushort address = 0; address < Device.Size; address++)
                {
                    var test = (address == 0) ?
                        Device.ReadByte(address) :    // First call sets address to read from,
                        Device.ReadByte();            // then following calls continue forwards
                    if (test != fillByte)
                    {
                        // Data error!
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                            "Invalid data, read {0:X2} but expected {1:X2}!", test, fillByte));
                    }
                }
            });
        }

        /// <summary>
        /// Writes a sequence of numbers to the entire memory area.
        /// </summary>
        public void Sequence()
        {
            RunTest(delegate
            {
                // Write numbers
                WriteOutput("Writing sequential numbers...");
                for (ushort address = 0; address < Device.Size; address += 2)
                {
                    var data = BitConverter.GetBytes(address);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data);
                    Device.WritePage(address, data);
                }

                // Display resulting memory
                UpdateContents();

                // Read numbers
                WriteOutput("Verify numbers in sequence...");
                for (ushort address = 0; address < Device.Size; address += 2)
                {
                    var data = BitConverter.GetBytes(address);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(data);
                    var test = Device.ReadPage(address, 2);
                    if (test.Length != data.Length || test[0] != data[0] || test[1] != data[1])
                    {
                        // Data error!
                        throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                            "Invalid data, read {0:X2}{1:X2} but expected {2:X2}{3:X2}!", test[0], test[1], data[0], data[1]));
                    }
                }
            });
        }

        /// <summary>
        /// Clears all content.
        /// </summary>
        public override void Clear()
        {
            // Call base class to clear output
            base.Clear();

            // Clear graph
            Contents = "";

            // Update display
            DoPropertyChanged(nameof(Contents));
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Runs a test method with status and error output.
        /// </summary>
        /// <param name="test">Test delegate to run.</param>
        /// <param name="name">Name to use in the output.</param>
        protected override void RunTest(Action test, [CallerMemberName] string name = "")
        {
            // Call base class to run test
            base.RunTest(test, name);

            // Update properties
            DoPropertyChanged(nameof(Device));
        }

        /// <summary>
        /// Reads all memory, formats for display then updates <see cref="Contents"/>.
        /// </summary>
        private void UpdateContents()
        {
            // Read all memory
            WriteOutput("Reading all memory...");
            var size = Device.Size;
            var contentBytes = Device.ReadPage(0, size);

            // Format
            WriteOutput("Formatting for display...");
            var contents = new StringBuilder();
            for (var address = 0; address < size; address += ContentBytesPerLine)
            {
                // Address
                contents.AppendFormat(CultureInfo.InvariantCulture, "{0:X4} ", address);

                // Hexadecimal
                for (var byteIndex = 0; byteIndex < ContentBytesPerLine; byteIndex++)
                {
                    var byteAddress = address + byteIndex;
                    if (byteAddress < size)
                        contents.AppendFormat(CultureInfo.InvariantCulture, "{0:X2} ", contentBytes[byteAddress]);
                    else
                        contents.Append("   ");
                }

                // ASCII
                var dataSize = (address + ContentBytesPerLine < size) ? ContentBytesPerLine : size - address;
                var text = Encoding.ASCII.GetString(contentBytes, address, dataSize).FilterSpecial();
                contents.Append(text);

                // Next line...
                contents.AppendLine();
            }

            // Update display
            Contents = contents.ToString();
            DoPropertyChanged(nameof(Contents));
        }

        #endregion
    }
}
