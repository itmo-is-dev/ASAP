using ITMO.Dev.ASAP.Google.Application.Abstractions.Providers;
using System.Globalization;

namespace ITMO.Dev.ASAP.Google.Application.Providers;

public class RuCultureInfoProvider : ICultureInfoProvider
{
    private static readonly CultureInfo RuCultureInfo = new CultureInfo("ru-RU");

    public CultureInfo GetCultureInfo()
    {
        return RuCultureInfo;
    }
}