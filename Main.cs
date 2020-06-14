using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static ColdClearNET.Interface;

namespace ColdClearNET
{
    /// <summary>
    /// Cold Clear Bot main class
    /// </summary>
    public class ColdClear : IDisposable
    {
        private readonly IntPtr ptr;

        /// <summary>
        /// 지정된 옵션과 무게를 사용하여 빈 보드, 빈 대기열 및 가방의 7개 피스가 모두 포함된 봇 실을 실행<br/>
        /// 봇 인스턴스가 완료되면 Disposition()을 호출하는 것을 잊지 마십시오.
        /// </summary>
        /// <param name="options">봇에 전달될 옵션.</param>
        /// <param name="weights">봇에게 전달될 평가 가중치.</param>
        public ColdClear(in CCOptions options, in CCWeights weights)
        {
            ptr = cc_launch_async(options, weights);
        }

        /* Launches a bot thread with a predefined field, empty queue, remaining pieces in the bag, hold piece,
         * back-to-back status, and combo count. This allows you to start CC from the middle of a game.
         * 
         * The bag_remain parameter is a bit field indicating which pieces are still in the bag. Each bit
         * correspond to CCPiece enum. This must match the next few pieces provided to CC via
         * cc_add_next_piece_async later.
         * 
         * The field parameter is a pointer to the start of an array of 400 booleans in row major order,
         * with index 0 being the bottom-left cell.
         * 
         * The hold parameter is a pointer to the current hold piece, or `NULL` if there's no hold piece now.
         */
        public ColdClear(in CCOptions options, in CCWeights weights, bool[] field, uint bag_remain, CCPiece? hold, bool b2b, uint combo)
        {
            ptr = cc_launch_with_board_async(options, weights, field, bag_remain, hold, b2b, combo);
        }

        /// <summary>
        /// 기본 옵션과 무게를 사용하여 빈 보드, 빈 대기열 및 가방의 7개 피스각 모두 포함된 봇 쓰레드를 실행<br/>
        /// 봇 인스턴스가 완료되면 <c>Dispose()</c>을 호출하는 것을 잊지 마십시오.
        /// </summary>
        public ColdClear() : this(GetDefaultOptions(), GetDefaultWeights()) { }

        /// <summary>
        /// 큐 끝에 새 피스를 추가<br/><br/>
        ///
        /// 추측이 가능하다면, 그 피스는 가방 안에 있어야 한다.<br/>
        /// 예를 들어 출발 시퀀스 IJOZT로 새 게임을 시작하면 이 기능을 처음 호출할 때 L이나 S 피스만 제공할 수 있다.
        /// </summary>
        /// <param name="piece">추가할 피스.</param>
        public void AddPiece(CCPiece piece)
        {
            cc_add_next_piece_async(ptr, piece);
        }

        /// <summary>
        /// 큐 끝에 새 피스들을 추가
        /// </summary>
        /// <seealso cref="AddPiece"/>
        /// <param name="pieces">추가할 피스의 열거형.</param>
        public void AddPieces(IEnumerable<CCPiece> pieces)
        {
            foreach (var piece in pieces)
            {
                cc_add_next_piece_async(ptr, piece);
            }
        }

        /// <summary>
        /// 재생 필드, 연속 상태 및 콤보 수를 재설정<br/><br/>
        ///
        /// 이는 가비지가 수신되거나 어떤 이유로(예: 15 이동 규칙) 플레이어가 해당 피스을 올바른 위치에 배치할 수 없을 때에만 사용해야 하며, 이는 봇이 이전 계산을 버리도록 강제하기 때문이다.<br/><br/>
        ///
        /// 참고: 콤보는 가이드라인 게임에서 표시된 콤보와 같지 않다. 여기서, 연속된 행 지우기 수입니다. 그래서 일반적으로 말해서 화면에 'x 콤보'가 뜨면 여기서 x+1을 사용해야 한다.
        /// </summary>
        /// <param name="field">400개 줄의 큰 순서대로 배열된 불리언(boolean), 지수 0은 하단 왼쪽 셀이다.</param>
        /// <param name="b2b">back-to-back 연속 상태</param>
        /// <param name="combo">콤보 값</param>
        public void Reset(bool[] field, bool b2b, uint combo)
        {
            cc_reset_async(ptr, field, b2b, combo);
        }

