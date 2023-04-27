using ITMO.Dev.ASAP.Domain.Tools;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ITMO.Dev.ASAP.DataAccess.ValueConverters;

public class SpbDateTimeValueConverter : ValueConverter<SpbDateTime, DateTime>
{
    public SpbDateTimeValueConverter()
        : base(
            x => Calendar.ToUtc(x),
            x => Calendar.FromUtc(x)) { }
}