using System;
using System.Runtime.InteropServices;

namespace MouthFeatures {
    /// <summary>
    /// Wraps the mouth tracker API.
    /// </summary>
    internal class API
    {
        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int init(string localAddress, int localPort, string remoteAddress, int remotePort, string cameraScriptsPath);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int getFeatures(IntPtr buffer);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void getImages(IntPtr image, IntPtr normImage, IntPtr lipImage);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int getVideo(IntPtr frame);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void setColorFilter(int green, int blue);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void setMiddleLine(int height);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void startCalibration();

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void stopCalibration();

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void setSendFlag(bool flag);

        [DllImport("tracking", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int end();
    }

    public enum Errors{
        /// <summary>
        /// No error.
        /// </summary>
        OK                  = 0,
        /// <summary>
        /// Fail to instantiate decoder.
        /// </summary>
        NO_NETWORK          = -1,
        /// <summary>
        /// Codec is not correct.
        /// </summary>
        NO_CODEC            = -2,
        /// <summary>
        /// Codec context is not initialized.
        /// </summary>
        NO_CODEC_CONTEXT    = -3,
        /// <summary>
        /// Error in binding sockets.
        /// </summary>
        NO_SOCKET_BIND      = -4,
        /// <summary>
        /// Receiving thread has been closed.
        /// </summary>
        NO_RECEIVER         = -5,
        /// <summary>
        /// Sender thread has been closed.
        /// </summary>
        BAD_FRAME           = -6,
        /// <summary>
        /// Raspberry has not been initialized.
        /// </summary>
        NO_RASP             = -7,
        /// <summary>
        /// The frame has not been taken.false You may have not initialized API.
        /// </summary>
        NO_FRAME             = -8,
        /// <summary>
        /// An unknown error.
        /// </summary>
        UNKNOWN             = -9
    }
}
