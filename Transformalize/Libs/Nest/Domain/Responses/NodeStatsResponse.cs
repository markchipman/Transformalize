﻿using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Stats;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
    public interface INodeStatsResponse : IResponse
    {
        string ClusterName { get; }
        Dictionary<string, NodeStats> Nodes { get; }
    }

    [JsonObject]
    public class NodeStatsResponse : BaseResponse, INodeStatsResponse
    {
        public NodeStatsResponse()
        {
            this.IsValid = true;
        }

        [JsonProperty(PropertyName = "cluster_name")]
        public string ClusterName { get; internal set; }
        [JsonProperty(PropertyName = "nodes")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, NodeStats> Nodes { get; set; }
    }
}