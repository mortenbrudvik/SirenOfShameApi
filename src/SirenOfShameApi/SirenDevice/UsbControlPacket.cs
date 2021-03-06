using System;
using System.Runtime.InteropServices;

namespace SirenOfShameApi.SirenDevice
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct UsbControlPacket
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte ReportId;

        [MarshalAs(UnmanagedType.U1)]
        public ControlByte1Flags ControlByte1;

        [MarshalAs(UnmanagedType.U1)]
        public byte AudioMode;

        [MarshalAs(UnmanagedType.U1)]
        public byte LedMode;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 AudioDuration;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 LedDuration;

        [MarshalAs(UnmanagedType.U1)]
        public byte ReadAudioIndex;

        [MarshalAs(UnmanagedType.U1)]
        public byte ReadLedIndex;

        [MarshalAs(UnmanagedType.U1)]
        public byte ManualLeds0;

        [MarshalAs(UnmanagedType.U1)]
        public byte ManualLeds1;

        [MarshalAs(UnmanagedType.U1)]
        public byte ManualLeds2;

        [MarshalAs(UnmanagedType.U1)]
        public byte ManualLeds3;

        [MarshalAs(UnmanagedType.U1)]
        public byte ManualLeds4;
    }
}