﻿namespace Neverland.Web.Models
{
    public class AjaxResult
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public object? Data { get; set; }
    }
}
