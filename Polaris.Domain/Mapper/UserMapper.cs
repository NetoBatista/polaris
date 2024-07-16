using Polaris.Domain.Dto.User;
using Polaris.Domain.Entity;

namespace Polaris.Domain.Mapper
{
    public static class UserMapper
    {
        public static User ToEntity(UserCreateRequestDTO request)
        {
            return new User
            {
                Name = request.Name,
                Email = request.Email,
                Language = request.Language
            };
        }

        public static User ToEntity(UserUpdateRequestDTO request)
        {
            return new User
            {
                Id = request.Id,
                Name = request.Name,
                Language = request.Language,
            };
        }

        public static User ToEntity(UserRemoveRequestDTO request)
        {
            return new User
            {
                Id = request.Id
            };
        }

        public static UserResponseDTO ToResponseDTO(User entity)
        {
            return new UserResponseDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Language = entity.Language,
                Email = entity.Email,
            };
        }

        public static List<UserResponseDTO> ToResponseDTO(List<User> entity)
        {
            return entity.Select(ToResponseDTO).ToList();
        }
    }
}
