namespace Lagoon.UI.Helpers;

/// <summary>
/// Class containing date helpers.
/// </summary>
public class Date
{

    /// Date from which a two-character year entry changes to 1900 instead of 2000.
    private static int _rollingYear;

    /// <summary>
    /// Return a date from a string
    /// </summary>
    /// <param name="p_s_date">date</param>
    /// <param name="p_b_yearFirst">year before month</param>
    /// <returns></returns>
    internal static RawDate MagicStrToDate(string p_s_date, bool p_b_yearFirst)
    {
        List<string> l_o_part = new();
        List<bool> l_o_alpha = new();
        StringBuilder l_sb_part = new();
        bool l_b_alpha = false;
        string l_s_raw;
        RawDate l_o_rawDate = new();
        string l_s_year;

        // String empty => empty date
        if (string.IsNullOrEmpty(p_s_date))
        {
            l_o_rawDate.IsEmpty = true;
            l_o_rawDate.HasData = true;
            return l_o_rawDate;
        }
        foreach (char c in p_s_date)
        {
            if ("/-.".Contains(c))
            {
                if (l_sb_part.Length > 0)
                {
                    l_o_part.Add(l_sb_part.ToString());
                    l_o_alpha.Add(l_b_alpha);
                    l_sb_part.Length = 0;
                }
            }
            else if ("0123456789".Contains(c))
            {
                // New part (letters to numeric)
                if (l_sb_part.Length > 0 && l_b_alpha)
                {
                    l_o_part.Add(l_sb_part.ToString());
                    l_o_alpha.Add(l_b_alpha);
                    l_sb_part.Length = 0;
                }
                // Save the char into the part 
                l_b_alpha = false;
                l_sb_part.Append(c);
            }
            else
            {
                // Start new part  
                if (l_sb_part.Length > 0 && !l_b_alpha)
                {
                    l_o_part.Add(l_sb_part.ToString());
                    l_o_alpha.Add(l_b_alpha);
                    l_sb_part.Length = 0;
                }
                // Save the char into the part 
                l_b_alpha = true;
                l_sb_part.Append(c);
            }
        }

        // Save the last part
        if (l_sb_part.Length > 0)
        {
            l_o_part.Add(l_sb_part.ToString());
            l_o_alpha.Add(l_b_alpha);
            l_sb_part.Length = 0;
        }

        // Check if the is a separator
        if (l_o_part.Count == 1 && !l_o_alpha[0])
        {
            // Get the date without separator
            l_s_raw = l_o_part[0];
            l_o_part.Clear();
            // The dateis completly numeric without separator
            switch (l_s_raw.Length)
            {
                case 6:
                    l_o_part.Add(l_s_raw[..2]);
                    l_o_part.Add(l_s_raw.Substring(2, 2));
                    l_o_part.Add(l_s_raw.Substring(4, 2));
                    break;
                case 8:
                    if (p_b_yearFirst)
                    {
                        l_o_part.Add(l_s_raw[..4]);
                        l_o_part.Add(l_s_raw.Substring(4, 2));
                        l_o_part.Add(l_s_raw.Substring(6, 2));
                    }
                    else
                    {
                        l_o_part.Add(l_s_raw[..2]);
                        l_o_part.Add(l_s_raw.Substring(2, 2));
                        l_o_part.Add(l_s_raw.Substring(4, 4));
                    }

                    break;
                default:
                    // Impossible to find the date
                    return l_o_rawDate;
            }
            // Indicate all parts as number
            l_o_alpha.AddRange(new bool[] { false, false, false });
        }
        // Impossible to find the date
        if (l_o_part.Count != 3)
        {
            return l_o_rawDate;
        }
        // Check day and year frmat (numeric)
        if (l_o_alpha[0] || l_o_alpha[2])
        {
            return l_o_rawDate;
        }
        // Check is year  must be in last part of the  date
        if (l_o_part[2].Length > 2)
        {
            p_b_yearFirst = false;
        }
        else
        {
            // Check is year must be in first part of the date
            if (l_o_part[0].Length > 2)
            {
                p_b_yearFirst = true;
            }
        }
        // Get values
        if (p_b_yearFirst)
        {
            l_s_year = l_o_part[0];
            l_o_rawDate.Day = int.Parse(l_o_part[2]);
        }
        else
        {
            l_o_rawDate.Day = int.Parse(l_o_part[0]);
            l_s_year = l_o_part[2];
        }
        l_o_rawDate.Year = int.Parse(l_s_year);
        if (l_s_year.Length == 2)
        {
            if (l_o_rawDate.Year < RollingYear)
            {
                l_o_rawDate.Year = int.Parse("20" + l_s_year);
            }
            else
            {
                l_o_rawDate.Year = int.Parse("19" + l_s_year);
            }
        }
        if (l_o_alpha[1])
        {
            l_o_rawDate.Month = 0;
            if (!TryGetMonthValue(l_o_part[1], out int rawDateMonth))
            {
                return l_o_rawDate;
            }

            l_o_rawDate.Month = rawDateMonth;
        }
        else
        {
            l_o_rawDate.Month = int.Parse(l_o_part[1]);
        }
        l_o_rawDate.HasData = true;
        return l_o_rawDate;
    }

