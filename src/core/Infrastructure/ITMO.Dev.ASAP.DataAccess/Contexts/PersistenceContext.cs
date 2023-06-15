using ITMO.Dev.ASAP.Application.DataAccess;
using ITMO.Dev.ASAP.Application.DataAccess.Repositories;

namespace ITMO.Dev.ASAP.DataAccess.Contexts;

public class PersistenceContext : IPersistenceContext
{
    private readonly DatabaseContext _context;

    public PersistenceContext(
        IUserRepository users,
        IStudentRepository students,
        IMentorRepository mentors,
        IAssignmentRepository assignments,
        IGroupAssignmentRepository groupAssignments,
        IStudentGroupRepository studentGroups,
        ISubjectRepository subjects,
        ISubjectCourseRepository subjectCourses,
        ISubmissionRepository submissions,
        IUserAssociationRepository userAssociations,
        IStudentAssignmentRepository studentAssignments,
        DatabaseContext context)
    {
        Users = users;
        Students = students;
        Mentors = mentors;
        Assignments = assignments;
        GroupAssignments = groupAssignments;
        StudentGroups = studentGroups;
        Subjects = subjects;
        SubjectCourses = subjectCourses;
        Submissions = submissions;
        UserAssociations = userAssociations;
        StudentAssignments = studentAssignments;
        _context = context;
    }

    public IUserRepository Users { get; }

    public IStudentRepository Students { get; }

    public IMentorRepository Mentors { get; }

    public IAssignmentRepository Assignments { get; }

    public IGroupAssignmentRepository GroupAssignments { get; }

    public IStudentGroupRepository StudentGroups { get; }

    public ISubjectRepository Subjects { get; }

    public ISubjectCourseRepository SubjectCourses { get; }

    public ISubmissionRepository Submissions { get; }

    public IUserAssociationRepository UserAssociations { get; }

    public IStudentAssignmentRepository StudentAssignments { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}