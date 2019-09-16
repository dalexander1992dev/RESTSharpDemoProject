using System;
using RestSharp;
using System.Collections.Generic;
using RestSharp.Serialization.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using RESTSharpDemoProject.Model;
using System.Threading.Tasks;

namespace RESTSharpDemoProject
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var client = new RestClient("http://localhost:3000");

            var request = new RestRequest("posts/{postid}", Method.GET);
            request.AddUrlSegment("postid", 1);

            var response = client.Execute(request);

            /*Using: JsonDeserializer*/
            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["title"];

            /*Using: JObject*/
            JObject obs = JObject.Parse(response.Content);

            Assert.That(obs["title"].ToString(), Is.EqualTo("RestSharp Course"), "The title is not correct");
        }

        [Test]
        public void PostWithAnonymousBody()
        {
            var client = new RestClient("http://localhost:3000");

            var request = new RestRequest("posts/{postid}/profile", Method.POST);
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(new { name = "David" });

            request.AddUrlSegment("postid", 1);

            var response = client.Execute(request);

            /*Using: JsonDeserializer*/
            var deserialize = new JsonDeserializer();
            var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            var result = output["name"];

            Assert.That(result, Is.EqualTo("David"), "The Author is not correct");
        }

        [Test]
        public void PostWithTypeClassBody() //Execute Method with Generic Approach
        {
            var client = new RestClient("http://localhost:3000");

            var request = new RestRequest("posts", Method.POST);
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(new Posts() {
                id = "14",
                author ="Execute Automation",
                title ="RestSharp demo course"}
            );

            var response = client.Execute<Posts>(request); //Execute Method with Generic Approach

            /*Using: JsonDeserializer*/
            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            Assert.That(response.Data.author, Is.EqualTo("Execute Automation"), "The Author is not correct");
        }

        [Test]
        public void PostWithAsync()
        {
            var client = new RestClient("http://localhost:3000");

            var request = new RestRequest("posts", Method.POST);
            request.RequestFormat = DataFormat.Json;

            request.AddJsonBody(new Posts()
            {
                id = "14",
                author = "Execute Automation",
                title = "RestSharp demo course"
            }
            );

            var response = client.Execute<Posts>(request); //Execute Method with Generic Approach

            /*Using: JsonDeserializer*/
            //var deserialize = new JsonDeserializer();
            //var output = deserialize.Deserialize<Dictionary<string, string>>(response);
            //var result = output["author"];

            Assert.That(response.Data.author, Is.EqualTo("Execute Automation"), "The Author is not correct");
        }

        private async Task<IRestResponse<T>> ExecuteAsyncRequest<T>(RestClient client, IRestResponse request)where T:class, new()
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse<T>>();

           // client.ExecuteAsync<T>(request, NewMethod(taskCompletionSource));

            return await taskCompletionSource.Task;
        }

        private static Action<IRestResponse<T>> NewMethod<T>(TaskCompletionSource<IRestResponse<T>> taskCompletionSource) where T : class, new()
        {
            return restresponse =>
            {
                if (restresponse.ErrorException != null)
                {
                    const string message = "Error retrieving response.";
                    throw new ApplicationException(message, restresponse.ErrorException);
                }
                taskCompletionSource.SetResult(restresponse);
            };
        }
    }
}
