using InfrastructureCore;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Parking.Helper
{
    public static class ApiCaller
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string baseUrl;

        public static void SetBaseUrl(string url)
        {
            baseUrl = url;
        }

        public static async Task<string> CallApiAsync(string apiEndpoint, string queryString)
        {
            string apiUrl = baseUrl + apiEndpoint + queryString;

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return apiResponse;
                }
                else
                {
                    Console.WriteLine("API call failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return null;
        }
        public static async Task<Result> AddVehicleUserToFolder(string userId, string licenseNumber, string imageVehicle64)
        {
            var result = new Result();
            string apiEndpoint = "/addVehicleUser";
            string apiUrl = baseUrl + apiEndpoint;
            try
            {
                var requestBody = new
                {
                    userId = userId,
                    licenseNumber = licenseNumber,
                    imageVehicle64 = imageVehicle64
                };
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result.Data = apiResponse;
                    result.Success = true;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = response.RequestMessage.ToString();
                }

            }
            catch(Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public static async Task<Result> VerifyLicensePlateAsync(string licensePlateImage)
        {
            var result = new Result();
            string apiEndpoint = "/detectVehicle";
            string apiUrl = baseUrl + apiEndpoint;

            try
            {
                // Prepare the request body
                var requestBody = new
                {
                    imagelp64 = licensePlateImage
                };
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    result.Data = apiResponse;
                    result.Success = true;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = response.RequestMessage.ToString();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
