﻿using Microsoft.AspNetCore.Http;
using System;

namespace WebApplication.Core.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        { }

        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
