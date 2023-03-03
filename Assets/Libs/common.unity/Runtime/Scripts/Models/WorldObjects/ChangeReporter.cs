namespace ZCU.TechnologyLab.Common.Unity.Models.WorldObjects
{
    public class ChangeReporter
    {
        /// <summary>
        /// Remaining time to next report. 
        /// </summary>
        private float _remainingTime;
        
        public bool EnableReporting { get; set; }
        
        public float ReportDelay { get; set; }
        
        public ChangeReporter(float reportDelay, bool enableReporting)
        {
            ReportDelay = reportDelay;
            EnableReporting = enableReporting;
        }
        
        public bool ReportChangeInIntervals(bool isChanged, float deltaTime)
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= deltaTime;
            }

            if (isChanged
                && EnableReporting 
                && _remainingTime <= 0)
            {
                _remainingTime = ReportDelay;
                return true;
            }

            return false;
        }
    }
}