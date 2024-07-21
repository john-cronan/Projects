using JPC.Common;

namespace JPC.Common.UnitTests
{
    [TestClass]
    public class PathCanonicalizerTests
    {
        [TestMethod]
        public void Canonicalize_does_nothing()
        {
            var input = @"C:\Windows\System32\drivers\etc\hosts";
            var testee = new PathCanonicalizer();
            var actual = testee.MakeCanonical(input);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void Canonicalize_absolute_paths_on_windows()
        {
            var input = @"C:\Windows\.\System32\drivers\etc\..\..\..\..\Windows\System32\drivers\.\.\etc\hosts";
            var testee = new PathCanonicalizer();
            var actual = testee.MakeCanonical(input);
            var expected = @"C:\Windows\System32\drivers\etc\hosts";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Throws_on_attempt_to_navigate_above_root()
        {
            var input = @"C:\Windows\..\..";
            var testee = new PathCanonicalizer();
            try
            {
                testee.MakeCanonical(input);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void Canonicalizes_relative_path()
        {
            var input = @"Program Files\Acme Rockets\Road-Runner Killer\..\Cartoon Bomb";
            var testee = new PathCanonicalizer();
            var actual = testee.MakeCanonical(input);
            var expected = @"Program Files\Acme Rockets\Cartoon Bomb";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Canonicalizes_root_directory()
        {
            var input = @"C:\";
            var testee = new PathCanonicalizer();
            var actual = testee.MakeCanonical(input);
            Assert.AreEqual(input, actual);
        }
    }
}
