using System.Text.Json;
using System.Text.Json.Serialization;

namespace JatetxeaApi.DTOak
{
    public class DeskuntuKodeaDto
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("kodea")]
        public string? Kodea { get; set; }

        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }

        [JsonPropertyName("coupon_code")]
        public string? CouponCode { get; set; }

        [JsonPropertyName("discount_code")]
        public string? DiscountCode { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("order_id")]
        public int? OrderId { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraFields { get; set; }

        public string? GetCode()
        {
            var directCode = FirstNotBlank(Code, Kodea, Codigo, CouponCode, DiscountCode, Name);
            if (!string.IsNullOrWhiteSpace(directCode))
            {
                return directCode;
            }

            if (ExtraFields == null)
            {
                return null;
            }

            foreach (var fieldName in new[] { "Code", "Kodea", "Codigo", "couponCode", "discountCode" })
            {
                if (ExtraFields.TryGetValue(fieldName, out var value) && value.ValueKind == JsonValueKind.String)
                {
                    var text = value.GetString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return text;
                    }
                }
            }

            return null;
        }

        private static string? FirstNotBlank(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }
    }

    public class DeskuntuEmaitzaDto
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("percentage")]
        public double Percentage { get; set; }

        [JsonPropertyName("code_id")]
        public int? CodeId { get; set; }
    }
}
