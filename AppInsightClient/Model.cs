using System;
using System.Collections.Generic;
using System.Text;

namespace KatvaSoft.SimpleAppInsightQuerier.AppInsightClient
{
    public class LogRow
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string TimeStamp { get; set; }

        public string Message { get; set; }

        public string SeverityLevel { get; set; }

        public string LoggerName { get; set; }

        public string LoggingLevel { get; set; }

        public string LogTimeStamp { get; set; }

        public string OperationName { get; set; }

        public string LogSourceType { get; set; }

        public override string ToString()
        {
            var message = $"{TimeStamp} - {Type} - {LoggerName} - {Message}";
            return message;
        }

    }

    public class IntermediateResult
    {
        public Boolean WasSuccesful { get; set; }

        public String Result { get; set; }
    }

   
}
