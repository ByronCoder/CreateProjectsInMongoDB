using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectsAPI.Models;
using MongoDB.Driver;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ProjectsAPI.Services
{
    public class ProjectService
    {
        private readonly IMongoCollection<Project> _projects;

        public ProjectService(IConfiguration config)
        {
           
            var client = new MongoClient(config.GetConnectionString("ProjectDb"));
            var database = client.GetDatabase("projects");
            _projects = database.GetCollection<Project>("Projects");

        }

        public List<Project> GetAllProjects()
        {
            try
            {
               return _projects.Find(project => true).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Project GetProjectData (string id)
        {
            try
            {
                return _projects.Find<Project>(proj => proj.Id == id).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Project AddProject(Project project)
        {
            try
            {
                _projects.InsertOne(project);
                return project;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ReplaceOneResult UpdateProject(string id, Project projectIn)
        {
            try
            {
                return _projects.ReplaceOne(project => project.Id == id, projectIn);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteEmployee(string id)
        {
            try
            {
                _projects.DeleteOne(projects => projects.Id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }
       


    }
}
