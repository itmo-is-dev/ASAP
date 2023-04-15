using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubjectCourseAssociationMapping
{
    public static GoogleSubjectCourseAssociationDto ToDto(this GoogleTableSubjectCourseAssociation association)
    {
        return new GoogleSubjectCourseAssociationDto(association.SpreadsheetId);
    }

    public static SubjectCourseAssociationDto ToDto(this SubjectCourseAssociation entity)
    {
        return entity switch
        {
            GoogleTableSubjectCourseAssociation association => association.ToDto(),
            _ => throw new ArgumentOutOfRangeException(nameof(entity)),
        };
    }

    public static GoogleTableSubjectCourseAssociation ToEntity(
        this GoogleSubjectCourseAssociationDto dto,
        SubjectCourse subjectCourse)
    {
        return new GoogleTableSubjectCourseAssociation(
            Guid.NewGuid(),
            subjectCourse,
            dto.SpreadsheetId);
    }

    public static SubjectCourseAssociation ToEntity(this SubjectCourseAssociationDto dto, SubjectCourse subjectCourse)
    {
        return dto switch
        {
            GithubSubjectCourseAssociationDto association => association.ToEntity(subjectCourse),
            GoogleSubjectCourseAssociationDto association => association.ToEntity(subjectCourse),
            _ => throw new ArgumentOutOfRangeException(nameof(dto)),
        };
    }
}