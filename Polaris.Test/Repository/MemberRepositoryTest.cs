using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Constant;
using Polaris.Domain.Entity;
using Polaris.Repository;
using Polaris.Test.Util;

namespace Polaris.Test.Repository
{
    [TestClass]
    public class MemberRepositoryTest
    {
        private MemberRepository _repository;
        private PolarisContext _context;

        [TestInitialize]
        public void Setup()
        {
            _context = DatabaseUtil.Create();
            _repository = new MemberRepository(_context);
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Dispose();
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var entity = new Member
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
            entity = await _repository.Create(entity);
            var exists = await _context.Member.AnyAsync(x => x.Id == entity!.Id);
            Assert.IsTrue(exists);
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            var entity = new Member
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
            _context.Member.Add(entity);
            await _context.SaveChangesAsync();
            var removed = await _repository.Remove(entity);
            Assert.IsTrue(removed);
        }

        [TestMethod("Should not be able remove")]
        public async Task NotRemove()
        {
            var entity = new Member
            {
                Id = Guid.NewGuid(),
            };
            var removed = await _repository.Remove(entity);
            Assert.IsFalse(removed);
        }

        [TestMethod("Should be able get")]
        public async Task Get()
        {
            var entity = new Member
            {
                ApplicationNavigation = new Application
                {
                    Name = Guid.NewGuid().ToString(),
                },
                AuthenticationNavigation = new Authentication
                {
                    Type = AuthenticationTypeConstant.EmailOnly,
                    RefreshToken = Guid.NewGuid().ToString()
                },
                UserNavigation = new User
                {
                    Email = $"{Guid.NewGuid()}@email.com",
                    Name = Guid.NewGuid().ToString(),
                    Language = UserLanguageConstant.EN_US
                }
            };
            _context.Member.Add(entity);
            await _context.SaveChangesAsync();
            var response = await _repository.Get(entity);
            Assert.AreEqual(response.Count, 1);
        }

        [TestMethod("Should be able exists")]
        public async Task Exists()
        {
            var entity = new Member
            {
                ApplicationId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
            _context.Member.Add(entity);
            await _context.SaveChangesAsync();
            var exists = await _repository.Exists(entity);
            Assert.IsTrue(exists);

        }
    }
}
