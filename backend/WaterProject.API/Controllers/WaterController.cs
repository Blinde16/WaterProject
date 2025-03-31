using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WaterProject.API.Data;

namespace WaterProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterController : ControllerBase
    {
        private WaterDbContext _waterContext;
        public WaterController(WaterDbContext temp) 
        { 
            _waterContext = temp;
        }
        [HttpGet("AllProjects")]
        public IActionResult GetProjects(int pageSize = 10, int PageNumber = 1, [FromQuery] List<string>? projectTypes = null)
        {
            var query = _waterContext.Projects.AsQueryable();

            if (projectTypes != null && projectTypes.Any())
            {
                query = query.Where(p => projectTypes.Contains(p.ProjectType));
            }

            var totalNumberProjects = query.Count();

            var something = query
                .Skip((PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            

            var someObject = new
            {
                Projects = something,
                TotalNumberProjects = totalNumberProjects
            };

            return Ok(someObject);
        }
        [HttpGet("GetProjectTypes")]
        public IActionResult GetProjectTypes ()
        {
            var projectTypes = _waterContext.Projects
                .Select(p => p.ProjectType)
                .Distinct()
                .ToList();

            return Ok(projectTypes);
        }
        [HttpPost("AddProject")]
        public IActionResult AddProject([FromBody] Project newProject)
        {
            _waterContext.Projects.Add(newProject);
            _waterContext.SaveChanges();
            return Ok(newProject);
        }

        [HttpPut("updateProject/{projectId}")]
        public IActionResult updateProject(int projectId, [FromBody] Project updatedProject)
        {
            var existingProject = _waterContext.Projects.Find(projectId);

            existingProject.ProjectName = updatedProject.ProjectName;
            existingProject.ProjectType = updatedProject.ProjectType;
            existingProject.ProjectRegionalProgram = updatedProject.ProjectRegionalProgram;
            existingProject.ProjectImpact = updatedProject.ProjectImpact;
            existingProject.ProjectPhase = updatedProject.ProjectPhase;
            existingProject.ProjectFunctionalityStatus = updatedProject.ProjectFunctionalityStatus;

            _waterContext.Projects.Update(existingProject);
            _waterContext.SaveChanges();
            return Ok(existingProject);
        }
        [HttpDelete("DeleteProject/{projectId}")]
        public IActionResult DeleteProject(int projectId)
        {
            var project = _waterContext.Projects.Find(projectId);

            if (project == null)
            {
                return NotFound(new {message = "project not found"});
            }

            _waterContext.Projects.Remove(project);
            _waterContext.SaveChanges();
            return NoContent();
        }
    }
}
