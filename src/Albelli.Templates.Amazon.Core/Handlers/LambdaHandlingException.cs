using System;

namespace Albelli.Templates.Amazon.Core.Handlers
{
    public class LambdaHandlingException : Exception
    {
        public LambdaHandlingException(string message) : base(message)
        {

        }
    }
}