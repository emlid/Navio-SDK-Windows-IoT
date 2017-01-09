using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Diagnostics;
using System.Globalization;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioFram
{
    /// <summary>
    /// Start-up task.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        #region Private Fields

        /// <summary>
        /// Hardware.
        /// </summary>
        INavioBoard _board;

        /// <summary>
        /// Background task deferral, allowing the task to continue executing after the <see cref="Run(IBackgroundTaskInstance)"/> method has completed.
        /// </summary>
        BackgroundTaskDeferral _taskDeferral;

        #endregion

        #region Public Methods

        /// <summary>
        /// Application start-up.
        /// </summary>
        /// <param name="taskInstance">Task instance.</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Initialize task
            _taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;

            // Connect to hardware
            Debug.WriteLine("Connecting to Navio board.");
            _board = NavioDeviceProvider.Connect();
            Debug.WriteLine("Navio board was detected as a \"{0}\".", _board.Model);
            var fram = _board.Fram;
            if (fram == null)
            {
                // No FRAM on this board!
                Debug.WriteLine("This board does not have a FRAM chip!");
                throw new NotSupportedException();
            }
            Debug.WriteLine("FRAM has {0} bytes of memory.", fram.Size);

            // Log start
            Debug.WriteLine("Navio FRAM memory test start.");

            // Write single bytes
            Debug.WriteLine("Writing single bytes (all 1s)...");
            for (ushort address = 0; address < fram.Size; address++)
                fram.WriteByte(address, 0xff);

            // Read numbers
            Debug.WriteLine("Read single bytes (all 1s)...");
            for (ushort address = 0; address < fram.Size; address++)
            {
                var test = (address == 0) ?
                    fram.ReadByte(address) :    // First call sets address to read from,
                    fram.ReadByte();            // then following calls continue forwards
                if (test != 0xff)
                {
                    // Data error!
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                        "Invalid data, read {0:X2} but expected FF!", test));
                }
            }

            // Write numbers
            Debug.WriteLine("Writing sequential numbers...");
            for (ushort address = 0; address < fram.Size; address += 2)
            {
                var data = BitConverter.GetBytes(address);
                fram.WritePage(address, data);
            }

            // Read numbers
            Debug.WriteLine("Read of sequential numbers...");
            for (ushort address = 0; address < fram.Size; address += 2)
            {
                var data = BitConverter.GetBytes(address);
                var test = fram.ReadPage(address, 2);
                if (test.Length != data.Length || test[0] != data[0] || test[1] != data[1])
                {
                    // Data error!
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture,
                        "Invalid data, read {0:X2}{1:X2} but expected {2:X2}{3:X2}!", test[0], test[1], data[0], data[1]));
                }
            }

            // Block erase
            Debug.WriteLine("Block erase (write pages of 0s)...");
            var zeroBlock = new byte[fram.Size];
            fram.WritePage(0, zeroBlock);

            // Check erased
            Debug.WriteLine("Checking memory has been erased...");
            var testBlock = fram.ReadPage(0, zeroBlock.Length);
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

            // End
            Debug.WriteLine("Tests complete.");
            _taskDeferral.Complete();
        }

        #endregion

        #region Events

        /// <summary>
        /// Completes the task gracefully when canceled.
        /// </summary>
        /// <param name="sender">Background task instance.</param>
        /// <param name="reason">Cancellation reason.</param>
        private void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Release hardware resources
            Debug.WriteLine("Disconnecting from Navio board.");
            _board?.Dispose();

            // End execution
            Debug.WriteLine("Application finished.");
            _taskDeferral.Complete();
        }

        #endregion
    }
}
