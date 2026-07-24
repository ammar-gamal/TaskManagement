using TaskManagement.Dtos.Auth;
using TaskManagement.Entities;

namespace TaskManagement.ExtensionMethods.Mapping
{
    public static class UserMappingExtensions
    {
        public static User ToEntity(this RegisterDto dto) =>
        new()
        {
            Username = dto.Username
        };

    }
}
