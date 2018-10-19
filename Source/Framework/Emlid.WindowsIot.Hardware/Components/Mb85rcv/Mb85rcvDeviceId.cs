using System;

namespace Emlid.WindowsIot.Hardware.Components.Mb85rcv
{
    /// <summary>
    /// Encapsulates MB85RC#V device identifier data (3 bytes), allowing the
    /// individual parts to be easily accessed and validated.
    /// </summary>
    public struct Mb85rcvDeviceId : IEquatable<Mb85rcvDeviceId>
    {
        #region Constants

        /// <summary>
        /// Original Fujitsu FRAM manufacturer ID.
        /// </summary>
        public const int FujitsuManufacturerId = 0x00a;

        /// <summary>
        /// Size in bytes.
        /// </summary>
        public const int Size = 3;

        /// <summary>
        /// Masks a valid manufacturer ID (only 3 bytes of an integer).
        /// </summary>
        public const int ManufacturerIdMask = 0x0fff;

        /// <summary>
        /// Masks a valid product ID (only 3 bytes of an integer).
        /// </summary>
        public const int ProductIdMask = 0x0fff;

        /// <summary>
        /// Masks a valid product density (only 4 bits of a byte).
        /// </summary>
        public const int ProductDensityMask = 0x0f;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance from data bytes.
        /// </summary>
        /// <param name="data">Device ID data bytes.</param>
        public Mb85rcvDeviceId(byte[] data)
        {
            // Validate
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length != Size) throw new ArgumentOutOfRangeException(nameof(data));

            // Initialize members
            Data1 = data[0];
            Data2 = data[1];
            Data3 = data[2];
        }

        /// <summary>
        /// Creates and instance from a manufacturer ID and product data.
        /// </summary>
        /// <param name="manufacturerId">Manufacturer ID.</param>
        /// <param name="productDensity">Product density.</param>
        /// <param name="productData">Product data.</param>
        public Mb85rcvDeviceId(int manufacturerId, byte productDensity, byte productData)
        {
            // Validate
            if ((manufacturerId & ~ManufacturerIdMask) != 0) throw new ArgumentOutOfRangeException(nameof(manufacturerId));
            if ((productDensity & ~ProductDensityMask) != 0) throw new ArgumentOutOfRangeException(nameof(productDensity));

            // Initialize members
            Data1 = (byte)(manufacturerId >> 4);
            Data2 = (byte)((manufacturerId << 4) | productDensity);
            Data3 = productData;
        }

        /// <summary>
        /// Creates and instance from manufacturer and product IDs.
        /// </summary>
        /// <param name="manufacturerId">Manufacturer ID.</param>
        /// <param name="productId">Product ID.</param>
        public Mb85rcvDeviceId(int manufacturerId, int productId)
        {
            // Validate
            if ((manufacturerId & ~ManufacturerIdMask) != 0) throw new ArgumentOutOfRangeException(nameof(manufacturerId));
            if ((productId & ~ProductIdMask) != 0) throw new ArgumentOutOfRangeException(nameof(productId));

            // Initialize members
            Data1 = (byte)(manufacturerId >> 4);
            Data2 = (byte)((manufacturerId << 4) | (productId >> 8));
            Data3 = (byte)productId;
        }

        #endregion Lifetime

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(Mb85rcvDeviceId left, Mb85rcvDeviceId right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(Mb85rcvDeviceId left, Mb85rcvDeviceId right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="something">Object with which to compare by value.</param>
        public override bool Equals(object something)
        {
            return (something is Mb85rcvDeviceId other) && Equals(other);
        }

        /// <summary>
        /// Compares this object with another of the same type by value.
        /// </summary>
        /// <param name="other">Object with which to compare by value.</param>
        public bool Equals(Mb85rcvDeviceId other)
        {
            return
                other.Data1 == Data1 &&
                other.Data2 == Data2 &&
                other.Data3 == Data3;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Data1.GetHashCode() ^
                Data2.GetHashCode() ^
                Data3.GetHashCode();
        }

        #endregion Operators

        #region Properties

        #region Raw Data

        /// <summary>
        /// First data byte.
        /// </summary>
        public byte Data1 { get; set; }

        /// <summary>
        /// Second data byte.
        /// </summary>
        public byte Data2 { get; set; }

        /// <summary>
        /// Third data byte.
        /// </summary>
        public byte Data3 { get; set; }

        #endregion Raw Data

        #region Contents

        /// <summary>
        /// Gets or sets the manufacturer ID.
        /// Built from <see cref="Data1"/> and the upper 4 bits of <see cref="Data2"/>.
        /// </summary>
        public int ManufacturerId
        {
            get { return (Data1 << 4) | (Data2 >> 4); }
            set { Data1 = (byte)(value >> 4); Data2 = (byte)((value << 4) | (Data2 & 0x0f)); }
        }

        /// <summary>
        /// Gets or sets the product ID, a combination of <see cref="ProductDensity"/> and <see cref="ProductData"/>.
        /// Built from the lower 4 bits of <see cref="Data2"/> and <see cref="Data3"/>;
        /// </summary>
        public int ProductId
        {
            get { return ((Data2 & 0x0f) << 8) | Data3; }
            set { Data2 = (byte)((value & 0x0f00) >> 8); Data3 = (byte)value; }
        }

        /// <summary>
        /// Product density, the lower 4 bits of <see cref="Data2"/>.
        /// </summary>
        public byte ProductDensity
        {
            get { return (byte)(Data2 & 0x0f); }
            set { Data2 = (byte)((value & 0x0f) | (Data2 & 0xf0)); }
        }

        /// <summary>
        /// Proprietary part of the <see cref="ProductId"/>.
        /// The same as <see cref="Data3"/>.
        /// </summary>
        public byte ProductData
        {
            get { return Data3; }
            set { Data3 = value; }
        }

        #endregion Contents

        #endregion Properties
    }
}