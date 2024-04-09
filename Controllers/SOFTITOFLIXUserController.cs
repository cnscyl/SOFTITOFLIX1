using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOFTITOFLIX.Data;
using SOFTITOFLIX.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SOFTITOFLIX.DTO;
using SOFTITOFLIX.DTO.Responses;
using SOFTITOFLIX.DTO.Converters;
using SOFTITOFLIX.DTO.Requests;
using SOFTITOFLIX.Models.CompositeModels;

namespace SOFTITOFLIX.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SOFTITOFLIXUserController : ControllerBase
    {

        public struct ValidateTokenVM
        {
            public long Id { get; set; }
            public string ResetPasswordToken { get; set; }
            public string NewPassword { get; set; }
        }
        private readonly SignInManager<SOFTITOFLIXUser> _signInManager;
        private readonly SOFTITOFLIXContext _context;

        UserConverter userConverter = new UserConverter();

        public SOFTITOFLIXUserController(SignInManager<SOFTITOFLIXUser> signInManager, SOFTITOFLIXContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        // GET: api/SOFTITOFLIXUser
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult<List<UserGetResponse>> GetUsers(bool includePassive = true)
        {
            IQueryable<SOFTITOFLIXUser> users = _signInManager.UserManager.Users;

            if (includePassive == false)
            {
                users = users.Where(u => u.Passive == false);
            }
            
            return userConverter.Convert(users.AsNoTracking().ToList());
        }

        // GET: api/SOFTITOFLIXUser/5
        [HttpGet("{id}")] 
        [Authorize]
        public ActionResult<UserGetResponse> GetSOFTITOFLIXUser(long id)
        {
            SOFTITOFLIXUser? SOFTiTOFLIXUser;

            if (User.IsInRole("Administrator") == false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }
            }
            SOFTiTOFLIXUser = _signInManager.UserManager.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefault();

            if (SOFTiTOFLIXUser == null)
            {
              return NotFound();
            }
            else
            {
                UserGetResponse userGetResponse = userConverter.Convert(SOFTiTOFLIXUser);
                return userGetResponse;
            }
        }

        // PUT: api/SOFTITOFLIXUser/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        [Authorize]
        public ActionResult PutSOFTITOFLIXUser(long id, UserUpdateRequest userUpdateRequest)
        {
            SOFTITOFLIXUser? user;

            if (User.IsInRole("CustomerRepresentive") == false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }
            }

            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userUpdateRequest.UserName;
            user.PhoneNumber = userUpdateRequest.PhoneNumber;
            user.Email = userUpdateRequest.Email;

            _signInManager.UserManager.UpdateAsync(user).Wait();

            return Ok();
        }

        // POST: api/SOFTITOFLIXUser
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<string> PostSOFTITOFLIXUser(UserCreateRequest userCreateRequest)
        {
            //if (User.Identity!.IsAuthenticated == true)
            //{
            //    return BadRequest();
            //}

            SOFTITOFLIXUser newSOFTITOFLIXUser =  userConverter.Convert(userCreateRequest);

            IdentityResult identityResult = _signInManager.UserManager.CreateAsync(newSOFTITOFLIXUser, userCreateRequest.Password).Result;

            if(identityResult != IdentityResult.Success)
            {
                return identityResult.Errors.FirstOrDefault()!.Description;
            }
            return Ok(newSOFTITOFLIXUser.Id);
        }

        // DELETE: api/SOFTITOFLIXUser/5
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteSOFTITOFLIXUser(long id)
        {
            SOFTITOFLIXUser? user = null;

            if (User.IsInRole("CustomerRepresentive") == false)
            {
                if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
                {
                    return Unauthorized();
                }
            }
            user = _signInManager.UserManager.Users.Where(u => u.Id == id).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            if (user == null)
            {
                return NotFound();
            }

            user.Passive = true;
            _signInManager.UserManager.UpdateAsync(user).Wait();
            return Ok();
        }

        [HttpPost("Login")]
        public ActionResult<List<Media>> Login(LoginRequest loginRequest)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            List<Media>? medias = null;
            List<UserFavorite> userFavorites;
            IGrouping<short, MediaCategory>? mediaCategories;
            IQueryable<Media> mediaQuery;
            IQueryable<int> userWatcheds;

            SOFTITOFLIXUser? user = _signInManager.UserManager.FindByNameAsync(loginRequest.UserName).Result;

            if(user == null)
            {
                return NotFound();
            }
            else if (_context.UserSubscriptions.Where(us=>us.UserId == user!.Id && us.EndDate <= DateTime.Today).Any())
            {
                user.Passive = true;
                _signInManager.UserManager.UpdateAsync(user).Wait();
            }
            else if (user.Passive == true)
            {
                return Content("Passive");
            }

            signInResult = _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false).Result;
            if (signInResult.Succeeded == true)
            {
                //Kullanıcının favori olarak işaretlediği mediaları ve kategorilerini alıyoruz.
                userFavorites = _context.UserFavorites
                    .Where(u => u.UserId == user.Id)
                    .Include(u => u.Media)
                    .Include(u => u.Media!.MediaCategories).ToList();

                //userFavorites içindeki media kategorilerini ayıklıyoruz (SelectMany)
                //Bunları kategori id'lerine göre grupluyoruz (GroupBy)
                //Her grupta kaç adet item olduğuna bakıp (m.Count())
                //Çoktan aza doğru sıralıyoruz (OrderByDescending)
                //En üstteki, yani en çok item'a sahip grubu seçiyoruz (FirstOrDefault)
                mediaCategories = userFavorites.SelectMany(u => u.Media!.MediaCategories!)
                                               .GroupBy(mc => mc.CategoryId)
                                               .OrderByDescending(mc => mc.Count())
                                               .FirstOrDefault();

                if(mediaCategories != null)
                {
                    //Kullanıcının izlediği episode'lardan media'ya ulaşıp, sadece media id'lerini alıyoruz (Select)
                    //Tekrar eden media id'leri eliyoruz (Distinct)
                    userWatcheds = _context.UserWatcheds.Where(u => u.UserId == user.Id).Include(u => u.Episode).Select(u => u.Episode!.MediaId).Distinct();
                    //Öneri yapmak için mediaCategories.Key'i yani kullanıcının en çok favorilediği kategori id'sini kullanıyoruz
                    //Media listesini çekerken sadece bu kategoriye ait mediaların MediaCategories listesini dolduruyoruz
                    //(Include(m => m.MediaCategories!.Where(mc => mc.CategoryId == mediaCategories.Key)))
                    //Diğer mediaların MediaCategories listeleri boş kalıyor
                    //Sonrasında MediaCategories'i boş olmayan media'ları filtreliyoruz (m.MediaCategories!.Count > 0)
                    //Ayrıca bu kategoriye giren fakat kullanıcının izlemiş olduklarını da dışarıda bırakıyoruz (userWatcheds.Contains(m.Id) == false)
                    mediaQuery = _context.Media.Include(m => m.MediaCategories!.Where(mc => mc.CategoryId == mediaCategories.Key)).Where(m => m.MediaCategories!.Count > 0 && userWatcheds.Contains(m.Id) == false);
                    mediaQuery = mediaQuery.Include(m=>m.MediaCategories!.Where(mc=>mc.CategoryId == mediaCategories.Key).Distinct());
                    if (user.Restrictions != null)
                    {
                        //to do
                        //Son olarak, kullanıcı bir restrictiona sahipse seçilen media içerisinden bunları da çıkarmamız gerekiyor.
                        mediaQuery = mediaQuery.Include(m => m.MediaRestrictions!.Where(r => r.RestrictionId <= user.Restrictions));
                    }
                    medias = mediaQuery.ToList();
                }
            }
            return Ok(medias);
        }


        [HttpPost("Logout")]
        [Authorize]
        public void Logout()
        {
            _signInManager.SignOutAsync().Wait();
        }

        [HttpPost("ResetPassword")]
        public ActionResult<string> ResetPassword(long Id)
        {
            SOFTITOFLIXUser? user = _signInManager.UserManager.FindByIdAsync(Id.ToString()).Result;
            if (user != null)
            {
                return _signInManager.UserManager.GeneratePasswordResetTokenAsync(user!).Result;
            }
            return NotFound();
        }

        [HttpPost("ValidateToken")]
        public ActionResult<string> ValidateToken(ValidateTokenVM validateTokenVM)
        {
            SOFTITOFLIXUser? user = _signInManager.UserManager.FindByIdAsync(validateTokenVM.Id.ToString()).Result;
            if(user == null)
            {
                return BadRequest();   
            }
            IdentityResult? identityResult = _signInManager.UserManager.ResetPasswordAsync
                (user!, 
                validateTokenVM.ResetPasswordToken, 
                validateTokenVM.NewPassword
                ).Result;
            return Ok(identityResult.Succeeded);
            
        }
    }
}
