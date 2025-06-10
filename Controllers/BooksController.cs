using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApi.Data;
using BookApi.Models;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BooksController(BookDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAll()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetById(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            return book;
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<Book>> Create(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Book updatedBook)
        {
            if (id != updatedBook.Id)
                return BadRequest();

            _context.Entry(updatedBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id) =>
            _context.Books.Any(b => b.Id == id);

        // GET: api/books/search?query=something
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var loweredQuery = query.ToLower();

            var results = await _context.Books
                .Where(b =>
                    b.Title.ToLower().Contains(loweredQuery) ||
                    b.Author.ToLower().Contains(loweredQuery))
                .ToListAsync();

            return Ok(results);
        }


    }
}
