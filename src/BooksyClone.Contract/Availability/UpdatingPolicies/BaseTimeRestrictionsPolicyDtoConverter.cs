using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BooksyClone.Contract.Availability.UpdatingPolicies;

public class PolicyDtoConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return typeof(BaseTimeRestrictionsPolicyDto).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		var jsonObject = Newtonsoft.Json.Linq.JObject.Load(reader);
		var policyType = jsonObject["PolicyType"]?.Value<string>();

		BaseTimeRestrictionsPolicyDto result = policyType switch
		{
			"DayOfWeekTimeRestrictionsPolicy" => new DayOfWeekTimeRestrictionsPolicyDto(),
			"TestPolicy" => new TestPolicyDto(),
			_ => throw new NotSupportedException($"Policy type '{policyType}' is not supported."),
		};

		serializer.Populate(jsonObject.CreateReader(), result);
		return result;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		if(value is null)
		{
			writer.WriteNull();
			return;
		}

		var jsonObject = Newtonsoft.Json.Linq.JObject.FromObject(value, serializer);
		jsonObject.WriteTo(writer);
	}
}
