using Polaris.Domain.Dto.Application;
using Polaris.Domain.Entity;

namespace Polaris.Domain.Mapper
{
    public static class ApplicationMapper
    {
        public static Application ToEntity(ApplicationCreateRequestDTO request)
        {
            return new Application
            {
                Name = request.Name.ToUpper()
            };
        }

        public static Application ToEntity(ApplicationUpdateRequestDTO request)
        {
            return new Application
            {
                Id = request.Id,
                Name = request.Name.ToUpper()
            };
        }

        public static Application ToEntity(ApplicationRemoveRequestDTO request)
        {
            return new Application
            {
                Id = request.Id
            };
        }

        public static ApplicationResponseDTO ToResponseDTO(Application entity)
        {
            return new ApplicationResponseDTO
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static List<ApplicationResponseDTO> ToResponseDTO(List<Application> entity)
        {
            return entity.Select(ToResponseDTO).ToList();
        }
    }
}
