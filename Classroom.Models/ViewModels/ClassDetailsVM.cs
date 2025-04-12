using ClassroomConnect.Models;

namespace Classroom.Models.ViewModels
{
    public class ClassDetailsVM
    {
        public Class? Class { get; set; }
        public List<ClassMember>? ClassMembers { get; set; }
        public List<Assignment>? Assignments { get; set; }
    }
}