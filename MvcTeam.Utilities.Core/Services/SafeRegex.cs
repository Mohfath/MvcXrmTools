using Microsoft.Xrm.Sdk;
using System;
using System.Text.RegularExpressions;

namespace MvcTeam.Utilities.Services
{
    //Regular expressions typed into a workflow: a bad pattern gives a clear error instead of a raw exception,
    //and a pattern that backtracks forever is stopped after two seconds instead of hanging the workflow.
    public static class SafeRegex
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

        public static Regex Create(string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        {
            try
            {
                return new Regex(pattern, options, Timeout);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidPluginExecutionException($"Pattern '{pattern}' is not a valid regular expression: {ex.Message}", ex);
            }
        }

        public static T Run<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (RegexMatchTimeoutException ex)
            {
                throw new InvalidPluginExecutionException("The regular expression took longer than 2 seconds; simplify the pattern.", ex);
            }
        }
    }
}
