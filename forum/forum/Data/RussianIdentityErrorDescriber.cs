using Microsoft.AspNetCore.Identity;

namespace forum.Data
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() => new() { Code = nameof(DefaultError), Description = "Произошла неизвестная ошибка." };
        public override IdentityError DuplicateEmail(string email) => new() { Code = nameof(DuplicateEmail), Description = $"Email '{email}' уже занят." };
        public override IdentityError DuplicateUserName(string userName) => new() { Code = nameof(DuplicateUserName), Description = $"Имя пользователя '{userName}' уже занято." };
        public override IdentityError InvalidEmail(string email) => new() { Code = nameof(InvalidEmail), Description = $"Email '{email}' указан неверно." };
        public override IdentityError PasswordRequiresDigit() => new() { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру ('0'-'9')." };
        public override IdentityError PasswordRequiresLower() => new() { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать хотя бы одну строчную букву ('a'-'z')." };
        public override IdentityError PasswordRequiresUpper() => new() { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать хотя бы одну заглавную букву ('A'-'Z')." };
        public override IdentityError PasswordTooShort(int length) => new() { Code = nameof(PasswordTooShort), Description = $"Пароль должен быть не короче {length} символов." };
    }
}