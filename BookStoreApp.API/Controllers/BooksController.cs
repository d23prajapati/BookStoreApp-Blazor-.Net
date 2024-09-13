using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.DTOs.Book;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Controllers {
   [Route("API/[controller]")]
   [ApiController]
   public class BooksController : ControllerBase {
      private readonly BookStoreDbContext _context;
      private readonly IMapper _mapper;
      private readonly ILogger _logger;

      public BooksController(BookStoreDbContext context, IMapper mapper, ILogger logger) {
         _context = context;
         _mapper = mapper;
         _logger = logger;
      }

      // GET: API/Books
      /// <summary>
      /// Controller action to GET all Books
      /// </summary>
      /// <returns></returns>
      [HttpGet]
      public async Task<ActionResult<IEnumerable<BookReadOnlyDTO>>> GetBooks() {
         try {
            var bookDTOs = await _context.Books
            .Include(r => r.Author)
            .ProjectTo<BookReadOnlyDTO>(_mapper.ConfigurationProvider)
            .ToListAsync();

            return Ok(bookDTOs);
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error performing GET in {nameof(GetBooks)}");
            return StatusCode(500, Messages.Error500Message);
         }
      }

      // GET: API/Books/5
      /// <summary>
      /// Controller action to GET Book based on ID
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      [HttpGet("{id}")]
      public async Task<ActionResult<BookDetailsDTO>> GetBook(int id) {
         try {
            var bookDTO = await _context.Books
            .Include(q => q.Author)
            .ProjectTo<BookDetailsDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(q => q.Id == id);

            if (bookDTO == null) {
               return NotFound();
            }

            return Ok(bookDTO);
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error performing GET in {nameof(GetBook)}");
            return StatusCode(500, Messages.Error500Message);
         }
      }

      // PUT: API/Books/5
      /// <summary>
      /// Controller action to Update book
      /// </summary>
      /// <param name="id"></param>
      /// <param name="bookDTO"></param>
      /// <returns></returns>
      [HttpPut("{id}")]
      public async Task<IActionResult> PutBook(int id, BookUpdateDTO bookDTO) {
         if (id != bookDTO.Id) {
            _logger.LogWarning($"Update ID invalid in {nameof(PutBook)} - ID: {id}");
            return BadRequest();
         }

         var book = await _context.Books.FindAsync(id);

         if (book == null) {
            return NotFound();
         }

         _mapper.Map(bookDTO, book);
         _context.Entry(book).State = EntityState.Modified;

         try {
            await _context.SaveChangesAsync();
         } catch (DbUpdateConcurrencyException ex) {
            if (!await BookExists(id)) {
               return NotFound();
            } else {
               _logger.LogError(ex, $"Error Performing GET in {nameof(PutBook)}");
               return StatusCode(500, Messages.Error500Message);
            }
         }

         return NoContent();
      }

      // POST: API/Books
      /// <summary>
      /// Controller action to Create Book
      /// </summary>
      /// <param name="book"></param>
      /// <returns></returns>
      [HttpPost]
      public async Task<ActionResult<BookCreateDTO>> PostBook(BookCreateDTO bookDTO) {
         try {
            var book = _mapper.Map<Book>(bookDTO);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error Performing POST in {nameof(PostBook)}", bookDTO);
            return StatusCode(500, Messages.Error500Message);
         }
      }

      // DELETE: API/Books/5
      /// <summary>
      /// Controller action to Delete a book
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteBook(int id) {
         try {
            var book = await _context.Books.FindAsync(id);
            if (book == null) {
               return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
         } catch (Exception ex) {
            _logger.LogError(ex, $"Error Performing DELETE in {nameof(DeleteBook)}");
            return StatusCode(500, Messages.Error500Message);
         }
      }

      private async Task<bool> BookExists(int id) {
         return await _context.Books.AnyAsync(e => e.Id == id);
      }
   }
}