using System;
using System.Runtime.InteropServices;

namespace ColdClearNET
{
    internal static class Interface
    {
        public const string DllPath = "cold_clear.dll";


        [DllImport(DllPath,EntryPoint = "cc_launch_async")]
        public static extern IntPtr cc_launch_async(CCOptions options, CCWeights weights);

        [DllImport(DllPath,EntryPoint = "cc_destroy_async")]
        public static extern void cc_destroy_async(IntPtr bot);

        [DllImport(DllPath, EntryPoint = "cc_reset_async")]
        public static extern void cc_reset_async(IntPtr bot,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)]
            bool[] field,
            [MarshalAs(UnmanagedType.U1)] bool b2b,
            uint combo);

        [DllImport(DllPath, EntryPoint = "cc_add_next_piece_async")]
        public static extern void cc_add_next_piece_async(IntPtr bot, CCPiece piece);

        [DllImport(DllPath, EntryPoint = "cc_request_next_move")]
        public static extern void cc_request_next_move(IntPtr bot, uint incoming);

        [DllImport(DllPath, EntryPoint = "cc_poll_next_move")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool cc_poll_next_move(IntPtr bot, out CCMove move);

        [DllImport(DllPath, EntryPoint = "cc_is_dead_async")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool cc_is_dead_async(IntPtr bot);

        [DllImport(DllPath, EntryPoint = "cc_default_options")]
        public static extern void cc_default_options(out CCOptions options);

        [DllImport(DllPath, EntryPoint = "cc_default_weights")]
        public static extern void cc_default_weights(out CCWeights weights);

        [DllImport(DllPath, EntryPoint = "cc_fast_weights")]
        public static extern void cc_fast_weights(out CCWeights weights);
    }
}