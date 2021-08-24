## Run in local

 1. Open the solution in Visual Studio 2019
 2. Add necessary settings in the FileShareProcessor -> appsettings.json
 3. Set the trigger timer in the FileShareProcessor -> local.settings.json -> TimerMinutesInterval
4. Run FileShareProcessor
5. Navigate to the HTTP trigger URI in the execution window

## Technology Stack
1. Azure Durable function 2.0
2. .Net Core 3.1
3. xUnit.Net
4. Moq