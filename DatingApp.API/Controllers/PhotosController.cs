using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Route("api/users/{UserId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _Repo;
        private readonly IMapper _Mapper;
        private readonly Microsoft.Extensions.Options.IOptions<CloudinarySettings> _CLoudinaryConfig;
        private readonly Cloudinary _Cloudinary;

        public PhotosController(IDatingRepository Repo, IMapper Mapper, IOptions<CloudinarySettings> CLoudinaryConfig)
        {
            _Repo = Repo;
            _Mapper = Mapper;
            _CLoudinaryConfig = CLoudinaryConfig;

            Account account = new Account
            (
                _CLoudinaryConfig.Value.CloudName,
                _CLoudinaryConfig.Value.ApiKey,
                _CLoudinaryConfig.Value.ApiSecret
            );
            _Cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _Repo.GetPhoto(id);

            var photo = _Mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int UserId, [FromForm] PhotoForCreationDto PhotoForCreationDto)
        {
            if (UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _Repo.GetUser(UserId, true);

            var file = PhotoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if ((null != file) && (file.Length > 0))
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _Cloudinary.Upload(uploadParams);
                }
            }

            PhotoForCreationDto.Url = uploadResult.Url.ToString();
            PhotoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _Mapper.Map<Photo>(PhotoForCreationDto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);

            if (await _Repo.SaveAll())
            {
                var photoForReturn = _Mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { UserId = UserId, id = photo.Id }, photoForReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int UserId, int id)
        {
            if (UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _Repo.GetUser(UserId, true);

            if (!user.Photos.Any(p => id == p.Id))
                return Unauthorized();

            var photoFromRepo = await _Repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _Repo.GetMainPhotoFoUser(UserId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _Repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");

        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeletePhoto(int UserId, int Id)
        {
            if (UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _Repo.GetUser(UserId, true);

            if (!user.Photos.Any(p => Id == p.Id))
                return Unauthorized();

            var photoFromRepo = await _Repo.GetPhoto(Id);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo");

            if (null != photoFromRepo.PublicId)
            {
                var result = _Cloudinary.Destroy(new DeletionParams(photoFromRepo.PublicId));

                if ("ok" == result.Result)
                    _Repo.Delete(photoFromRepo);
            }
            else
                _Repo.Delete(photoFromRepo);

            if (await _Repo.SaveAll())
                return Ok();

            return BadRequest("Failed to dfelete the photo");
        }
    }
}

