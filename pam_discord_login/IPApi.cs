using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace pam_discord_login
{
    public class IPInfo
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("region_code")]
        public string RegionCode { get; set; }

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }

        [JsonPropertyName("country_code_iso3")]
        public string CountryCodeIso3 { get; set; }

        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }

        [JsonPropertyName("country_capital")]
        public string CountryCapital { get; set; }

        [JsonPropertyName("country_tld")]
        public string CountryTld { get; set; }

        [JsonPropertyName("continent_code")]
        public string ContinentCode { get; set; }

        [JsonPropertyName("in_eu")]
        public bool InEu { get; set; }

        [JsonPropertyName("postal")]
        public string Postal { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("utc_offset")]
        public string UtcOffset { get; set; }

        [JsonPropertyName("country_calling_code")]
        public string CountryCallingCode { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("currency_name")]
        public string CurrencyName { get; set; }

        [JsonPropertyName("languages")]
        public string Languages { get; set; }

        [JsonPropertyName("country_area")]
        public double CountryArea { get; set; }

        [JsonPropertyName("country_population")]
        public double CountryPopulation { get; set; }

        [JsonPropertyName("asn")]
        public string Asn { get; set; }

        [JsonPropertyName("org")]
        public string Org { get; set; }
    }

    public class IPApi
    {
        public static async Task<IPInfo> GetInfo(IPAddress ip)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://ipapi.co/{ip.ToString()}/json"),    
                Headers =
                {
                    { "User-Agent", "Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail Firefox/firefoxversion" },
                },
            };
            
            using (var response = await client.SendAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                    return JsonSerializer.Deserialize<IPInfo>(await response.Content.ReadAsStringAsync());
                else
                    return null;
            }
        }
    }
}