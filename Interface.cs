using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ColdClearNET
{
    internal static class Interface
    {
        internal const string DllPath = "cold_clear.dll";


        [DllImport(DllPath,EntryPoint = "cc_launch_async")]
        internal static extern IntPtr cc_launch_async(CCOptions options, CCWeights weights);

        [DllImport(DllPath,EntryPoint = "cc_launch_with_board_async")]
        internal static extern IntPtr cc_launch_with_board_async(
            CCOptions options, 
            CCWeights weights,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)]
            bool[] field,
            uint bag_remain,
            CCPiece? hold,
            [MarshalAs(UnmanagedType.U1)] bool b2b,
            uint combo);

        [DllImport(DllPath,EntryPoint = "cc_destroy_async")]
        internal static extern void cc_destroy_async(IntPtr bot);

        [DllImport(DllPath, EntryPoint = "cc_reset_async")]
        internal static extern void cc_reset_async(
            IntPtr bot,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)]
            bool[] field,
            [MarshalAs(UnmanagedType.U1)] bool b2b,
            uint combo);

        [DllImport(DllPath, EntryPoint = "cc_add_next_piece_async")]
        internal static extern void cc_add_next_piece_async(IntPtr bot, CCPiece piece);

        [DllImport(DllPath, EntryPoint = "cc_request_next_move")]
        internal static extern void cc_request_next_move(IntPtr bot, uint incoming);

        [DllImport(DllPath, EntryPoint = "cc_poll_next_move")]
        internal static extern CCBotPollStatus cc_poll_next_move(
            IntPtr bot,
            out CCMove move,
            [MarshalAs(UnmanagedType.LPArray,ArraySubType = UnmanagedType.Struct)] out CCPlanPlacement[] plan,
            ref uint plan_length);

        [DllImport(DllPath, EntryPoint = "cc_block_next_move")]
        internal static extern CCBotPollStatus cc_block_next_move(
            IntPtr bot,
            out CCMove move,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct)] out CCPlanPlacement[] plan,
            ref uint plan_length);

        [DllImport(DllPath, EntryPoint = "cc_default_options")]
        internal static extern void cc_default_options(out CCOptions options);

        [DllImport(DllPath, EntryPoint = "cc_default_weights")]
        internal static extern void cc_default_weights(out CCWeights weights);

        [DllImport(DllPath, EntryPoint = "cc_fast_weights")]
        internal static extern void cc_fast_weights(out CCWeights weights);
    }
}