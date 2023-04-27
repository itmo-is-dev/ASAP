using ITMO.Dev.ASAP.Domain.ValueObject;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ITMO.Dev.ASAP.DataAccess.ValueConverters;

public class PointsValueConverter : ValueConverter<Points, double>
{
    public PointsValueConverter()
        : base(x => x.Value, x => new Points(x)) { }
}