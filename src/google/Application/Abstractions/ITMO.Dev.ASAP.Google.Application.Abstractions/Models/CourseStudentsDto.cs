using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.Google.Application.Abstractions.Models;

public record struct CourseStudentsDto(IReadOnlyList<StudentPointsDto> StudentsPoints);