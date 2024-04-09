using SOFTITOFLIX.DTO.Requests;
using SOFTITOFLIX.DTO.Responses;
using SOFTITOFLIX.Models;
using System.Collections.Generic;

namespace SOFTITOFLIX.DTO.Converters
{
    public class UserConverter
    {
        public SOFTITOFLIXUser Convert(UserCreateRequest request)
        {
            SOFTITOFLIXUser newSOFTITOFLIXUser = new()
            {
                UserName = request.Email,

                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                BirthDate = request.BirthDate,

                Passive = false,

            };
            return newSOFTITOFLIXUser;
        }

        public UserGetResponse Convert(SOFTITOFLIXUser SOFTiTOFLIXUser)
        {
            UserGetResponse newUserResponse = new()
            {
                Id = SOFTiTOFLIXUser.Id,
                UserName = SOFTiTOFLIXUser.UserName!,
                Name = SOFTiTOFLIXUser.Name,
                Email = SOFTiTOFLIXUser.Email!,
                PhoneNumber = SOFTiTOFLIXUser.PhoneNumber!,
                BirthDate = SOFTiTOFLIXUser.BirthDate,

                Passive = SOFTiTOFLIXUser.Passive,

            };
            return newUserResponse;
        }
        public List<UserGetResponse> Convert(List<SOFTITOFLIXUser> SOFTiTOFLIXUsers)
        {
            List<UserGetResponse> usersResponses = new();
            foreach (var user in SOFTiTOFLIXUsers)
            {
                usersResponses.Add(Convert(user));
            }
            return usersResponses;
        }
    }
}
