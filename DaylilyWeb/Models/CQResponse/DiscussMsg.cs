﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaylilyWeb.Models.CQResponse
{
    public class DiscussMsg
    {
        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }
        [JsonProperty(PropertyName = "post_type")]
        public string PostType { get; set; }
        [JsonProperty(PropertyName = "message_type")]
        public string MessageType { get; set; }
        public string SubType { get; set; }
        [JsonProperty(PropertyName = "message_id")]
        public long MessageId { get; set; }
        [JsonProperty(PropertyName = "discuss_id")]
        public long DiscussId { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public long UserId { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "font")]
        public long Font { get; set; }
        [JsonProperty(PropertyName = "self_id")]
        public string SelfId { get; set; }
    }
}