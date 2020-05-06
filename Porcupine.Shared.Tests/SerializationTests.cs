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

            var expected = new byte[] { 10, 1, 97 };
            Assert.Equal(expected, bytes);

            var deserialized = CopyFileResponse.Parser.ParseFrom(bytes);
            Assert.Equal("a", deserialized.Path);
        }

        [Fact]
        public void SerializingCopyFileRequest_v1_to_v2()
        {
            var request = new Porcupine.CopyFileRequest() { Source = "source", Target = "target", CopyPermissions = true };
            var bytes = Google.Protobuf.MessageExtensions.ToByteArray(request);

            var deserialized_v2 = PorcupineTest.CopyFileRequest.Parser.ParseFrom(bytes);
            Assert.Equal("source", deserialized_v2.Source);
            Assert.Equal("target", deserialized_v2.Target);
            Assert.False(deserialized_v2.OverwriteExistingFile);
            
            var deserialized_v1 = Porcupine.CopyFileRequest.Parser.ParseFrom(bytes);
            Assert.Equal("source", deserialized_v1.Source);
            Assert.Equal("target", deserialized_v1.Target);
            Assert.True(deserialized_v1.CopyPermissions);
        }

        [Fact]
        public void SerializingCopyFileRequest_v2_to_v1()
        {
            var request = new PorcupineTest.CopyFileRequest() { Source = "source", Target = "target", OverwriteExistingFile = true };
            var bytes = Google.Protobuf.MessageExtensions.ToByteArray(request);

            var deserialized_v2 = PorcupineTest.CopyFileRequest.Parser.ParseFrom(bytes);
            Assert.Equal("source", deserialized_v2.Source);
            Assert.Equal("target", deserialized_v2.Target);
            Assert.True(deserialized_v2.OverwriteExistingFile);
            
            var deserialized_v1 = Porcupine.CopyFileRequest.Parser.ParseFrom(bytes);
            Assert.Equal("source", deserialized_v1.Source);
            Assert.Equal("target", deserialized_v1.Target);
            Assert.False(deserialized_v1.CopyPermissions);
        }
    }
}
