using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using trackingapi.Data;
using trackingapi.Models;
using System;
using System.Resources;
using trackingapi.Resources.controllers;

namespace trackingapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {

        private readonly IssueDbContext _context;
        private readonly IStringLocalizer<AppResource> _localizer;
        private readonly IConfiguration _configuration;

        public IssueController(IssueDbContext context,
            IStringLocalizer<AppResource> localizer,
            IStringLocalizerFactory factory,
            IConfiguration configuration) 
        {
            _context = context;
            this._localizer = localizer;
            this._configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<Issue>> Get() 
            => await _context.Issues.ToListAsync();

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Issue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            var x1 = _context.Issues.Include(x => x.Projects).FirstOrDefault();
            return issue == null ? NotFound() : Ok(issue);
        }

        [HttpGet("search/{title}")]
        [ProducesResponseType(typeof(Issue), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByTitle(string title)
        {
            var issue = await _context.Issues.SingleOrDefaultAsync(c=> c.Title == title);
            return issue == null ? NotFound() : Ok(issue);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Issue issue)
        {
            issue.Projects = new List<Project>();
            issue.Projects.Add(new Project
            {
                Id = 1,
                IssueId = issue.Id,
                Issue = issue
            });
            await _context.Issues.AddAsync(issue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = issue.Id }, issue);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, Issue issue)
        {
            if (id != issue.Id) return BadRequest();

            _context.Entry(issue).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var issueToDelete = await _context.Issues.FindAsync(id);
            if (issueToDelete == null) return NotFound();

            _context.Issues.Remove(issueToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("change-language")]
        public async Task<IActionResult> Change()
        {
            var current = _configuration["Language"];
            if (current == "en")
                _configuration["Language"] = "vi";
            else
                _configuration["Language"] = "en";

            return Ok($"Before: {current} - After: {_configuration["Language"]}");
        }

        [HttpGet("message")]
        public async Task<IActionResult> Message()
        {
            var current = _configuration["Language"];

            // Define the culture for localization (e.g., "en-US", "fr-FR", etc.)
            CultureInfo culture;
            if (current == "en")
                culture = CultureInfo.GetCultureInfo("");
            else
                culture = CultureInfo.GetCultureInfo(current);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            // Retrieve the localized string using the ResourceManager
            string localizedString = _localizer.GetString("Hello", culture);
            //var x = GetErrorMessage("TEST");
            return Ok($"Message: {localizedString}");
        }
    }
}
