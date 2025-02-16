﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class MongoProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 0)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.String)]
        [BsonElement(Order = 1)]
        public string ProductName { get; set; }
        [BsonRepresentation(BsonType.Int32)]
        [BsonElement(Order = 2)]
        public int Count { get; set; }
        [BsonRepresentation(BsonType.Boolean)]
        [BsonElement(Order = 3)]
        public bool IsAvaliable { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        [BsonElement(Order = 4)]
        public decimal Price { get; set; }
    }
}
