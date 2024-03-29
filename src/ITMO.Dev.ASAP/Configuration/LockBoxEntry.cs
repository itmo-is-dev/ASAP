﻿using System.Text;

namespace ITMO.Dev.ASAP.Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
public record LockBoxEntry(string Key, string TextValue, string BinaryValue)
{
    public string Value => string.IsNullOrWhiteSpace(TextValue)
        ? Encoding.UTF8.GetString(Convert.FromBase64String(BinaryValue))
        : TextValue;

    public override string ToString()
    {
        return string.Join(" = ", Key, Value);
    }
}