﻿namespace Kysect.Shreks.Integration.Google.Sheets;

public interface ISheet<in TData> : ISheet
{
    Task UpdateAsync(TData data, CancellationToken token);
}

public interface ISheet
{
    string Title { get; }

    int Id { get; }
}