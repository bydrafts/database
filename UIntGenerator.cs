using System;
using System.Security.Cryptography;

namespace Drafts.Database
{
    public class UIntGenerator : IDisposable
    {
        public static readonly UIntGenerator Instance = new();
        private readonly byte[] _bytes = new byte[4];
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public void Dispose() => _rng?.Dispose();

        public uint Generate()
        {
            _rng.GetBytes(_bytes);
            return BitConverter.ToUInt32(_bytes, 0);
        }
    }
}