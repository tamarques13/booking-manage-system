namespace BookingSystem.UnitTests.Helpers
{
    public class Utils
    {
        public static DateTime FutureDate(int hour)
        {
            return new DateTime(DateTime.Now.Year + 1, 1, 1, hour, 0, 0);
        }

        public static DateTime FutureSaturday()
        {
            DateTime today = DateTime.Now;
            int diff = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
            DateTime saturday = today.AddDays(diff == 0 ? 7 : diff);

            return saturday;
        }
    }
}