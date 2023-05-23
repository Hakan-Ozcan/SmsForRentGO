using Newtonsoft.Json;
using System.Net;
public class SmsSonuc
{
    public string Sonuc { get; set; }
    public string Kontor { get; set; }
    public string Message { get; set; }

}
class TokenAlma
{
    public static string GetToken()
    {
        string username = "ttapiuser1";
        string password = "ttapiuser1123";
        string grant_type = "password";

        string tokenUrl = "https://restapi.ttmesaj.com/ttmesajToken";
        var request = (HttpWebRequest)WebRequest.Create(tokenUrl);
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";

        var requestContent = $"grant_type={WebUtility.UrlEncode(grant_type)}&username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}";
        var requestData = System.Text.Encoding.UTF8.GetBytes(requestContent);

        using (var requestStream = request.GetRequestStream())// HTTP talebinin gövde akışını almak için kullanılır.
        {
            requestStream.Write(requestData, 0, requestData.Length);//requestStream üzerindeki Write metodu, requestData adında bir bayt dizisi (byte array) yazmayı sağlar.
        }

        try
        {
            using (var response = (HttpWebResponse)request.GetResponse())//HTTP talebini sunucuya gönderir ve sunucudan bir yanıt alır.
            using (var responseStream = response.GetResponseStream())//yanıtın gövde akışını almak için kullanılır.
            using (var reader = new StreamReader(responseStream))//StreamReader sınıfı, bir akıştan metin okumak için kullanılır.
            {
                var responseContent = reader.ReadToEnd();//reader üzerinden tüm metni okur ve bir dize olarak döndürür.
                return responseContent;
            }
        }
        catch (WebException ex)
        {
            var errorResponse = (HttpWebResponse)ex.Response;
            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
            {
                var errorResponseContent = reader.ReadToEnd();
                Console.WriteLine($"Hata Kodu: {errorResponse.StatusCode}");
                Console.WriteLine($"Hata Mesajı: {errorResponseContent}");
            }
            // Hata durumunda null veya boş bir değer döndürebilirsiniz.
            return null;
        }
       
    }
    public static void SendSMSTTNet()
    {
        var token = GetToken();
       

        string ApiUrl = "https://restapi.ttmesaj.com/";
        string description = string.Empty;


        var data = new
        {
            username = "tunalar.oto",
            password = "T8L3M1A5",
            numbers = "905369900342",
            message = "TUNALAR",
            origin = "Erol Tuna",
            sd = "0",
            ed = "0",
            isNotification = false,
            recipentType = "BIREYSEL",
            brandCode = "Opel"
        };

        var content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");

        using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Token Hatalı");
                Console.ReadLine();
                //initializer.TraceMe("Token Hatalı");             
            }
            else
            {


                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var httppost = httpClient.PostAsync(ApiUrl + "api/SendSms/SendSingle", content).Result;
                var response = JsonConvert.DeserializeObject<SmsSonuc>(httppost.Content.ReadAsStringAsync().Result);

                if (response.Sonuc.Contains("*OK*")) // success
                {
                    description = response.Sonuc.Replace("*OK*", "");

                    Console.WriteLine("Mesajınız başarıyla teslim edilmiştir." + Environment.NewLine + Environment.NewLine + "Message Id : " + description + Environment.NewLine + "Kontor : " + response.Kontor);
                    Console.ReadLine();
                    //initializer.TraceMe("Mesajınız başarıyla teslim edilmiştir." + Environment.NewLine + Environment.NewLine + "Message Id : " + description + Environment.NewLine + "Kontor : " + response.Kontor);
                }
                else
                {
                    //initializer.TraceMe("Hata : " + response.Message);
                    Console.WriteLine("Hata : " + response.Message);
                    Console.ReadLine();

                }
            }


        }
    }
}

