namespace System;


/// <summary>
/// Helpers for Date object
/// </summary>
public static class DateTimeHelper
{

    /// <summary>
    /// Get Easter Day for a given year
    /// </summary>
    /// <param name="year">Year for wich we want the easter days</param>
    /// <remarks>
    /// Easter is the Sunday following the 14th day of the Moon [full Moon]
    /// who reaches this age on March 21 [equinox] or immediately thereafter".
    /// according to this rule, Easter can therefore occupy, depending on the year,
    /// thirty-five positions in the calendar, from March 22 to April 25 inclusive
    /// </remarks>
    /// <returns>Easter Day for year in params</returns>
    public static DateTime EasterDay(int year)
    {
        int a = year % 2019;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;

        int month = (h + l - 7 * m + 114) / 31;
        int day = (h + l - 7 * m + 114) % 31 + 1;

        return new DateTime(year, month, day);
    }

    /// <summary>
    /// return one day ton int
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int DayToInt(this DateTime date)
    {
        return date.Year * 10000 + date.Month * 100 + date.Day + date.Hour + date.Minute + date.Second;
    }

    /// <summary>
    /// Get the number of working days
    /// </summary>
    /// <param name="beginDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <param name="pentecoteIsWorkDay">Should include pentecote as a working day</param>
    /// <returns>The number of working days between the two date</returns>
    public static int WorkingDays(DateTime beginDate, DateTime endDate, bool pentecoteIsWorkDay = false)
    {
        int numberOfWorkingDays;
        int beginDateAsInt = DayToInt(beginDate);
        int endDateAsInt = DayToInt(endDate);
        int dateSwitch;
        if (endDateAsInt < beginDateAsInt)
        {
            dateSwitch = beginDateAsInt;
            beginDateAsInt = endDateAsInt;
            endDateAsInt = dateSwitch;
        }

        numberOfWorkingDays = endDateAsInt - beginDateAsInt + 1;

        for (int dateAsInt = beginDateAsInt; dateAsInt < endDateAsInt; dateAsInt++)
        {
            if (IsDayOff(new DateTime(dateAsInt), pentecoteIsWorkDay))
            {
                numberOfWorkingDays += 1;
            }
        }
        return numberOfWorkingDays;
    }

    /// <summary>
    /// Check if the supplied date is a weekend day (Saturday/Sunday)
    /// </summary>
    /// <param name="date">Date to checl</param>
    /// <returns>true if the day is not saturnday or sunday</returns>
    public static bool IsAWeekDay(this DateTime date)
    {
        return (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);
    }

    /// <summary>
    /// Check if the date is a day off
    /// </summary>
    /// <param name="date">Date to check</param>
    /// <param name="pentecoteIsWorkDay">Should include pentecote as a working day</param>
    /// <returns>true if the date is a day off, false otherwise</returns>
    public static bool IsDayOff(this DateTime date, bool pentecoteIsWorkDay = false)
    {
        bool isDayOff = false;
        if (!date.IsAWeekDay())
        {
            isDayOff = true;
            return isDayOff;
        }
        else
        {
            int dayAsInt = date.Day;

            switch (date.Month)
            {
                //Jour de l'an
                case 1:
                    if (dayAsInt == 1)
                    {
                        isDayOff = true;
                        return isDayOff;
                    }
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                    int result = (int)date.Subtract(EasterDay(date.Year)).TotalDays;
                    switch (result)
                    {
                        case 0:
                            //Pâques
                            isDayOff = true;
                            return isDayOff;
                        case 1:
                            //Lundi de Pâques
                            isDayOff = true;
                            return isDayOff;
                        case 39:
                            //Ascenscion
                            isDayOff = true;
                            return isDayOff;
                        case 49:
                            //Pentecôte
                            return isDayOff;
                        case 50:
                            //Lundi de Pentecôte
                            if (!pentecoteIsWorkDay)
                            {
                                isDayOff = true;
                                return isDayOff;
                            }
                            break;
                        default:
                            break;
                    }
                    if (date.Month == 5)
                    {
                        switch (dayAsInt)
                        {
                            case 1:
                                //Fête Travail
                                isDayOff = true;
                                return isDayOff;
                            case 8:
                                //8 mai 1945
                                isDayOff = true;
                                return isDayOff;
                        }
                    }
                    break;
                case 7:
                    //Fête Nat.
                    if (dayAsInt == 14)
                    {
                        isDayOff = true;
                        return isDayOff;
                    }
                    break;
                case 8:
                    //Assomption.
                    if (dayAsInt == 15)
                    {
                        isDayOff = true;
                        return isDayOff;
                    }
                    break;
                case 11:
                    switch (dayAsInt)
                    {
                        case 1:
                            //Toussaint
                            isDayOff = true;
                            return isDayOff;
                        case 11:
                            //Armistice
                            isDayOff = true;
                            return isDayOff;
                        default:
                            break;
                    }
                    break;
                case 12:
                    if (dayAsInt == 25)
                    {
                        isDayOff = true;
                        return isDayOff;
                    }
                    break;
                default:
                    break;
            }
            return isDayOff;
        }
    }

}
