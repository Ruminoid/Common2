using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ReactiveUI;

namespace Ruminoid.Common2.Utils.UserTypes
{
    [PublicAPI]
    [JsonConverter(typeof(RateJsonConverter))]
    public class Rate : ReactiveObject
    {
        public Rate(RateValue value = RateValue.Unknown) => _value = value;

        private RateValue _value;

        public RateValue Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public static implicit operator Rate(RateValue value) =>
            new(value);

        public static implicit operator RateValue(Rate rate) =>
            rate.Value;

        public static implicit operator Rate([ValueRange(0, 5)] int value) =>
            (RateValue) value;

        public static implicit operator int(Rate rate) =>
            (int) rate.Value;
    }

    [PublicAPI]
    public enum RateValue
    {
        OneStar = 0,
        TwoStars,
        Unknown,
        ThreeStars,
        FourStars,
        FiveStars
    }

    internal class RateJsonConverter : JsonConverter<Rate>
    {
        public override void WriteJson(JsonWriter writer, Rate value, JsonSerializer serializer)
        {
            if (value != null) writer.WriteValue((int) value.Value);
            else writer.WriteNull();
        }

        public override Rate ReadJson(JsonReader reader, Type objectType, Rate existingValue, bool hasExistingValue,
            JsonSerializer serializer) =>
            (RateValue?) reader.Value;
    }
}