        /// <summary>
        /// 가능한 한 빨리 봇에게 이동을 제공하도록 요청<br/><br/>
        /// 
        /// 대부분의 경우, "가능한 한 빨리"는 매우 짧은 시간이며, 제공된 사고력의 하한선이 아직 도달하지 않았거나, 또는 봇이 다음 피스에 대한 정보가 부족하기 때문에 아직 움직임을 제공할 수 없는 경우에만 더 길다.<br/><br/>
        /// 
        /// 예를 들어, 제로피스 프리뷰와 홀드가 활성화된 게임에서, 봇은 자신이 어떤 작품을 들고 있는지 알 수 없기 때문에 결코 첫 번째 동작을 제공할 수 없을 것이다. 또 다른 예: 제로피스 프리뷰와 홀드 기능이 비활성화된 게임에서, 봇은 현재 피스가 생성된 후에만 이동할 수 있으며, 당신은 <c>AddPiece()</c> 또는 <c>AddPieces()</c>를 사용하여 해당 피스 정보를 봇에게 제공한다.<br/><br/>
        /// 
        /// 봇이 현재의 사고 주기를 끝내고 이동을 공급할 시간을 갖도록 이 기능을 피스가 스폰되기 전의 프레임이라고 부르는 것이 좋다.<br/><br/>
        /// 
        /// 일단 이동을 선택하면, 봇은 내부 상태를 작품이 올바르게 배치된 결과로 업데이트하고 <c>PollNextMove()</c>를 호출하여 이동을 이용할 수 있게 된다.
        /// </summary>
        /// <param name="incoming">다음 피스를 배치한 후 봇이 받을 것으로 예상되는 방해 블럭 라인 수</param>
        public void RequestNextMove(uint incoming)
        {
            cc_request_next_move(ptr, incoming);
        }

        /* Checks to see if the bot has provided the previously requested move yet.
         * 
         * The returned move contains both a path and the expected location of the placed piece. The
         * returned path is reasonably good, but you might want to use your own pathfinder to, for
         * example, exploit movement intricacies in the game you're playing.
         * 
         * If the piece couldn't be placed in the expected location, you must call `cc_reset_async` to
         * reset the game field, back-to-back status, and combo values.
         * 
         * If `plan` and `plan_length` are not `NULL` and this function provides a move, a placement plan
         * will be returned in the array pointed to by `plan`. `plan_length` should point to the length
         * of the array, and the number of plan placements provided will be returned through this pointer.
         * 
         * If the move has been provided, this function will return `CC_MOVE_PROVIDED`.
         * If the bot has not produced a result, this function will return `CC_WAITING`.
         * If the bot has found that it cannot survive, this function will return `CC_BOT_DEAD`
         */
        public CCBotPollStatus PollNextMove(out CCMove move, out CCPlanPlacement[] plan, ref uint plan_length)
        {
            return cc_poll_next_move(ptr, out move, out plan, ref plan_length);
        }

        /* This function is the same as `cc_poll_next_move` except when `cc_poll_next_move` would return
         * `CC_WAITING` it instead waits until the bot has made a decision.
         *
         * If the move has been provided, this function will return `CC_MOVE_PROVIDED`.
         * If the bot has found that it cannot survive, this function will return `CC_BOT_DEAD`
         */
        public CCBotPollStatus BlockNextMove(out CCMove move, out CCPlanPlacement[] plan, ref uint plan_length)
        {
            return cc_block_next_move(ptr, out move, out plan, ref plan_length);
        }


        private void ReleaseUnmanagedResources()
        {
            cc_destroy_async(ptr);
        }

