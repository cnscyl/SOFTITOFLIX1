using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOFTITOFLIX.Data;
using SOFTITOFLIX.Models;
using Microsoft.AspNetCore.Authorization;
using SOFTITOFLIX.Models.CompositeModels;
using System.Security.Claims;
using SOFTITOFLIX.DTO.Requests.MediaRequests;
using SOFTITOFLIX.DTO.Converters;
using SOFTITOFLIX.DTO.Responses.MediaResponses;

namespace SOFTITOFLIX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly SOFTITOFLIXContext _context;

        MediaConverter mediaConverter = new MediaConverter();

        public MediaController(SOFTITOFLIXContext context)
        {
            _context = context;
        }

        // GET: api/Media
        [HttpGet]
        [Authorize]
        public List<MediaGetResponse> GetAllMedia(
            bool includePassive = false,
            bool includeMediaCategories = false,
            bool includeMediaActors = false,
            bool includeMediaDirectors = false,
            bool includeMediaRestrictions = false)
        {
            IQueryable<Media> media = _context.Media;

            if(includePassive == true)
            {
                media = media.Where(m => m.Passive == true);
            }
            if (includeMediaCategories == true)
            {
                media = media.Include(m => m.MediaCategories);
            }
            if (includeMediaActors == true)
            {
                media = media.Include(m => m.MediaActors);
            }
            if (includeMediaDirectors == true)
            {
                media = media.Include(m => m.MediaDirectors);
            }
            if (includeMediaRestrictions == true)
            {
                media = media.Include(m => m.MediaRestrictions);
            }
            return mediaConverter.Convert(media.ToList());
        }

        // GET: api/Media/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<MediaGetResponse> GetMedia(
            int id,
            bool includePassive = false,
            bool includeMediaCategories = false,
            bool includeMediaActors = false,
            bool includeMediaDirectors = false,
            bool includeMediaRestrictions = false)
        {
            IQueryable<Media> media = _context.Media;

            if (media == null)
            {
                return NotFound();
            }
            if (includePassive == true)
            {
                media = media.Where(m => m.Passive == true);
            }
            if (includeMediaCategories == true)
            {
                media = media.Include(m => m.MediaCategories);
            }
            if (includeMediaActors == true)
            {
                media = media.Include(m => m.MediaActors);
            }
            if (includeMediaDirectors == true)
            {
                media = media.Include(m => m.MediaDirectors);
            }
            if (includeMediaRestrictions == true)
            {
                media = media.Include(m => m.MediaRestrictions);
            }
             
            return mediaConverter.Convert(media.Where(m => m.Id == id).FirstOrDefault());
        }

        // PUT: api/Media/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ContentAdmin")]
        public ActionResult PutMedia(int id, Media media)
        {
            if (id != media.Id)
            {
                return BadRequest();
            }
            Media? currentMedia = _context.Media.Find(id);


            if (media != null)
            {
                currentMedia.Name = media.Name;
                currentMedia.Description = media.Description;
                currentMedia.Passive = media.Passive;
                //mediaactor, mediacatagory, mediadirector, restrictions are missing
                try
                {
                    _context.SaveChanges();
                    return Ok();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediaExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return NotFound();
            }
            
        }

        // POST: api/Media
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ContentAdmin")]
        public ActionResult PostMedia(MediaCreateRequest mediaCreateRequest)
        {
            Media newMedia = mediaConverter.Convert(mediaCreateRequest);

            _context.Media.Update(newMedia);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("Favorite")]
        [Authorize]
        public ActionResult AddFavorite(int mediaId)
        {
            UserFavorite userFavorite = new UserFavorite();
            Media? media = _context.Media.Find(mediaId);


            if (media == null)
            {
                return NoContent();
            }
            try
            {
                userFavorite.MediaId = mediaId;
                userFavorite.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                _context.UserFavorites.Add(userFavorite);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

        // DELETE: api/Media/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ContentAdmin")]
        public ActionResult DeleteMedia(int id)
        {
            if (_context.Media == null)
            {
                return NotFound();
            }
             Media? media =  _context.Media.Find(id);
            if (media == null)
            {
                return NotFound();
            }

            media.Passive = true;
            _context.Media.Update(media);
            return Ok();
        }

        private bool MediaExists(int id)
        {
            return (_context.Media?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
