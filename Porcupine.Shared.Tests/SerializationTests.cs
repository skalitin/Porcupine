using System;
using Xunit;

namespace Porcupine.Shared.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void SerializingCopyFileRequest()
        {
            var request = new CopyFileRequest() { Source = "a", Target = "bcя" };
            var bytes = Google.Protobuf.MessageExtensions.ToByteArray(request);

            var expected = new byte[] { 10, 1, 97, 18, 4, 98, 99, 209, 143 };
            Assert.Equal(expected, bytes);

            var deserialized = CopyFileRequest.Parser.ParseFrom(bytes);
            Assert.Equal("a", deserialized.Source);
            Assert.Equal("bcя", deserialized.Target);
        }

        [Fact]
        public void SerializingCopyFileResponse()
        {
            var response = new CopyFileResponse() { Path = "a" };
            var bytes = Google.Protobuf.MessageExtensions.ToByteArray(response);

            var expected = new byte[] { 10, 1, 97, 18 };
            Assert.Equal(expected, bytes);

            var deserialized = CopyFileResponse.Parser.ParseFrom(bytes);
            Assert.Equal("a", deserialized.Path);
        }
    }
}
