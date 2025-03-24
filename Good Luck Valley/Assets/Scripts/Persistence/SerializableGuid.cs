using GoodLuckValley.Extensions.Guids;
using System;
using System.Text;
using UnityEngine;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField, HideInInspector] public uint Part1;
        [SerializeField, HideInInspector] public uint Part2;
        [SerializeField, HideInInspector] public uint Part3;
        [SerializeField, HideInInspector] public uint Part4;

        public static SerializableGuid Empty => new SerializableGuid(0, 0, 0, 0);

        public SerializableGuid(uint val1, uint val2, uint val3, uint val4)
        {
            Part1 = val1;
            Part2 = val2;
            Part3 = val3;
            Part4 = val4;
        }

        public SerializableGuid(Guid guid)
        {
            // Get the bytes of the guid
            byte[] bytes = guid.ToByteArray();

            // Separate the bytes into parts
            Part1 = BitConverter.ToUInt32(bytes, 0);
            Part2 = BitConverter.ToUInt32(bytes, 4);
            Part3 = BitConverter.ToUInt32(bytes, 8);
            Part4 = BitConverter.ToUInt32(bytes, 12);
        }

        /// <summary>
        /// Generates a new SerializableGuid from a new Guid
        /// </summary>
        /// <returns></returns>
        public static SerializableGuid NewGuid() => Guid.NewGuid().ToSerializableGuid();

        /// <summary>
        /// Creates a SerializableGuid from a hexadecimal string
        /// </summary>
        public static SerializableGuid FromHexString(string hexString)
        {
            // Validate the hex string
            if (hexString.Length != 32)
            {
                return Empty;
            }

            // Break the hexstring into serializable parts
            return new SerializableGuid(
                Convert.ToUInt32(hexString.Substring(0, 8), 16),
                Convert.ToUInt32(hexString.Substring(8, 8), 16),
                Convert.ToUInt32(hexString.Substring(16, 8), 16),
                Convert.ToUInt32(hexString.Substring(32, 8), 16)
            );
        }

        /// <summary>
        /// Converts the SerializableGuid to a hexadecimal string
        /// </summary>
        public string ToHexString()
        {
            // Build the Hex string
            StringBuilder sb = new StringBuilder();
            sb.Append(Part1.ToString("X8"));
            sb.Append(Part2.ToString("X8"));
            sb.Append(Part3.ToString("X8"));
            sb.Append(Part4.ToString("X8"));

            return sb.ToString();
        }

        /// <summary>
        /// Converts the SerializableGuid to a Guid
        /// </summary>
        public Guid ToGuid()
        {
            // Create an array to store the bytes
            byte[] bytes = new byte[16];

            // Convert and copy the bytes
            BitConverter.GetBytes(Part1).CopyTo(bytes, 0);
            BitConverter.GetBytes(Part2).CopyTo(bytes, 4);
            BitConverter.GetBytes(Part3).CopyTo(bytes, 8);
            BitConverter.GetBytes(Part4).CopyTo(bytes, 12);

            return new Guid(bytes);
        }

        public static implicit operator Guid(SerializableGuid serializableGuid) => serializableGuid.ToGuid();
        public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);
        public static bool operator ==(SerializableGuid left, SerializableGuid right) => left.Equals(right);
        public static bool operator !=(SerializableGuid left, SerializableGuid right) => !(left == right);

        public override bool Equals(object obj)
        {
            return obj is SerializableGuid guid && this.Equals(guid);
        }

        public bool Equals(SerializableGuid other)
        {
            return Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Part1, Part2, Part3, Part4);
        }
    }
}
