using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Common.Engine
{
    /// <summary>
    /// 限流器
    /// 改写自：hadoop\src\hdfs\org\apache\hadoop\hdfs\server\datanode\BlockTransferThrottler.java
    /// </summary>
    public class Throttler
    {
        private readonly int _period;          // period over which bw is imposed
        private readonly int _periodExtension; // Max period over which bw accumulates.
        private int _bytesPerPeriod; // total number of bytes can be sent in each period
        private int _curPeriodStart; // current period starting time
        private int _curReserve;     // remaining bytes can be sent in the period
        private int _bytesAlreadyUsed;

        /** Constructor 
         * @param bandwidthPerSec bandwidth allowed in bytes per second. 
         */
        public Throttler(int bandwidthPerSec): this(500, bandwidthPerSec)
        { // by default throttling period is 500ms 
        }

        /**
         * Constructor
         * @param period in milliseconds. Bandwidth is enforced over this
         *        period.
         * @param bandwidthPerSec bandwidth allowed in bytes per second. 
         */
        public Throttler(int period, int bandwidthPerSec)
        {
            this._curPeriodStart = CurrentTimeMillis();
            this._period = period;
            this._curReserve = this._bytesPerPeriod = bandwidthPerSec * period / 1000;
            this._periodExtension = period * 3;
        }

        /// <summary>
        /// 当前的时间戳
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static int CurrentTimeMillis()
        {
            return Environment.TickCount;
        }
        /**
         * @return current throttle bandwidth in bytes per second.
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long GetBandwidth()
        {
            return _bytesPerPeriod * 1000 / _period;
        }

        /**
         * Sets throttle bandwidth. This takes affect latest by the end of current
         * period.
         * 
         * @param bytesPerSecond 
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetBandwidth(int bytesPerSecond)
        {
            if (bytesPerSecond <= 0)
            {
                throw new Exception("" + bytesPerSecond);
            }
            _bytesPerPeriod = bytesPerSecond * _period / 1000;
        }

        /** Given the numOfBytes sent/received since last time throttle was called,
         * make the current thread sleep if I/O rate is too fast
         * compared to the given bandwidth.
         *
         * @param numOfBytes
         *     number of bytes sent/received since last time throttle was called
         */
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Throttle(int numOfBytes)
        {
            if (numOfBytes <= 0)
            {
                return;
            }

            _curReserve -= numOfBytes;
            _bytesAlreadyUsed += numOfBytes;

            while (_curReserve <= 0)
            {
                int now = CurrentTimeMillis();
                int curPeriodEnd = _curPeriodStart + _period;

                if (now < curPeriodEnd)
                {
                    // Wait for next period so that curReserve can be increased.
                    try
                    {
                        Thread.Sleep(curPeriodEnd - now);
                    }
                    catch (Exception)
                    { }
                }
                else if (now < (_curPeriodStart + _periodExtension))
                {
                    _curPeriodStart = curPeriodEnd;
                    _curReserve += _bytesPerPeriod;
                }
                else
                {
                    // discard the prev period. Throttler might not have
                    // been used for a long time.
                    _curPeriodStart = now;
                    _curReserve = _bytesPerPeriod - _bytesAlreadyUsed;
                }
            }

            _bytesAlreadyUsed -= numOfBytes;
        }
    }
}