    /// <summary>
    ///Date from which a two-character year entry changes to 1900 instead of 2000.Date from which a two-character year entry changes to 1900 instead of 2000.
    /// </summary>
    public static int RollingYear
    {
        get
        {
            string l_s_dico;
            if (_rollingYear == 0)
            {
                l_s_dico = "UIDateRollingYear".Translate();
                if (!l_s_dico.All(char.IsDigit))
                {
                    l_s_dico = "51";
                }

                _rollingYear = int.Parse(l_s_dico);
            }
            return _rollingYear;
        }
    }

    /// <summary>
    /// Try to find the month index from the month label.
    /// </summary>
    /// <param name="monthLabel">Month label</param>
    /// <param name="monthIndex">Month index</param>
    /// <returns></returns>
    private static bool TryGetMonthValue(string monthLabel, out int monthIndex)
    {
        string[] l_as_month = GetShortMonthArray();
        for (int i = 0; i <= l_as_month.Length - 1; i++)
        {
            if (l_as_month[i].Equals(monthLabel, StringComparison.OrdinalIgnoreCase))
            {
                monthIndex = i + 1;
                return true;
            }
        }
        monthIndex = 0;
        return false;
    }

    /// <summary>
    /// Returns the table of short month names.
    /// </summary>
    protected static string[] GetShortMonthArray()
    {
        return ("MonthNamesShort").Translate().Split(',');
    }
}

#region RawDate class

///<summary>
/// Structure containing the different values of a date extracted from a text string.
///</summary>
public class RawDate
{

    /// <summary>
    /// Date is  emmpty ? 
    /// </summary>
    public bool IsEmpty { get; set; }

    /// <summary>
    /// Flag if  all date parts are finded .
    /// Warning : Do not indicate if the date is valid.
    /// </summary>
    public bool HasData { get; set; }

    /// <summary>
    /// Day
    /// </summary>
    public int Day { get; set; }

    /// <summary>
    /// Month
    /// </summary>
    public int Month { get; set; }

    /// <summary>
    /// Year
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Return the date with eah parts
    /// </summary>
    /// <returns></returns>
    public DateTime ToDate()
    {
        if (IsValid() && !IsEmpty)
        {
            return new DateTime(Year, Month, Day);
        }

        return default;
    }

    /// <summary>
    /// Check if the date is valid
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        try
        {
            CheckValidity();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// We check that a valid date can be created from the value in the text box.
    /// </summary>
    public void CheckValidity()
    {
        // Interpretation of the text string has failed. ?
        if (!HasData)
        {
            throw new Exception("InvalidDate".Translate());
        }
        // The text was empty ?
        if (IsEmpty)
        {
            return;
        }
        // Chech values
        if (Day < 1 || Day > 31)
        {
            throw new Exception("InvalidDay".Translate());
        }

        if (Month < 1 || Month > 12)
        {
            throw new Exception("InvalidMonth".Translate());
        }

        if ((Day == 31) && (Month == 4 || Month == 6 || Month == 9 || Month == 11))
        {
            throw new Exception("InvalidDay31".Translate());
        }

        if (Month == 2)
        {
            if ((Day > 29) || (Day == 29 && !DateTime.IsLeapYear(Year)))
            {
                throw new Exception("InvalidFebruary".Translate());
            }
        }
        if (Year < 1 || Year > 9999)
        {
            throw new Exception("InvalidYear".Translate());
        }
    }
}

#endregion
