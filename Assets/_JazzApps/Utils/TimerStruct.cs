using Fusion;
using UnityEngine;

namespace JazzApps.Utils
{
    public enum State
    {
        INACTIVE,
        SIGNALLING,
        ACTIVE,
    }
    /// <summary>
    /// A deterministic approach to a timer system.
    /// </summary>
    public struct TimerStruct : INetworkStruct
    {
        public State State { get; private set; }
        public float TimeLeft { get; private set; }
        public float TimeLeftPercent => TimeLeft / length;
        public float TimeLeftOverallPercent => (TimeLeft + length * intervalsLeft) / length * (intervals+1);
        public string TimeLeftFormatted => TimeLeft < 0 ? "0" : TimeLeft.ToString("G2");
        public string TimeLeftPercentFormatted => TimeLeftPercent < 0 ? "0" : TimeLeftPercent.ToString("G2");
        public string TimeLeftOverallPercentFormatted => TimeLeftOverallPercent < 0 ? "0" : TimeLeftOverallPercent.ToString("G2");
        private float intervalsLeft;
        private readonly float length;
        private readonly float intervals;
        
        public TimerStruct(float length, float repeatedTimes = 0)
        {
            this.State = State.ACTIVE;
            this.length = length;
            this.TimeLeft = length;
            this.intervals = repeatedTimes < 0 ? Mathf.Infinity : repeatedTimes;
            this.intervalsLeft = intervals;
        }
        public static TimerStruct Process(TimerStruct timer, float deltaTime, out State state)
        {
            timer.TimeLeft -= timer.TimeLeft > 0 ? deltaTime : 0;
            if (timer.TimeLeft <= 0)
            {
                if (timer.intervalsLeft > 0)
                {
                    timer.TimeLeft = timer.length;
                    timer.intervalsLeft--;
                    timer.State = State.SIGNALLING;
                }
                else
                    timer.State = State.INACTIVE;
            }
            else
                timer.State = State.ACTIVE;
            state = timer.State;
            return timer;
        }
    }
}