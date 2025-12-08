using HexagonalModular.Core.Identity.ValueObjects;
using HexagonalModular.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexagonalModular.Core.Identity.Entities
{
    public class UserDomain
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsActive { get; private set; }

        private UserDomain() { }

        public UserDomain(string name, Email email, string passwordHash)
        {
            Id = GuidFactory.NewSequential();
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
        }
        public UserDomain(Guid id, string name, Email email, string passwordHash, bool isActive)
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = isActive;
        }

        public void ChangePassword(string newHash)
        {
            PasswordHash = newHash ?? throw new ArgumentNullException(nameof(newHash));
        }

        public void Deactivate() => IsActive = false;
    }
}

