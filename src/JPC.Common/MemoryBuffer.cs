using System;
using System.Buffers.Binary;
using System.Drawing;

namespace JPC.Common
{
    public class MemoryBuffer
    {
        private readonly byte[] _bytes;

        //
        //  The buffer is divided into 3 regions: Unused Space + Occupied Space +
        //  Free Space, which we can write into. The sum of all 3 regions equals
        //  the total byte[] at all times.
        //

        //
        //  Free Space always starts at offset 0. If Occupied Space also starts at 0, then
        //  the length of Free Space is 0. The length of Free Space = Starting Offset of
        //  Occupied Space.

        //  The offset at which data starts. Everything below this offset is Unused Space.
        //  The region then extends to the starting offset of Free Space - 1.
        private int _occupiedSpaceStartAt;

        //  The index at which free space starts. Directly below this index is occupied space
        //  (if any); below that is unused space.
        private int _freeSpaceStartsAt;


        public MemoryBuffer(int size)
        {
            _bytes = new byte[size];
            _freeSpaceStartsAt = _bytes.Length;
        }

        public MemoryBuffer(byte[] bytes)
        {
            _bytes = new byte[bytes.Length];
            Array.Copy(bytes, 0, _bytes, 0, bytes.Length);
            _freeSpaceStartsAt = _bytes.Length;
        }

        public byte[] Bytes => _bytes;
        public ByteOrders ByteOrder { get; set; }
        public int FreeSpaceLength => _bytes.Length - _freeSpaceStartsAt;
        public int FreeSpaceStartsAt => _freeSpaceStartsAt;
        public int OccupiedSpaceLength => _freeSpaceStartsAt - OccupiedSpaceStartAt;
        public int OccupiedSpaceStartAt => _occupiedSpaceStartAt;
        public int TotalAvailableSpace => FreeSpaceLength + OccupiedSpaceStartAt;
        public int UnusedLength => OccupiedSpaceStartAt;


        public void BytesWritten(int howMany)
        {
            if (howMany > FreeSpaceLength)
            {
                throw new ArgumentOutOfRangeException(nameof(howMany));
            }
            _freeSpaceStartsAt += howMany;
            AssertInvariants();
        }

        public void MoveDataToZero()
        {
            var bytesAvailable = OccupiedSpaceLength;
            Array.Copy(_bytes, OccupiedSpaceStartAt, _bytes, 0, OccupiedSpaceLength);
            _occupiedSpaceStartAt = 0;
            _freeSpaceStartsAt = bytesAvailable;
            Array.Clear(_bytes, _freeSpaceStartsAt, FreeSpaceLength);
            AssertInvariants();
        }

        public short ReadInt16()
        {
            try
            {
                var i = BitConverter.ToInt16(_bytes, OccupiedSpaceStartAt);
                _occupiedSpaceStartAt += sizeof(short);
                return SwapByteOrder ? BinaryPrimitives.ReverseEndianness(i) : i;
            }
            finally
            {
                AssertInvariants();
            }
        }

        public ushort ReadUInt16()
        {
            try
            {
                var u = BitConverter.ToUInt16(_bytes, OccupiedSpaceStartAt);
                _occupiedSpaceStartAt += sizeof(ushort);
                return SwapByteOrder ? BinaryPrimitives.ReverseEndianness(u) : u;
            }
            finally
            {
                AssertInvariants();
            }
        }

        public void Seek(int offset)
        {
            _occupiedSpaceStartAt += offset;
            AssertInvariants();
        }

        public byte this[int index]
        {
            get { return _bytes[index]; }
            set { _bytes[index] = value; }
        }


        private void AssertInvariants()
        {
            if (OccupiedSpaceStartAt < 0)
                throw new InvalidOperationException("Condition violated: Occupied space must start at >= 0");
            if (OccupiedSpaceStartAt >= _bytes.Length)
                throw new InvalidOperationException("Condition violated: Occupied space must start below the ending index of the buffer");
            if (_freeSpaceStartsAt < 0)
                throw new InvalidOperationException("Condition violated: Free space must start at >= 0");
            if (OccupiedSpaceStartAt >= _freeSpaceStartsAt)
                throw new InvalidOperationException("Condition violated: Free space must start after Occupied space");
            if (TotalAvailableSpace > OccupiedSpaceLength)
                throw new InvalidOperationException("Condition violated: Total space available must be <= total space used");
        }

        private bool SwapByteOrder
            => ByteOrder switch
            {
                ByteOrders.System => false,
                ByteOrders.LittleEndian => !BitConverter.IsLittleEndian,
                ByteOrders.BigEndian => BitConverter.IsLittleEndian,
                _ => throw new ArgumentException("Invalid ByteOrders value")
            };
    }
}


//public class MemoryBuffer
//{
//    private readonly byte[] _bytes;

//    public MemoryBuffer(int size)
//    {
//        _bytes = new byte[size];
//        ByteOrder = Endianess.System;
//    }

//    public byte this[int index]
//    {
//        get { return _bytes[index]; }
//        set { _bytes[index] = value; }
//    }

//    public byte[] Bytes => _bytes;
//    public int BytesRemaining => _bytes.Length - Offset - 1;
//    public byte CurrentByte => _bytes[Offset];
//    public Endianess ByteOrder { get; set; }
//    public int Offset { get; set; }
//    public int Length => _bytes.Length;

//    public void CopyBytesFrom(byte[] array, int offset, int length)
//    {
//        Array.Copy(array, offset, Bytes, Offset, length);
//        Offset += length;
//    }

//    public byte[] ReadBytes(int length)
//    {
//        var bytesToReturn = new byte[length];
//        Array.Copy(_bytes, Offset, bytesToReturn, 0, length);
//        Offset += length;
//        return bytesToReturn;
//    }

//    public short ReadInt16()
//    {
//        return SwapByteOrder
//            ? BinaryPrimitives.ReverseEndianness(BitConverter.ToInt16(_bytes, Offset))
//            : BitConverter.ToInt16(_bytes, Offset);
//    }

//    public ReadOnlySpan<byte> CreateReadOnlySpan(int size)
//        => CreateReadOnlySpan(Offset, size);

//    public ReadOnlySpan<byte> CreateReadOnlySpan(int index, int size)
//        => new ReadOnlySpan<byte>(_bytes, index, size);

//    public int ReadInt32()
//    {
//        return SwapByteOrder
//            ? BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(_bytes, Offset))
//            : BitConverter.ToInt32(_bytes, Offset);
//    }

//    public ushort ReadUInt16()
//    {
//        return SwapByteOrder
//            ? BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt16(_bytes, Offset))
//            : BitConverter.ToUInt16(_bytes, Offset);
//    }

//    public uint ReadUInt32()
//    {
//        return SwapByteOrder
//            ? BinaryPrimitives.ReverseEndianness(BitConverter.ToUInt32(_bytes, Offset))
//            : BitConverter.ToUInt32(_bytes, Offset);
//    }

//    public void Zero()
//        => Array.Clear(_bytes, 0, _bytes.Length);

//    private bool SwapByteOrder
//    {
//        get => ByteOrder switch
//        {
//            Endianess.System => false,
//            Endianess.Little => !BitConverter.IsLittleEndian,
//            Endianess.Big => BitConverter.IsLittleEndian,
//            _ => throw new ArgumentException("Invalid endianess value")
//        };
//    }
//}
//}
