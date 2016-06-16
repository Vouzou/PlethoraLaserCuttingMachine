﻿//Reference: http://dotnetbyexample.blogspot.com/2012/02/json-deserialization-with-jsonnet-class.html

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlethoraModels;

namespace ImportJsonApplication
{
    public abstract class PlethoraJsonConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jsonObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(objectType, jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class JsonEdgeConverter : PlethoraJsonConverter<Edge>
    {
        protected override Edge Create(Type objectType, JObject jsonObject)
        {
            var typeName = jsonObject["Type"].ToString();
            switch (typeName)
            {
                case "LineSegment":
                    return new LineSegment();
                case "CircularArc":
                    return new CircularArc();
                default: return null;
            }
        }
    }
}
