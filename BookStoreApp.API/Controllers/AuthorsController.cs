using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.DTOs.Author;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Controllers {
   [Route("API/[controller]")]
   [ApiController]
   public class AuthorsController : ControllerBase {
      private readonly BookStoreDbContext _context;
      private readonly IMapper _mapper;
      private readonly ILogger<AuthorsController> _logger;

      public AuthorsController(BookStoreDbContext context, IMapper mapper, ILogger<AuthorsController> logger) {
         _context = context;
         _mapper = mapper;
         _logger = logger;
      }

      // GET: API/Authors
      /// <summary>
      /// Controller action to Get all Authors
      /// </summary>
      /// <returns></returns>
      [HttpGet]
      public async Task<ActionResult<IEnumerable<AuthorReadOnlyDTO>>> GetAuthors() {
         try {
            var authors = _mapper.Map<IEnumerable<AuthorReadOnlyDTO>>(await _context.Authors.ToListAsync());
            return Ok(authors);
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error performing GET in {nameof(GetAuthors)}");
            return StatusCode(500, Messages.Error500Message);
         }
      }

      // GET: API/Authors/5
      /// <summary>
      /// Controller action to get Author based on ID
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      [HttpGet("{id}")]
      public async Task<ActionResult<AuthorReadOnlyDTO>> GetAuthor(int id) {
         try {
            var author = await _context.Authors.FindAsync(id);

            if (author == null) {
               _logger.LogWarning($"Record Not Found: {nameof(GetAuthor)} - ID: {id}");
               return NotFound();
            }

            return Ok(_mapper.Map<AuthorReadOnlyDTO>(author));
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error performing GET in {nameof(GetAuthor)}");
            throw;
         }

      }

      // PUT: API/Authors/5
      /// <summary>
      /// Controller action to update Author information
      /// </summary>
      /// <param name="id"></param>
      /// <param name="author"></param>
      /// <returns></returns>
      [HttpPut("{id}")]
      public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDTO authorDTO) {

         if (id != authorDTO.Id) {
            _logger.LogWarning($"Update ID invalid in {nameof(PutAuthor)} - ID: {id}");
            return BadRequest();
         }

         var author = _mapper.Map<Author>(authorDTO);


         _context.Entry(author).State = EntityState.Modified;

         try {
            await _context.SaveChangesAsync();
         } catch (DbUpdateConcurrencyException ex) {
            if (!await AuthorExists(id)) {
               return NotFound();
            } else {
               _logger.LogError(ex, $"Error Performing GET in {nameof(PutAuthor)}");
               return StatusCode(500, Messages.Error500Message);
            }
         }

         return NoContent();
      }

      // POST: API/Authors
      /// <summary>
      /// Controller action to create an Author
      /// </summary>
      /// <param name="author"></param>
      /// <returns></returns>
      [HttpPost]
      public async Task<ActionResult<AuthorCreateDTO>> PostAuthor(AuthorCreateDTO authorDTO) {
         try {
            var author = _mapper.Map<Author>(authorDTO);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error Performing POST in {nameof(PostAuthor)}", authorDTO);
            return StatusCode(500, Messages.Error500Message);
         }
         
      }

      // DELETE: API/Authors/5
      /// <summary>
      /// Controller action to delete an author
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteAuthor(int id) {
         try {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) {
               return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteAuthor)}");
            return StatusCode(500, Messages.Error500Message);
         }
         
      }

      private async Task<bool> AuthorExists(int id) {
         return await _context.Authors.AnyAsync(e => e.Id == id);
      }
   }
}
