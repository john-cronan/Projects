namespace JPC.Common.UnitTests
{
    [TestClass]
    public class MemoryBufferTests
    {
        [TestMethod]
        public void MoveDataToZero_zeroes_out_remaining_bytes()
        {
            var bytes = Enumerable.Range(0, 10).Select(i => (byte)i).ToArray();
            var testee = new MemoryBuffer(bytes);
            testee.Seek(5);
            testee.MoveDataToZero();
            Assert.IsFalse(testee.Bytes.Skip(5).Any(b => b != 0));
            Assert.IsFalse(testee.Bytes.Take(5).Any(b => b == 0));
        }

        [TestMethod]
        public void ReadInt16_reads_correct_little_endian_value()
        {
            var bytes = new byte[] { 0x0, 0x04, 0x08, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var testee = new MemoryBuffer(bytes);
            testee.ByteOrder = ByteOrders.LittleEndian;
            testee.Seek(1);
            var i = testee.ReadInt16();
            Assert.AreEqual(0x804, i);
        }

        [TestMethod]
        public void ReadInt16_reads_correct_big_endian_value()
        {
            var bytes = new byte[] { 0x0, 0x10, 0x01, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var testee = new MemoryBuffer(bytes);
            testee.ByteOrder = ByteOrders.BigEndian;
            testee.Seek(1);
            var i = testee.ReadInt16();
            Assert.AreEqual(0x1001, i);
        }

        [TestMethod]
        public void ReadUInt16_reads_correct_little_endian_value()
        {
            var bytes = new byte[] { 0x0, 0xf0, 0x0f, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var testee = new MemoryBuffer(bytes);
            testee.ByteOrder = ByteOrders.LittleEndian;
            testee.Seek(1);
            var i = testee.ReadUInt16();
            Assert.AreEqual(0x0ff0, i);
        }

        [TestMethod]
        public void ReadUInt16_reads_correct_big_endian_value()
        {
            var bytes = new byte[] { 0x0, 0xf0, 0x0f, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var testee = new MemoryBuffer(bytes);
            testee.ByteOrder = ByteOrders.BigEndian;
            testee.Seek(1);
            var i = testee.ReadUInt16();
            Assert.AreEqual(0x0f00f, i);
        }

        [TestMethod]
        public void Write_from_stream()
        {
            var bytes = new byte[] { 0x0, 0x1, 0x2, 0x3 };
            using var memoryStream = new MemoryStream(bytes);
            var testee = new MemoryBuffer(8);
            var written = memoryStream.Read(testee.Bytes, 0, bytes.Length);
            testee.BytesWritten(written);
        }
    }

    //[TestClass]
    //public class MemoryBufferTests
    //{
    //    [TestMethod]
    //    public void ReadInt16_reads_correct_littleendian_value()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02 };
    //        var testee = new MemoryBuffer(8);
    //        testee.ByteOrder = Endianess.Little;
    //        testee.CopyBytesFrom(originalBytes, 0, 2);
    //        testee.Offset -= 2;
    //        var actualValue = testee.ReadInt16();

    //        Assert.IsTrue(BitConverter.IsLittleEndian
    //            ? BitConverter.ToInt16(originalBytes, 0) == actualValue
    //            : BitConverter.ToInt16(originalBytes, 0) != actualValue);
    //    }

    //    [TestMethod]
    //    public void ReadInt16_reads_correct_bigendian_value()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02 };
    //        var testee = new MemoryBuffer(8);
    //        testee.ByteOrder = Endianess.Big;
    //        testee.CopyBytesFrom(originalBytes, 0, 2);
    //        testee.Offset -= 2;
    //        var actualValue = testee.ReadInt16();

    //        Assert.IsTrue(BitConverter.IsLittleEndian
    //            ? BitConverter.ToInt16(originalBytes, 0) != actualValue
    //            : BitConverter.ToInt16(originalBytes, 0) == actualValue);
    //    }

    //    [TestMethod]
    //    public void ReadInt32_reads_correct_littleendian_value()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    //        var testee = new MemoryBuffer(8);
    //        testee.ByteOrder = Endianess.Little;
    //        testee.CopyBytesFrom(originalBytes, 0, 4);
    //        testee.Offset -= 2;
    //        var actualValue = testee.ReadInt32();

    //        Assert.IsTrue(BitConverter.IsLittleEndian
    //            ? BitConverter.ToInt16(originalBytes, 0) != actualValue
    //            : BitConverter.ToInt16(originalBytes, 0) == actualValue);
    //    }

    //    [TestMethod]
    //    public void ReadInt32_reads_correct_bigendian_value()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    //        var testee = new MemoryBuffer(8);
    //        testee.ByteOrder = Endianess.Big;
    //        testee.CopyBytesFrom(originalBytes, 0, 4);
    //        testee.Offset -= 4;
    //        var actualValue = testee.ReadInt32();

    //        Assert.IsTrue(BitConverter.IsLittleEndian
    //            ? BitConverter.ToInt32(originalBytes, 0) != actualValue
    //            : BitConverter.ToInt32(originalBytes, 0) == actualValue);
    //    }

    //    [TestMethod]
    //    public void ReadInt32_is_byte_order_dependent()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };
    //        var testeeBig = new MemoryBuffer(8);
    //        testeeBig.ByteOrder = Endianess.Big;
    //        var testeeLittle = new MemoryBuffer(8);
    //        testeeBig.ByteOrder = Endianess.Big;

    //        Assert.IsTrue(testeeBig.ReadInt32() == testeeLittle.ReadInt32());
    //    }

    //    [TestMethod]
    //    public void ReadUInt16_reads_correct_littleendian_value()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02 };
    //        var testee = new MemoryBuffer(8);
    //        testee.ByteOrder = Endianess.Little;
    //        testee.CopyBytesFrom(originalBytes, 0, 2);
    //        testee.Offset -= 2;
    //        var actualValue = testee.ReadUInt16();

    //        Assert.IsTrue(BitConverter.IsLittleEndian
    //            ? BitConverter.ToUInt16(originalBytes, 0) == actualValue
    //            : BitConverter.ToUInt16(originalBytes, 0) != actualValue);
    //    }

    //    [TestMethod]
    //    public void ReadUInt16_reads_correct_bigendian_value()
    //    {
    //        var originalBytes = new byte[] { 0x01, 0x02 };
    //        var testee = new MemoryBuffer(8);
    //        testee.ByteOrder = Endianess.Big;
    //        testee.CopyBytesFrom(originalBytes, 0, 2);
    //        testee.Offset -= 2;
    //        var actualValue = testee.ReadUInt16();

    //        Assert.IsTrue(BitConverter.IsLittleEndian
    //            ? BitConverter.ToUInt16(originalBytes, 0) != actualValue
    //            : BitConverter.ToUInt16(originalBytes, 0) == actualValue);
    //    }

    //}
}
