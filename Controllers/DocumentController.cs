using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public DocumentController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("{id}")]
        public ActionResult<Document> GetById(int id)
        {
            var document = _appDbContext.Documents.Find(id);

            if (document == null)
            {
                return NotFound($"Document with ID {id} not found.");
            }

            return Ok(document);
        }

        [HttpGet]
        public ActionResult Get(int take = 10, int skip = 0, string orderBy = "Id", string title = "", string type = "", DateTime? expirationAfter = null, DateTime? expirationBefore = null)
        {
            var query = _appDbContext.Documents.AsQueryable();
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(p => p.Type == type);
            }
            if (expirationAfter.HasValue)
            {
                DateOnly expirationAfterDateOnly = DateOnly.FromDateTime(expirationAfter.Value); 
                query = query.Where(p => p.Expiration > expirationAfterDateOnly);
            }

            if (expirationBefore.HasValue)
            {
                DateOnly expirationAfterDateOnly = DateOnly.FromDateTime(expirationBefore.Value);
                query = query.Where(p => p.Expiration < expirationAfterDateOnly);
            }
            query = orderBy.ToLower() switch
            {
                "title" => query.OrderBy(p => p.Title),
                "summary" => query.OrderBy(p => p.Summary),
                "type" => query.OrderBy(p => p.Type),
                "expiration" => query.OrderBy(p => p.Expiration),
                "effective" => query.OrderBy(p => p.Effective),
                _ => query.OrderBy(p => p.Id),
            };
            var totalRecord = query.Count();
            var data = query.Skip(skip).Take(take).ToList();

            var result = new
            {
                meta = new
                {
                    skip = skip,
                    take = take,
                    totalRecord = totalRecord
                },
                data = data
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Document>> Post(Document Document)
        {
            _appDbContext.Documents.Add(Document);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = Document.Id }, "Create user successfully!");
        }

        [HttpPut]
        public async Task<IActionResult> Put(Document document)
        {
            if (document == null || document.Id <= 0)
            {
                return BadRequest("Invalid or missing document Id.");
            }

            var existingDocument = await _appDbContext.Documents.FindAsync(document.Id);

            if (existingDocument == null)
            {
                return NotFound($"Document with ID {document.Id} not found.");
            }
            _appDbContext.Entry(existingDocument).CurrentValues.SetValues(document);

            await _appDbContext.SaveChangesAsync();

            return Ok($"Document with ID {document.Id} updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var Document = await _appDbContext.Documents.FindAsync(id);

            if (Document == null)
            {
                return NotFound($"Document with ID {id} not found.");
            }

            _appDbContext.Documents.Remove(Document);
            await _appDbContext.SaveChangesAsync();

            return $"Document with ID {id} deleted successfully.";
        }
    }
}
