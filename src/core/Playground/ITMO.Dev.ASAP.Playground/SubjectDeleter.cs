using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Domain.DeadlinePolicies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Assignment = ITMO.Dev.ASAP.Domain.Study.Assignment;
using GroupAssignment = ITMO.Dev.ASAP.Domain.Study.GroupAssignment;
using Mentor = ITMO.Dev.ASAP.Domain.Users.Mentor;
using Subject = ITMO.Dev.ASAP.Domain.Study.Subject;
using SubjectCourse = ITMO.Dev.ASAP.Domain.Study.SubjectCourse;
using SubjectCourseAssociation = ITMO.Dev.ASAP.Domain.SubjectCourseAssociations.SubjectCourseAssociation;
using SubjectCourseGroup = ITMO.Dev.ASAP.Domain.Study.SubjectCourseGroup;
using Submission = ITMO.Dev.ASAP.Domain.Submissions.Submission;

namespace ITMO.Dev.ASAP.Playground;

public class SubjectDeleter
{
    private readonly IDatabaseContext _context;

    public SubjectDeleter(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task DeleteSubject(Guid subjectId)
    {
        await using IDbContextTransaction transaction = await _context.BeginTransactionAsync(default);

        List<Submission> submissions = await _context.Submissions
            .Where(x => x.GroupAssignment.Assignment.SubjectCourse.Subject.Id.Equals(subjectId))
            .ToListAsync();

        _context.Submissions.RemoveRange(submissions);

        await _context.SaveChangesAsync(default);

        List<GroupAssignment> groupAssignments = await _context.GroupAssignments
            .Where(x => x.Assignment.SubjectCourse.Subject.Id.Equals(subjectId))
            .ToListAsync();

        _context.GroupAssignments.RemoveRange(groupAssignments);
        await _context.SaveChangesAsync(default);

        List<Assignment> assignments = await _context.Assignments
            .Where(x => x.SubjectCourse.Subject.Id.Equals(subjectId))
            .ToListAsync();

        _context.Assignments.RemoveRange(assignments);
        await _context.SaveChangesAsync(default);

        List<SubjectCourseAssociation> subjectCourseAssociations = await _context.SubjectCourseAssociations
            .Where(x => x.SubjectCourse.Subject.Id.Equals(subjectId))
            .ToListAsync();

        _context.SubjectCourseAssociations.RemoveRange(subjectCourseAssociations);
        await _context.SaveChangesAsync(default);

        List<SubjectCourseGroup> courseGroups = await _context.SubjectCourseGroups
            .Where(x => x.SubjectCourse.Subject.Id.Equals(subjectId))
            .ToListAsync();

        _context.SubjectCourseGroups.RemoveRange(courseGroups);
        await _context.SaveChangesAsync(default);

        List<Mentor> mentors = await _context.Mentors
            .Where(x => x.Course.Subject.Id.Equals(subjectId))
            .ToListAsync();

        _context.Mentors.RemoveRange(mentors);
        await _context.SaveChangesAsync(default);

        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.Subject.Id.Equals(subjectId))
            .Include(x => x.DeadlinePolicies)
            .ToListAsync();

        IEnumerable<DeadlinePolicy> deadlinePolicies = courses.SelectMany(x => x.DeadlinePolicies);

        _context.DeadlinePolicies.RemoveRange(deadlinePolicies);
        await _context.SaveChangesAsync(default);

        _context.SubjectCourses.RemoveRange(courses);
        await _context.SaveChangesAsync(default);

        Subject subject = await _context.Subjects.GetByIdAsync(subjectId);

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync(default);

        await transaction.CommitAsync();
    }
}