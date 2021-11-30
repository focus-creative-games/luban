using Bright.Time;
using System.Collections.Generic;

namespace Luban.Common.Utils
{
    public class Phase
    {
        public string Name { get; set; }

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public long ElapseTime { get; set; }
    }

    public class ProfileTimer
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Stack<Phase> phaseStack = new Stack<Phase>();

        public void StartPhase(string name)
        {
            phaseStack.Push(new Phase() { Name = name, StartTime = Bright.Time.TimeUtil.NowMillis });
        }

        private Phase EndPhase()
        {
            var phase = phaseStack.Pop();
            phase.EndTime = Bright.Time.TimeUtil.NowMillis;
            phase.ElapseTime = phase.EndTime - phase.StartTime;
            return phase;
        }

        public Phase EndPhaseAndLog()
        {
            var phase = EndPhase();
            s_logger.Info("====== {name} cost {time} ms ======", phase.Name, phase.ElapseTime);
            return phase;
        }
    }
}
