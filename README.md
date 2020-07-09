# Cosmos DB trigger in .NET Core to get new documents saved and store them in CosmosDB using SQL API

Cosmos DB trigger in .NET Core to get new documents saved and store them in CosmosDB using SQL API

This is a Cosmos DB trigger function written in C# - .NET Core 3.1. It waits until a document creates in the database, process it and send notifications to a mobile phone

## Technology stack  
* .NET Core 3.1 on Visual Studio 2019
* Azure functions v3 and Azure Cosmos DB SQL API
* The sample code has used Notify.lk sms API to send messages, https://www.notify.lk/

## How to run the solution
 * You have to create a Cosmos DB account with SQL API then go to the Keys section, get the connectionstring to connect to the database
 * Open the solution file in Visual Studio and build the project
 
## Code snippets
### Run method in Cosmos DB trigger
This trigger fires when a document is created or modified in the Cosmos DB,
```
[FunctionName("SendNotification")]
public void Run([CosmosDBTrigger(databaseName: "TrafficData", collectionName: "Vehicles", 
  ConnectionStringSetting = "DBConnectionstring", LeaseCollectionName = "leases", 
  CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input, ILogger log)
{
  if (input != null && input.Count > 0)
  {
    log.LogInformation("Documents modified " + input.Count);
    log.LogInformation("First document Id " + input[0].Id);

    string vehicleNo = input[0].GetPropertyValue<string>("vehicleNumber");
    double speed = input[0].GetPropertyValue<double>("speed");
    string city = input[0].GetPropertyValue<string>("city");
    string mobile = input[0].GetPropertyValue<string>("mobile");

    if (speed > 80)
    {
       string message = string.Format("High speed detected in {0}, Vehicle No {1} and Speed {2},", city,
          vehicleNo, speed);
       log.LogInformation(message);
       SendNotifications(mobile, message);
    }
  }
 }
```
### Send sms to mobile using Notify.lk sms gateway
```
public void SendNotifications (string mobile, string msg)
{
   string smsurl = "https://app.notify.lk/api/v1/send";
   string userId = "";
   string APIKey = "";
   string senderId = "NotifyDEMO";

   string to = mobile;
   string message = msg;

   var client = new RestClient(smsurl);
   client.Timeout = -1;

   var request = new RestRequest(Method.POST);
   request.AddHeader("Content-Type", "application/json");

   var param = new Notification { user_id = userId, api_key = APIKey, sender_id = senderId, to = to,
      message = message };
   request.AddJsonBody(param);

   IRestResponse response = client.Execute(request);
   Console.WriteLine(response.Content);
}
```
