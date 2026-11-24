using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace Data.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Validates and adjusts a future DateTime with optional time adjustment.
    /// </summary>
    /// <param name="dateTime">The DateTime to validate and adjust</param>
    /// <param name="yearOffset">Number of years to add to current date if dateTime is invalid (default: 10)</param>
    /// <param name="setToEndOfDay">If true, sets time to end of day (23:59:59.9999999). If false, keeps original time or sets to start of day based on setToStartOfDay parameter</param>
    /// <param name="setToStartOfDay">If true and setToEndOfDay is false, sets time to start of day (00:00:00.000). If both are false, keeps original time</param>
    /// <returns>A validated DateTime, optionally adjusted to start or end of day</returns>
    public static DateTime ValidateFuture(this DateTime dateTime , int yearOffset = 10 , bool setToEndOfDay = true /*, bool setToStartOfDay = false*/)
    {
        DateTime result;

        if (dateTime <= SqlDateTime.MinValue.Value)
            result = DateTime.Now.Date.AddYears(yearOffset);
        else
            result = dateTime;

        if (setToEndOfDay)
            return result.Date.AddDays(1).AddTicks(-1);
        //else if (setToStartOfDay)
        //    return result.Date;
        else
            return result;
    }
    /// <summary>
    /// Validates and adjusts a future nullable DateTime with optional time adjustment.
    /// If the DateTime is null, invalid (less than or equal to SqlDateTime.MinValue), 
    /// returns the current date plus the specified number of years. Otherwise, returns the original DateTime.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime to validate and adjust</param>
    /// <param name="yearOffset">Number of years to add to current date if dateTime is null or invalid (default: 10)</param>
    /// <param name="setToEndOfDay">If true, sets time to end of day (23:59:59.9999999). If false, keeps original time</param>
    /// <returns>A validated DateTime, optionally adjusted to end of day</returns>
    public static DateTime ValidateFuture(this DateTime? dateTime , int yearOffset = 10 , bool setToEndOfDay = true)
    {
        DateTime result;

        if (dateTime == null || dateTime <= SqlDateTime.MinValue.Value)
            result = DateTime.Now.Date.AddYears(yearOffset);
        else
            result = dateTime.Value;

        if (setToEndOfDay)
            return result.Date.AddDays(1).AddTicks(-1);
        else
            return result;
    }
    /// <summary>
    /// Validates and adjusts a past DateTime. If the DateTime is invalid (less than or equal to SqlDateTime.MinValue), 
    /// returns the current date minus the specified number of years. Otherwise, returns the original DateTime.
    /// Optionally sets the time to the start of the day.
    /// </summary>
    /// <param name="dateTime">The DateTime to validate and adjust</param>
    /// <param name="yearOffset">Number of years to subtract from current date if dateTime is invalid (default: 20)</param>
    /// <param name="setToStartOfDay">If true, sets time to start of day (00:00:00.000). If false, keeps original time</param>
    /// <returns>A validated DateTime, optionally adjusted to start of day</returns>
    public static DateTime ValidatePast(this DateTime dateTime , int yearOffset = 20 , bool setToStartOfDay = true)
    {
        DateTime result;

        if (dateTime <= SqlDateTime.MinValue.Value)
            result = DateTime.Now.Date.AddYears(-yearOffset);
        else
            result = dateTime;

        if (setToStartOfDay)
            return result.Date;
        else
            return result;
    }
    /// <summary>
    /// Validates and adjusts a past nullable DateTime. If the DateTime is null or invalid (less than or equal to SqlDateTime.MinValue), 
    /// returns the current date minus the specified number of years. Otherwise, returns the original DateTime.
    /// Optionally sets the time to the start of the day.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime to validate and adjust</param>
    /// <param name="yearOffset">Number of years to subtract from current date if dateTime is null or invalid (default: 20)</param>
    /// <param name="setToStartOfDay">If true, sets time to start of day (00:00:00.000). If false, keeps original time</param>
    /// <returns>A validated DateTime, optionally adjusted to start of day</returns>
    public static DateTime ValidatePast(this DateTime? dateTime , int yearOffset = 20 , bool setToStartOfDay = true)
    {
        DateTime result;

        if (dateTime == null || dateTime <= SqlDateTime.MinValue.Value)
            result = DateTime.Now.Date.AddYears(-yearOffset);
        else
            result = dateTime.Value;

        if (setToStartOfDay)
            return result.Date;
        else
            return result;
    }

    /// <summary>
    /// Sets the DateTime to the beginning of the day (00:00:00.000).
    /// </summary>
    /// <param name="dateTime">The DateTime to modify</param>
    /// <returns>A DateTime set to the beginning of the day</returns>
    public static DateTime ToStartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }
    
    /// <summary>
    /// Sets the DateTime to the end of the day (23:59:59.9999999).
    /// </summary>
    /// <param name="dateTime">The DateTime to modify</param>
    /// <returns>A DateTime set to the end of the day</returns>
    public static DateTime ToEndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    public static DateOnly? ParseDateOnlyOrNull(this object? value)
        => DateOnly.TryParse(value?.ToString() , out var date) ? date : null;

    extension(DateOnly)
    {
        public static DateOnly Date
            => DateOnly.FromDateTime(DateTime.Today);
    }
}
