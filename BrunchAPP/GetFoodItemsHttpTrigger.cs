using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BrunchAPP.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BrunchAPP
{
    public static class GetFoodItemsHttpTrigger
    {
        [Function("GetFoodItems")]
        public static HttpResponseData GetFoodItems([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);

            response.WriteAsJsonAsync(FoodItemStore.fooditems);

            return response;
        }
        [Function("GetFoodItemByID")]
        public static HttpResponseData GetFoodItemByID([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fooditem/{id}")] HttpRequestData req,
           FunctionContext executionContext , string id)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            var foodItem = FoodItemStore.fooditems.FirstOrDefault(f => f.Id.Equals(id));
            if (foodItem == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            response.WriteAsJsonAsync(foodItem);

            return response;
        }

        [Function("AddFoodItem")]
        public static async Task<HttpResponseData> AddFoodItemAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fooditem")] HttpRequestData req,
          FunctionContext executionContext)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var fooditem = JsonConvert.DeserializeObject<FoodItem>(requestBody);

            FoodItemStore.fooditems.Add(fooditem);

            response.WriteAsJsonAsync(fooditem);
            return response;
        }

        [Function("DeleteFoodItem")]
        public static async Task<HttpResponseData> DeleteFoodItem([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "fooditem/{id}")] HttpRequestData req,
          FunctionContext executionContext, string id)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);

            var fooditem = FoodItemStore.fooditems.FirstOrDefault(f => f.Id.Equals(id));

            if (fooditem == null)
            {
                return null;
            }

            FoodItemStore.fooditems.Remove(fooditem);
            response.WriteAsJsonAsync(fooditem);
            return response;
        }
    }
}
