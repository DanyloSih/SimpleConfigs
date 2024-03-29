﻿namespace SimpleConfigs.Core
{
    /// <summary>
    /// Represents an algorithm for converting C# object data into a byte array and vice versa.
    /// </summary>
    public interface ISerializationManager
    {
        /// <summary>
        /// Convert C# object data into a byte array.
        /// </summary>
        public Task<byte[]> SerializeAsync(object serializableObject);

        /// <summary>
        /// Fill C# object with data in byte array format.
        /// </summary>
        /// <param name="populatingObject">An object whose fields will be filled with <paramref name="serializationData"/>.</param>
        /// <param name="serializationData">Data obtained from method <see cref="SerializeAsync"/>.</param>
        public Task DeserializeAsync(object populatingObject, byte[] serializationData);
    }
}