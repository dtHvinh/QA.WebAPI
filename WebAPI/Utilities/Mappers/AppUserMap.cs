﻿using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Response.AppUserResponses;
using WebAPI.Response.AuthResponses;

namespace WebAPI.Utilities.Mappers;

public static class AppUserMap
{
    public static AuthResponse ToAuthResponseDto(this AppUser user, string accessToken, string refreshToken, string role)
    {
        return new AuthResponse(accessToken, refreshToken, user.UserName!, user.ProfilePicture, role);
    }

    public static AppUser ToAppUser(this RegisterDto dto)
    {
        return new AppUser
        {
            UserName = $"user{Random.Shared.Next(100)}{Random.Shared.Next(100000)}",
            Email = dto.Email
        };
    }

    public static AuthorResponse? ToAuthorResponse(this AppUser? obj)
    {
        return obj is null ? null :
            new AuthorResponse
            {
                Id = obj.Id,
                Username = obj!.UserName,
                ProfilePicture = obj.ProfilePicture,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Reputation = obj.Reputation,
            };
    }

    public static UserResponse ToUserResponse(this AppUser? obj)
    {
        return new UserResponse
        {
            ProfilePicture = obj!.ProfilePicture,
            Username = obj!.UserName!,
            Reputation = obj.Reputation,
            Bio = obj.Bio,
            DateJoined = obj.DateJoined,
            LastActive = obj.LastActive
        };
    }
}
