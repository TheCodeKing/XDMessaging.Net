using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Conditions;
using XDMessaging.Messages;
using XDMessaging.Serialization;

namespace XDMessaging.Transport.WindowsMessaging
{
    internal class WinMsgDataGram : IDisposable
    {
        private readonly DataGram dataGram;

        private readonly ISerializer serializer;

        private bool allocatedMemory;

        private Native.COPYDATASTRUCT dataStruct;

        internal WinMsgDataGram(ISerializer serializer, string channel, string dataType, string message)
        {
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;
            allocatedMemory = false;
            dataStruct = new Native.COPYDATASTRUCT();
            dataGram = new DataGram(channel, dataType, message);
        }

        private WinMsgDataGram(IntPtr lpParam, ISerializer serializer)
        {
            serializer.Requires("serializer").IsNotNull();

            this.serializer = serializer;
            allocatedMemory = false;
            dataStruct = (Native.COPYDATASTRUCT) Marshal.PtrToStructure(lpParam, typeof (Native.COPYDATASTRUCT));
            var bytes = new byte[dataStruct.cbData];
            Marshal.Copy(dataStruct.lpData, bytes, 0, dataStruct.cbData);
            string rawmessage;
            using (var stream = new MemoryStream(bytes))
            {
                var b = new BinaryFormatter();
                rawmessage = (string) b.Deserialize(stream);
            }

            dataGram = serializer.Deserialize<DataGram>(rawmessage);
        }

        public string Channel => dataGram.Channel;

        public string Message => dataGram.Message;

        internal bool IsValid => dataGram.IsValid;

        public void Dispose()
        {
            if (dataStruct.lpData == IntPtr.Zero)
            {
                return;
            }

            if (allocatedMemory)
            {
                Marshal.FreeCoTaskMem(dataStruct.lpData);
            }

            dataStruct.lpData = IntPtr.Zero;
            dataStruct.dwData = IntPtr.Zero;
            dataStruct.cbData = 0;
        }

        public static implicit operator DataGram(WinMsgDataGram dataGram)
        {
            return dataGram.dataGram;
        }

        internal static WinMsgDataGram FromPointer(IntPtr lpParam, ISerializer serializer)
        {
            return new WinMsgDataGram(lpParam, serializer);
        }

        public override string ToString()
        {
            return dataGram.ToString();
        }

        internal Native.COPYDATASTRUCT ToStruct()
        {
            var raw = serializer.Serialize(dataGram);

            byte[] bytes;

            var b = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                b.Serialize(stream, raw);
                stream.Flush();
                var dataSize = (int) stream.Length;

                bytes = new byte[dataSize];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(bytes, 0, dataSize);
            }
            var ptrData = Marshal.AllocCoTaskMem(bytes.Length);

            allocatedMemory = true;
            Marshal.Copy(bytes, 0, ptrData, bytes.Length);

            dataStruct.cbData = bytes.Length;
            dataStruct.dwData = IntPtr.Zero;
            dataStruct.lpData = ptrData;

            return dataStruct;
        }
    }
}