        /// <summary>
        /// 봇 쓰레드을 종료하고 봇과 관련된 메모리를 확보
        /// </summary>
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ColdClear()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        /// 기본 옵션을 반환
        /// </summary>
        /// <returns>기본 옵션</returns>
        public static CCOptions GetDefaultOptions()
        {
            cc_default_options(out CCOptions options);
            return options;
        }

        /// <summary>
        /// 기본 가중치 반환
        /// </summary>
        /// <returns>기본 가중치</returns>
        public static CCWeights GetDefaultWeights()
        {
            cc_default_weights(out CCWeights weights);
            return weights;
        }

        /// <summary>
        /// 빠른 게임 구성 가중치 반환
        /// </summary>
        /// <returns>빠른 게임 구성 가중치</returns>
        public static CCWeights GetFastWeights()
        {
            cc_fast_weights(out CCWeights weights);
            return weights;
        }
    }

    public enum CCPiece : uint
    {
        CC_I, CC_O, CC_T, CC_L, CC_J, CC_S, CC_Z
    }

    public enum CCTspinStatus : uint
    {
        CC_NONE_TSPIN_STATUS,
        CC_MINI,
        CC_FULL
    }

    public enum CCMovement : uint
    {
        CC_LEFT,
        CC_RIGHT,
        CC_CW,
        CC_CCW,
        /// <summary>
        /// Only 소프트 드랍
        /// </summary>
        CC_DROP
    }

    public enum CCMovementMode : uint
    {
        CC_0G,
        CC_20G,
        CC_HARD_DROP_ONLY
    }

    public enum CCBotPollStatus : uint
    {
        CC_MOVE_PROVIDED,
        CC_WAITING,
        CC_BOT_DEAD
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CCPlanPlacement
    {
        public CCPiece piece;
        public CCTspinStatus tspin;

        /* Expected cell coordinates of placement, (0, 0) being the bottom left */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] expected_x;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] expected_y;

        /* Expected lines that will be cleared after placement, with -1 indicating no line */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] cleared_lines;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CCMove
    {
        /// <summary>
        /// hold가 필요한지 여부
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool hold;

        /// <summary>
        /// 셀 배치의 예상 좌표, (0, 0) 왼쪽 하단
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] expected_x;

        /// <summary>
        /// 셀 배치의 예상 좌표, (0, 0) 왼쪽 하단
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] expected_y;

        /// <summary>
        /// 경로의 이동 수
        /// </summary>
        public byte movement_count;

        /// <summary>
        /// Movements 동작들<br/>
        /// 길이는 항상 32이므로 실제 길이로 <c>movement_count</c>를 사용하십시오.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public CCMovement[] movements;

        public uint nodes;
        public uint depth;
        public uint original_rank;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CCOptions
    {
        public CCMovementMode mode;
        [MarshalAs(UnmanagedType.U1)] public bool use_hold;
        [MarshalAs(UnmanagedType.U1)] public bool speculate;
        [MarshalAs(UnmanagedType.U1)] public bool pcloop;
        public uint min_nodes;
        public uint max_nodes;
        public uint threads;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CCWeights
    {
        public int back_to_back;
        public int bumpiness;
        public int bumpiness_sq;
        public int height;
        public int top_half;
        public int top_quarter;
        public int jeopardy;
        public int cavity_cells;
        public int cavity_cells_sq;
        public int overhang_cells;
        public int overhang_cells_sq;
        public int covered_cells;
        public int covered_cells_sq;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] tslot;

        public int well_depth;
        public int max_well_depth;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public int[] well_column;

        public int b2b_clear;
        public int clear1;
        public int clear2;
        public int clear3;
        public int clear4;
        public int tspin1;
        public int tspin2;
        public int tspin3;
        public int mini_tspin1;
        public int mini_tspin2;
        public int perfect_clear;
        public int combo_garbage;
        public int move_time;
        public int wasted_t;
        [MarshalAs(UnmanagedType.U1)] public bool use_bag;
    }
}