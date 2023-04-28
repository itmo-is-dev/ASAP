using System.Globalization;

namespace ITMO.Dev.ASAP.Google.Application.Abstractions.Providers;

public interface ICultureInfoProvider
{
    CultureInfo GetCultureInfo();
}