using Microsoft.EntityFrameworkCore;
using Polaris.Domain.Constant;
using Polaris.Domain.Entity;
using Polaris.Repository;
using Polaris.Test.Util;

namespace Polaris.Test.Repository
{
    [TestClass]
    public class UserRepositoryTest
    {
        private UserRepository _repository;
        private PolarisContext _context;

        [TestInitialize]
        public void Setup()
        {
            _context = DatabaseUtil.Create();
            _repository = new UserRepository(_context);
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Dispose();
        }

        [TestMethod("Should be able AlreadyCreated")]
        public async Task AlreadyCreated()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            var alreadyCreated = await _repository.Exists(entity);
            Assert.IsTrue(alreadyCreated);
        }

        [TestMethod("Should not be able AlreadyCreated")]
        public async Task NotAlreadyCreated()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com"
            };
            var alreadyCreated = await _repository.Exists(entity);
            Assert.IsFalse(alreadyCreated);
        }

        [TestMethod("Should be able create")]
        public async Task Create()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            entity = await _repository.Create(entity);
            var created = await _context.User.AnyAsync();
            Assert.IsTrue(created);
        }

        [TestMethod("Should be able get by email")]
        public async Task GetByEmail()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            var responseUser = await _repository.Get(entity);
            Assert.IsNotNull(responseUser);
        }

        [TestMethod("Should not be able get by email")]
        public async Task NotGetByEmail()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com"
            };
            var responseUser = await _repository.Get(entity);
            Assert.IsNull(responseUser);
        }

        [TestMethod("Should be able get all")]
        public async Task GetAll()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            var responseUser = await _repository.Get();
            Assert.AreEqual(responseUser.Count, 1);
        }

        [TestMethod("Should be able remove")]
        public async Task Remove()
        {
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            var removed = await _repository.Remove(entity);
            Assert.IsTrue(removed);
        }

        [TestMethod("Should not be able remove")]
        public async Task NotRemove()
        {
            var entity = new User
            {
                Id = Guid.NewGuid()
            };
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _repository.Remove(entity));
        }

        [TestMethod("Should be able update")]
        public async Task Update()
        {
            var newName = Guid.NewGuid().ToString();
            var newLanguage = UserLanguageConstant.PT_BR;
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
            entity.Name = newName;
            entity.Language = newLanguage;
            var updated = await _repository.Update(entity);
            Assert.IsTrue(updated);
        }

        [TestMethod("Should not be able update")]
        public async Task NotUpdate()
        {
            var newName = Guid.NewGuid().ToString();
            var newLanguage = UserLanguageConstant.PT_BR;
            var entity = new User
            {
                Email = $"{Guid.NewGuid()}@email.com",
                Name = Guid.NewGuid().ToString(),
                Language = UserLanguageConstant.EN_US
            };
            entity.Name = newName;
            entity.Language = newLanguage;
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _repository.Update(entity));
        }
    }
}
