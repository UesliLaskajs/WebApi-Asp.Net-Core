﻿using System.Text.Json;

namespace BackOfficeInventoryApi.Models
{
    public class ErrorModel
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString() => JsonSerializer.Serialize(this);
        
    }
}
