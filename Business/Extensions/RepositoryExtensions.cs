using Data;
using Data.Extensions;
using Data.VO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Business.Extensions;

public static class RepositoryExtensions
{
    public static async Task<string> GetLastCodePlusOne<T>(this NHibernateRepository repositoryBase , string whereCondition = "" , string internalWhereCondition = "" , string leftjoin = "") where T : EntityBaseWithCode
    {
        string code = await repositoryBase.GetLastCode<T>(whereCondition , internalWhereCondition , leftjoin);
        return StringLib.GetLastDigit(code);
    }
    /// <summary>
    /// Retrieves the next available code by incrementing the last used code, ensuring it is unique.
    /// </summary>
    public static async Task<bool> GetNextUniqueCode<T>(this NHibernateRepository repositoryBase , T t , string whereCondition = "" , string internalWhereCondition = "" , string leftjoin = "", string init = null) where T : EntityBaseWithCode
    {
        if (!string.IsNullOrEmpty(t.Code) && !await repositoryBase.IsCodeExist<T>(t , whereCondition , leftjoin))
            return false;

        t.Code = await repositoryBase.GetLastCodePlusOne<T>(whereCondition , internalWhereCondition , leftjoin);
        if (t.Code.Equals("1") && init is not null)
            t.Code = init;

        while (await repositoryBase.IsCodeExist<T>(t , whereCondition , leftjoin))
        {
            t.Code = StringLib.GetLastDigit(t.Code);
        }
        return true;
    }
    public static async Task<bool> GetNextUniqueNumber<T>(this NHibernateRepository repositoryBase , T t , string whereCondition = "" , string leftjoin = "") where T : EntityBaseWithCode
    {
        // Check if the provided number is valid and doesn't exist
        string andWhereClause = string.IsNullOrEmpty(whereCondition) ? "" : "AND " + whereCondition;
        if (t.Number > 0 && !await repositoryBase.IsNumberExist<T>(t , andWhereClause , leftjoin))
            return false;
        // Get the last number with proper WHERE clause formatting
        string whereClause = string.IsNullOrEmpty(whereCondition) ? "" : "WHERE " + whereCondition;
        t.Number = await repositoryBase.GetLastNumber<T>(whereClause , leftjoin);
        t.Number++;
        // Keep incrementing until we find a unique number
        while (await repositoryBase.IsNumberExist<T>(t , andWhereClause , leftjoin))
        {
            t.Number++;
        }
        return true;
    }
}
public class StringLib
{
    public static string GetLastDigit(string code)
    {
        //"A1B2c3d456"
        if (code == null || string.IsNullOrEmpty(code))
            return "1";
        try
        {
            // Find all numbers in the string
            MatchCollection matches = Regex.Matches(code , @"\d+");

            if (matches.Count == 0)
                return code + "1"; // No numbers found, append default

            // Get the last number found
            Match lastMatch = matches[matches.Count - 1];
            long number = Convert.ToInt64(lastMatch.Value);
            number++; // Increment the number

            // Replace the last number with the incremented value
            string result = code.Substring(0 , lastMatch.Index) +
                            number.ToString() +
                            code.Substring(lastMatch.Index + lastMatch.Length);

            return result;
        }
        catch (Exception ex)
        {
            return "1";
        }
        /*try
        {
            string[] numbers = Regex.Split(code, @"\D+");
            long number = 0;
            int index = 0;
            if (!string.IsNullOrEmpty(numbers[numbers.Length - 1]))
            {
                number = Convert.ToInt64(numbers[numbers.Length - 1]);
                index = code.Length - numbers[numbers.Length - 1].Length;
            }
            else
            {
                number = Convert.ToInt64(numbers[0]);
                index = code.Length - numbers[0].Length;
                ++number;
                return Convert.ToString(number);
            }

            string str = code.Substring(0, index);
            ++number;
            return str + number;
        }
        catch (Exception ex)
        {
            return "0001";
        }*/
    }
}
