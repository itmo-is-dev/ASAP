namespace ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;

public record GoogleSubjectCourseAssociationDto(
    Guid SubjectCourseId,
    string SpreadsheetId) : SubjectCourseAssociationDto(SubjectCourseId);