using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace KatvaSoft.SimpleAppInsightQuerier.AppInsightClient
{
    public class AzureAIMapper
    {

        public List<LogRow> ConvertResultToLogRows(string result)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(result);

            dynamic jsonObjValues = jsonObject.value;

            var logRows = new List<LogRow>();

            foreach(var jsonObjValue in jsonObjValues)
            {
                var logRow = new LogRow();
                logRow.Id = jsonObjValue.id;
                logRow.Type = jsonObjValue.type;
                logRow.TimeStamp = jsonObjValue.timestamp;
                logRow.SeverityLevel = jsonObjValue.trace.severityLevel;
                logRow.Message = jsonObjValue.trace.message;
                if(jsonObjValue.customDimensions != null)
                {
                    logRow.LoggerName = jsonObjValue.customDimensions.LoggerName;
                    logRow.LoggingLevel = jsonObjValue.customDimensions.LoggingLevel;
                    logRow.LogSourceType = jsonObjValue.customDimensions.SourceType;
                }
                if(jsonObjValue.operation != null)
                {
                    logRow.OperationName = jsonObjValue.operation.name;
                }
                logRows.Add(logRow);
            }

            return logRows;
        }
             
    }
}
