using Controllers;

namespace Messaging
{
    public class GamePlayMessages
    {
        public class PlayerFailureEvent
        { }

        public class DataBitCollectedEvent
        { }

        public class ReportBitCountEvent
        {
            public readonly int BitCount;

            public ReportBitCountEvent(int bitCount)
            {
                BitCount = bitCount;
            }
        }

        /// <summary>
        /// Event to request <see cref="PlayerStateController"/> to report how many bits it's holding rn.
        /// </summary>
        public class RequestHoldingBitCountEvent
        { }

        public class ReportHoldingBitCountEvent
        {
            public readonly int BitCount;

            public ReportHoldingBitCountEvent(int bitCount)
            {
                BitCount = bitCount;
            }
        }

        public class DirectoryReceivedEvent
        { }

        public class DirectoryCompletedEvent
        { }

        public class RetryRequestedEvent
        { }
    }
}