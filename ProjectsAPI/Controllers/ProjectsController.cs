using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsAPI.Services;
using ProjectsAPI.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace ProjectsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projService;

        public ProjectsController(ProjectService projService)
        {
            _projService = projService;

        }

        [HttpGet]
        public ActionResult<IEnumerable<Project>> Index()
        {
            return _projService.GetAllProjects();
        }

        [HttpGet("{id}")]
        public ActionResult<Project> Details(string id)
        {
            var proj = _projService.GetProjectData(id);

            if (proj == null)
            {
                return NotFound();
            }

            return proj;
        }

        [HttpPost, Authorize]
        public ActionResult<Project> Create(Project project)
        {
            return _projService.AddProject(project);
        }

        [HttpPut("{id}"), Authorize]
        public ReplaceOneResult Edit(string id, Project projIn)
        {
            var proj = _projService.GetProjectData(id);

            return _projService.UpdateProject(id, projIn);
           
        }

      [HttpDelete("{id}"), Authorize]
      public ActionResult Delete(string id)
      {
            var proj = _projService.GetProjectData(id);

            _projService.DeleteEmployee(proj.Id);

            return NoContent();
      }

    }
}