// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace PointOfInterestSkill.Models
{
    public class InputObject
    {
        [JsonProperty("currentLatitude")]
        public double? CurrentLatitude { get; set; }

        [JsonProperty("currentLongitude")]
        public double? CurrentLongitude { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("poiType")]
        public string PoiType { get; set; }

        [JsonProperty("routeType")]
        public string RouteType { get; set; }

        [JsonProperty("showRoute")]
        public bool? ShowRoute { get; set; }
    }
}
