using System;
using System.Globalization;

public static class TafqeetHelper
{
    private static readonly string[] Units =
    {
        "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة", "ستة", "سبعة", "ثمانية", "تسعة",
        "عشرة", "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر",
        "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
    };

    private static readonly string[] Tens =
    {
        "", "", "عشرون", "ثلاثون", "أربعون", "خمسون",
        "ستون", "سبعون", "ثمانون", "تسعون"
    };

    private static readonly string[] Hundreds =
    {
        "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة",
        "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
    };

    private static readonly string[] ThousandsGroups =
    {
        "", "ألف", "مليون", "مليار", "تريليون"
    };

    public static string Tafqeet(decimal number)
    {
        if (number == 0) return "صفر جنيه";

        long integerPart = (long)Math.Floor(number);
        int fractionPart = (int)Math.Round((number - integerPart) * 100); // القروش

        string result = $"{ConvertToArabicWords(integerPart)} جنيه";
        if (fractionPart > 0)
            result += $" و{ConvertToArabicWords(fractionPart)} قرشاً";

        return result;
    }

    private static string ConvertToArabicWords(long number)
    {
        if (number == 0) return "";

        string words = "";
        int group = 0;

        while (number > 0)
        {
            int threeDigits = (int)(number % 1000);
            if (threeDigits != 0)
            {
                string groupText = ConvertThreeDigits(threeDigits);
                if (!string.IsNullOrEmpty(groupText))
                {
                    if (group > 0)
                        words = $"{groupText} {ThousandsGroups[group]}{(groupText.EndsWith("ة") ? "" : "")} {(string.IsNullOrEmpty(words) ? "" : "و" + words)}";
                    else
                        words = $"{groupText} {(string.IsNullOrEmpty(words) ? "" : "و" + words)}";
                }
            }
            number /= 1000;
            group++;
        }

        return words.Trim();
    }

    private static string ConvertThreeDigits(int number)
    {
        int hundreds = number / 100;
        int tens = number % 100;
        string words = "";

        if (hundreds > 0)
        {
            words += Hundreds[hundreds];
            if (tens > 0) words += " و";
        }

        if (tens > 0)
        {
            if (tens < 20)
                words += Units[tens];
            else
            {
                int unit = tens % 10;
                int ten = tens / 10;
                if (unit > 0)
                    words += Units[unit] + " و" + Tens[ten];
                else
                    words += Tens[ten];
            }
        }

        return words.Trim();
    }
}
