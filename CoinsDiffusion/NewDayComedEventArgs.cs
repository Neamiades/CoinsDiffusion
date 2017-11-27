using System;

namespace CoinsDiffusion
{
    public class NewDayComedEventArgs : EventArgs
    {
        public readonly uint CurrentDay;

        public NewDayComedEventArgs(uint currentDay)
        {
            CurrentDay = currentDay;
        }
    }
}
