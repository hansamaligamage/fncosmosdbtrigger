using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace cosmosdbtrigger
{
    public class SendNotification
    {
        [FunctionName("SendNotification")]
        public void Run([CosmosDBTrigger(
            databaseName: "TrafficData",
            collectionName: "Vehicles",
            ConnectionStringSetting = "DBConnectionstring",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input, ILogger log)
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
                    string message = string.Format("High speed detected in {0}, Vehicle No {1} and Speed {2},", city, vehicleNo, speed);
                    log.LogInformation(message);
                    SendNotifications(mobile, message);
                }
            }
        }

        public void SendNotifications (string mobile, string msg)
        {

            string smsurl = "https://app.notify.lk/api/v1/send";
            string userId = "12105";
            string APIKey = "D9ApoNCnlWTPbO46sJVd";
            string senderId = "NotifyDEMO";

            string to = mobile;
            string message = msg;

            var client = new RestClient(smsurl);
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            var param = new Notification { user_id = userId, api_key = APIKey, sender_id = senderId, to = to, message = message };
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
