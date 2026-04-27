namespace ServiceHub.Helpers
{
    public static class RoleHelper
    {
        public static string GetRoleDisplay(string role) => role switch
        {
            "Admin" => "Администратор",
            "Chief" => "Начальник",
            "User" => "Пользователь",
            _ => role
        };
    }
}